using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Services.OrderAPI.DBContext;
using AutoMapper;
using MT.Services.OrderAPI.Models.DTO;
using MT.Services.OrderAPI.Utility;
using MT.Services.OrderAPI.Models;
using MT.Web.Models;
using Stripe.Checkout;
using Stripe;

namespace MT.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        readonly OrderDbContext _orderDbContext;
        ResponseDto _responseDto;
        readonly IMapper _mapper;
        public OrderController(OrderDbContext orderDbContext, IMapper mapper)
        {
            _responseDto = new ResponseDto();
            _orderDbContext = orderDbContext;
            _mapper = mapper;
        }

        [HttpPost("create-order")]
        public async Task<ResponseDto> CreateOrder([FromBody] ShoppingCartDTO cartDto)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDto.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.OrderStatus.Pending.ToString();
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(cartDto.CartDetails);

                var newOrder = _mapper.Map<OrderHeader>(orderHeaderDTO);
                var orderAddedInDB = (await _orderDbContext.OrderHeaders.AddAsync(newOrder)).Entity;
                await _orderDbContext.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderAddedInDB.OrderHeaderId;
                _responseDto.Result = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost("create-payment-session")]
        public async Task<ResponseDto> CreatePaymentSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Name = stripeRequestDTO.UserDetails.Name,
                    Email = stripeRequestDTO.UserDetails.Email,
                    Address = new AddressOptions
                    {
                        Line1 = stripeRequestDTO.UserDetails.BillingAddress,
                        PostalCode = stripeRequestDTO.UserDetails.BillingZipCode,
                        City = stripeRequestDTO.UserDetails.BillingCity,
                        State = stripeRequestDTO.UserDetails.BillingState,
                        Country = stripeRequestDTO.UserDetails.BillingCountry,
                    }
                };
                var customerService = new CustomerService();
                var newCustomer = customerService.Create(customerCreateOptions);

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>(),
                    CustomerEmail = stripeRequestDTO.OrderHeader.UserEmail,
                    BillingAddressCollection = "required",
                    ShippingAddressCollection = new SessionShippingAddressCollectionOptions()
                    {
                        AllowedCountries = new List<string>() { "IN", "US" }
                    },
                    Currency = "INR"
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.ProductPrice * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.ProductName,
                                Description = item.Product.Description,
                                Images = new List<string>() { item.Product.ImageUrl },
                                Metadata = new Dictionary<string, string>() { { "description", item.Product.Description } }
                            }
                        },
                        Quantity = item.Quantity
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountObj;
                }

                var service = new SessionService();
                Session stripSession = service.Create(options);
                stripeRequestDTO.StripSessionUrl = stripSession.Url;
                stripeRequestDTO.StripSessionId = stripSession.Id;

                OrderHeader orderHeader = _orderDbContext.OrderHeaders.First(x => x.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripSessionId = stripSession.Id;
                _orderDbContext.SaveChanges();

                _responseDto.Result = stripeRequestDTO;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }
    }
}
