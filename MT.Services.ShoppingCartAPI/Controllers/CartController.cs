using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MT.MessageBus;
using MT.Services.CouponAPI.DBContext;
using MT.Services.ShoppingCartAPI.Models;
using MT.Services.ShoppingCartAPI.Models.DTO;
using MT.Services.ShoppingCartAPI.RabbitMQSender;
using MT.Services.ShoppingCartAPI.Service.Interfaces;

namespace MT.Services.ShoppingCartAPI.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ShoppingCartDbContext _cartDbContext;
    ResponseDto _responseDto;
    IMapper _mapper;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private readonly IUserService _userService;
    private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
    private readonly IConfiguration _configuration;

    public CartController(ShoppingCartDbContext cartDbContext, IMapper mapper,
            IProductService productService, ICouponService couponService, IUserService userService,
            IRabbitMQCartMessageSender rabbitMQCartMessageSender, IConfiguration configuration)
    {
        _cartDbContext = cartDbContext;
        _responseDto = new ResponseDto();
        _mapper = mapper;
        _productService = productService;
        _couponService = couponService;
        _userService = userService;
        _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("upsert")]
    public async Task<ResponseDto> Upsert([FromBody] ShoppingCartDTO shoppingCart)
    {
        try
        {
            var cartHeaderFromDb = await _cartDbContext.CartHeaders
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync(u => u.UserId == shoppingCart.CartHeader.UserId);
            if (cartHeaderFromDb == null)
            {
                var newCartHeader = _mapper.Map<CartHeader>(shoppingCart.CartHeader);
                await _cartDbContext.CartHeaders.AddAsync(newCartHeader);
                await _cartDbContext.SaveChangesAsync();

                shoppingCart.CartDetails.First().CartHeaderId = newCartHeader.CartHeaderId;
                var newCartDetails = _mapper.Map<List<CartDetail>>(shoppingCart.CartDetails);
                await _cartDbContext.CartDetails.AddRangeAsync(newCartDetails);
                await _cartDbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _cartDbContext.CartDetails
                                                            .AsNoTracking()
                                                            .FirstOrDefaultAsync(u => u.ProductId == shoppingCart.CartDetails.First().ProductId &&
                                                                                 u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    shoppingCart.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    var newCartDetails = _mapper.Map<List<CartDetail>>(shoppingCart.CartDetails);
                    await _cartDbContext.CartDetails.AddRangeAsync(newCartDetails);
                    await _cartDbContext.SaveChangesAsync();
                }
                else
                {
                    if (!shoppingCart.DirectUpdate)
                        shoppingCart.CartDetails.First().Quantity += cartDetailsFromDb.Quantity;
                    shoppingCart.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    shoppingCart.CartDetails.First().CartDetailId = cartDetailsFromDb.CartDetailId;

                    if (shoppingCart.CartDetails.First().Quantity <= 0)
                    {
                        _responseDto.Message = "Invalid item quantity entered";
                        _responseDto.IsSuccess = false;
                        return _responseDto;
                    }
                    var updatedCartDetails = _mapper.Map<List<CartDetail>>(shoppingCart.CartDetails);
                    _cartDbContext.CartDetails.UpdateRange(updatedCartDetails);
                    await _cartDbContext.SaveChangesAsync();

                    if (!string.IsNullOrWhiteSpace(cartHeaderFromDb.CouponCode))
                        await IsCouponApplicable(cartHeaderFromDb.CartHeaderId);
                }
            }
            _responseDto.Result = shoppingCart;
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }

    [HttpPost]
    [Route("remove-item")]
    public async Task<ResponseDto> RemoveItem(int cartHeaderId, int cartDetailId)
    {
        try
        {
            var removeCartItem = await _cartDbContext.CartDetails.FirstAsync(x => x.CartDetailId == cartDetailId && x.CartHeaderId == cartHeaderId);
            var cartTotalCount = await _cartDbContext.CartDetails.CountAsync(x => x.CartHeaderId == cartHeaderId);
            _cartDbContext.CartDetails.Remove(removeCartItem);
            if (cartTotalCount == 1)
            {
                var removeCartHeader = _cartDbContext.CartHeaders.First(c => c.CartHeaderId == cartHeaderId);
                _cartDbContext.CartHeaders.Remove(removeCartHeader);
            }
            await _cartDbContext.SaveChangesAsync();

            if (cartTotalCount > 0)
                await IsCouponApplicable(cartHeaderId);

            _responseDto.Result = true;
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }

    [HttpPost]
    [Route("remove-all-items")]
    public async Task<ResponseDto> RemoveAllItems(string userId)
    {
        try
        {
            var removeCartHeader = _cartDbContext.CartHeaders.First(c => c.UserId == userId);
            var removeCartDetails = _cartDbContext.CartDetails.Where(c => c.CartHeaderId == removeCartHeader.CartHeaderId);

            _cartDbContext.CartDetails.RemoveRange(removeCartDetails);
            _cartDbContext.CartHeaders.Remove(removeCartHeader);

            await _cartDbContext.SaveChangesAsync();
            _responseDto.Result = true;
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
            _responseDto.Result = false;
        }
        return _responseDto;
    }

    [HttpGet]
    [Route("get-items/{userId}")]
    public async Task<ResponseDto> GetItems(string userId, bool loadUser = false)
    {
        try
        {
            var cartHeader = await _cartDbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cartHeader != null)
            {
                var cartDetails = _cartDbContext.CartDetails.Where(x => x.CartHeaderId == cartHeader.CartHeaderId).ToList();
                var shoppingCartDTO = new ShoppingCartDTO()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(cartHeader),
                    CartDetails = _mapper.Map<List<CartDetailDTO>>(cartDetails)
                };

                var allProducts = await _productService.GetProductsAsync();
                foreach (var cartDetail in shoppingCartDTO.CartDetails)
                {
                    cartDetail.Product = allProducts.First(p => p.ProductId == cartDetail.ProductId);
                    cartDetail.CartHeader = null;
                    shoppingCartDTO.CartHeader.CartTotal += (cartDetail.Product.Price * cartDetail.Quantity);
                }

                if (!string.IsNullOrWhiteSpace(cartHeader.CouponCode))
                {
                    var couponFromDb = await _couponService.GetCouponByCode(cartHeader.CouponCode);
                    if (couponFromDb != null && couponFromDb.CouponId > 0)
                    {
                        shoppingCartDTO.CartHeader.CartTotal -= couponFromDb.DiscountAmount;
                        shoppingCartDTO.CartHeader.Discount = couponFromDb.DiscountAmount;
                    }
                }

                if (loadUser)
                    shoppingCartDTO.User = await _userService.GetUserAsync(cartHeader.UserId);

                _responseDto.Result = shoppingCartDTO;
            }
            else _responseDto.Result = new ShoppingCartDTO();
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }

    [HttpPost("apply-coupon")]
    public async Task<ResponseDto> ApplyCoupon([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        try
        {
            var cartFromDb = await _cartDbContext.CartHeaders
                                                .FirstAsync(c => c.UserId == shoppingCartDTO.CartHeader.UserId);

            if (cartFromDb != null)
            {
                if (string.IsNullOrEmpty(shoppingCartDTO.CartHeader.CouponCode))
                {
                    cartFromDb.CouponCode = null;
                    _responseDto.Result = true;
                }
                else
                {
                    var couponFromDb = await _couponService.GetCouponByCode(shoppingCartDTO.CartHeader.CouponCode);
                    if (couponFromDb == null || couponFromDb.CouponId == 0)
                    {
                        cartFromDb.CouponCode = null;
                        _responseDto.Message = "Invalid coupon!";
                        _responseDto.IsSuccess = false;
                    }
                    else
                    {
                        var cartDetailFromDb = _cartDbContext.CartDetails.AsNoTracking().Where(x => x.CartHeaderId == cartFromDb.CartHeaderId).ToList();
                        var allProducts = await _productService.GetProductsAsync();
                        var cartTotal = 0d;
                        foreach (var cartDetail in cartDetailFromDb)
                        {
                            cartDetail.Product = allProducts.First(p => p.ProductId == cartDetail.ProductId);
                            cartTotal += (cartDetail.Product.Price * cartDetail.Quantity);
                        }

                        if (cartTotal < couponFromDb.MinimumAmount)
                        {
                            _responseDto.Message = "To apply this coupon, cart total should be greater then or equal to " + couponFromDb.MinimumAmount.ToString("c");
                            _responseDto.IsSuccess = false;
                            cartFromDb.CouponCode = null;
                        }
                        else
                        {
                            cartFromDb.CouponCode = couponFromDb.CouponCode;
                            _responseDto.Result = true;
                        }
                    }
                }

                _cartDbContext.Update(cartFromDb);
                await _cartDbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }


    [HttpPost("email-cart-request")]
    public async Task<ResponseDto> EmailCartRequest([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        try
        {
            var topicOrQueueName = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            await _rabbitMQCartMessageSender.SendMessage(shoppingCartDTO, topicOrQueueName);
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }


    [NonAction]
    private async Task IsCouponApplicable(int cartHeaderId)
    {
        var allProducts = await _productService.GetProductsAsync();
        var finalCartTotal = 0d;

        var cartHeaderFromDb = _cartDbContext.CartHeaders.First(c => c.CartHeaderId == cartHeaderId);
        var allCartDetailsFromDb = _cartDbContext.CartDetails
                                                .AsNoTracking()
                                                .Where(u => u.CartHeaderId == cartHeaderId)
                                                .ToList();

        foreach (var cartItem in allCartDetailsFromDb)
        {
            finalCartTotal += (allProducts.First(x => x.ProductId == cartItem.ProductId).Price * cartItem.Quantity);
        }

        var couponFromDb = await _couponService.GetCouponByCode(cartHeaderFromDb.CouponCode);
        if (finalCartTotal < couponFromDb.MinimumAmount)
        {
            cartHeaderFromDb.CouponCode = null;
            _cartDbContext.CartHeaders.Update(cartHeaderFromDb);
            await _cartDbContext.SaveChangesAsync();
        }
    }
}