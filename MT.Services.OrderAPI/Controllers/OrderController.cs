using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Services.OrderAPI.DBContext;
using AutoMapper;
using MT.Services.OrderAPI.Models.DTO;
using MT.Services.OrderAPI.Service.Interfaces;
using MT.Services.OrderAPI.Utility;
using MT.Services.OrderAPI.Models;

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
    }
}
