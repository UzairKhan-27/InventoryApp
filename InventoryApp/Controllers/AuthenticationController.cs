using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        try
        {
            var (success, token, message) = await _service.Login(dto.Username, dto.Password);
            if (!success) return Unauthorized(message);

            return Ok(token);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize(Roles = "CentralAdmin")]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        try
        {
            var userContext = new UserContext(User);
            string changedBy = userContext.UserId.ToString();
            var (success, message) = await _service.Register(dto.Username, dto.Password, dto.Role, dto.StoreId, changedBy);
            if (!success) 
                return BadRequest(message);

            return Ok(message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

}

