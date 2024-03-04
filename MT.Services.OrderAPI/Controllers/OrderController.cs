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
using Microsoft.EntityFrameworkCore;
using MT.Services.OrderAPI.Service.Interfaces;
using MT.MessageBus;
using Newtonsoft.Json;

namespace MT.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        readonly OrderDbContext _orderDbContext;
        readonly ICartService _cartService;
        ResponseDto _responseDto;
        readonly IMapper _mapper;
        readonly IMessageBus _messageBus;
        IConfiguration _configuration;
        public OrderController(OrderDbContext orderDbContext, ICartService cartService, IMapper mapper,
            IMessageBus messageBus, IConfiguration configuration)
        {
            _responseDto = new ResponseDto();
            _orderDbContext = orderDbContext;
            _cartService = cartService;
            _mapper = mapper;
            _messageBus = messageBus;
            _configuration = configuration;
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
                var newCustomer = await customerService.CreateAsync(customerCreateOptions);

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
                        AllowedCountries = new List<string>() { "US" } // , "IN"
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

                OrderHeader orderHeader = await _orderDbContext.OrderHeaders
                                                                .FirstAsync(x => x.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripSessionId = stripSession.Id;
                await _orderDbContext.SaveChangesAsync();

                _responseDto.Result = stripeRequestDTO;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost("validate-payment-session")]
        public async Task<ResponseDto> ValidatePaymentSession(int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _orderDbContext.OrderHeaders.FirstAsync(x => x.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session stripSession = service.Get(orderHeader.StripSessionId);

                var paymentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentService.Get(stripSession.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.OrderStatus.Approved.ToString();
                    await _orderDbContext.SaveChangesAsync();

                    await _cartService.RemoveAllAsync(orderHeader?.UserId!);

                    RewardsDTO rewardsDTO = new RewardsDTO
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:NewOrderGeneratedTopic");
                    await _messageBus.PublishMessage(rewardsDTO, topicName);

                    _responseDto.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }


        [HttpGet("get-orders/{userId}")]
        public async Task<ResponseDto> GetAllOrders(string userId = "", string? orderStatus = "")
        {
            try
            {
                IEnumerable<OrderHeader> allOrders;
                if (User.IsInRole(SD.RoleAdmin.ToString()))
                {
                    allOrders = await _orderDbContext.OrderHeaders
                                                    .Include(x => x.OrderDetails)
                                                    .Where(x => !string.IsNullOrWhiteSpace(orderStatus) ? x.Status == orderStatus.ToString() : true)
                                                    .OrderByDescending(o => o.OrderHeaderId)
                                                    .AsNoTracking()
                                                    .ToListAsync();
                }
                else
                {
                    allOrders = await _orderDbContext.OrderHeaders
                                                    .Include(x => x.OrderDetails)
                                                    .Where(x => x.UserId == userId)
                                                    .Where(x => !string.IsNullOrWhiteSpace(orderStatus) ? x.Status == orderStatus.ToString() : true)
                                                    .OrderByDescending(o => o.OrderHeaderId)
                                                    .AsNoTracking()
                                                    .ToListAsync();
                }

                _responseDto.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(allOrders);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpGet("get-order/{orderId:int}/{userId}")]
        public async Task<ResponseDto> GetOrder(int orderId, string userId = "")
        {
            try
            {
                OrderHeader? orderResult;
                if (User.IsInRole(SD.RoleAdmin.ToString()))
                {
                    orderResult = await _orderDbContext.OrderHeaders
                                                    .Include(x => x.OrderDetails)
                                                    .FirstOrDefaultAsync(x => x.OrderHeaderId == orderId);
                }
                else
                {
                    orderResult = await _orderDbContext.OrderHeaders
                                                    .Include(x => x.OrderDetails)
                                                    .FirstOrDefaultAsync(x => x.OrderHeaderId == orderId && x.UserId == userId);
                }

                _responseDto.Result = _mapper.Map<OrderHeaderDTO>(orderResult);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }

            return _responseDto;
        }

        [HttpPost("update-order-status/{orderId:int}/{userId}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string orderStatus, string userId = "")
        {
            try
            {
                OrderHeader? orderFromDb;
                if (User.IsInRole(SD.RoleAdmin.ToString()))
                    orderFromDb = await _orderDbContext.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderId);
                else orderFromDb = await _orderDbContext.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderId && x.UserId == userId);

                if (orderFromDb == null)
                {
                    _responseDto.Message = "Order not found";
                    _responseDto.IsSuccess = false;
                    return _responseDto;
                }


                if ((orderFromDb.Status == SD.OrderStatus.Approved.ToString() || orderFromDb.Status == SD.OrderStatus.ReadyForPickup.ToString())
                    && orderStatus == SD.OrderStatus.Canceled.ToString())
                {
                    var options = new RefundCreateOptions()
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderFromDb.PaymentIntentId,
                    };

                    var service = new RefundService();
                    Refund refundObj = await service.CreateAsync(options);
                }

                orderFromDb.Status = Enum.Parse(typeof(SD.OrderStatus), orderStatus).ToString();

                await _orderDbContext.SaveChangesAsync();
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
