using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Services.AuthAPI.Models.DTO;
using MT.Services.AuthAPI.RabbmitMQSender;
using MT.Services.AuthAPI.Service.Interface;

namespace MT.Services.AuthAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMQAuthMessageSender _rabbitMQAuthMessageSender;

    public AuthController(IAuthService authService, IRabbitMQAuthMessageSender rabbitMQAuthMessageSender, IConfiguration configuration)
    {
        _authService = authService;
        _rabbitMQAuthMessageSender = rabbitMQAuthMessageSender;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationDTO registration)
    {
        var response = new ResponseDto();
        var errMessage = await _authService.Register(registration);
        if (!string.IsNullOrEmpty(errMessage))
        {
            response.IsSuccess = false;
            response.Message = errMessage;
            return BadRequest(response);
        }

        registration.Password = null;
        var topicOrQueueName = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
        await _rabbitMQAuthMessageSender.SendMessage(registration, topicOrQueueName);

        response.Message = "Registration successful";
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var response = new ResponseDto();
        var loginResponse = await _authService.Login(login);
        if (string.IsNullOrWhiteSpace(loginResponse.ID))
        {
            response.IsSuccess = false;
            response.Message = "Username or password is incorrect";
            return BadRequest(response);
        }

        response.Result = loginResponse;
        response.Message = "Login successful";
        return Ok(response);
    }

    [HttpPost]
    [Route("change-password")]
    [Authorize]
    public async Task<ResponseDto> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        var _responseDto = new ResponseDto();
        try
        {
            var changePasswordResponse = await _authService.ChangePassword(changePasswordDTO);
            if (!string.IsNullOrWhiteSpace(changePasswordResponse))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = changePasswordResponse;
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Result = true;
            }
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RegistrationDTO assignRole)
    {
        var response = new ResponseDto();
        var roleAssigned = await _authService.AssignRole(assignRole.Email, assignRole.RoleName.ToUpper());
        if (!roleAssigned)
        {
            response.IsSuccess = false;
            response.Message = "Error encountered";
            return BadRequest(response);
        }

        response.Message = "Role assigned successfully";
        return Ok(response);
    }
}