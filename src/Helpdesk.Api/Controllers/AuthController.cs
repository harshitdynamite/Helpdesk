using System.Security.Claims;
using Helpdesk.Api.Dtos.Auth;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Interfaces;
using Helpdesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Api.Controllers;

/// <summary>
/// Email/password authentication. There is no logout endpoint: tokens are stateless, so a
/// client "logs out" by discarding its token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    /// <summary>Validates credentials and returns a signed JWT on success, otherwise 401.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var token = _tokenService.CreateToken(user.Id, user.Email!, user.Role);
        return Ok(new LoginResponse(
            token.Token, user.Email!, user.DisplayName, user.Role.ToString(), token.ExpiresAtUtc));
    }

    /// <summary>Echoes the caller's identity from the token — proves the scheme works.</summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<MeResponse> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
        return Ok(new MeResponse(id, email, role));
    }

    /// <summary>
    /// Admin-only: creates an Agent login. There is no self-service sign-up — an admin
    /// provisions every agent account.
    /// </summary>
    [HttpPost("agents")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<UserResponse>> CreateAgent(CreateAgentRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return Conflict($"A user with email '{request.Email}' already exists.");

        var agent = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            Role = UserRole.Agent,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(agent, request.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
            return ValidationProblem(ModelState);
        }

        var response = new UserResponse(agent.Id, agent.Email!, agent.DisplayName, agent.Role.ToString());
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
