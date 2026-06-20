using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Dtos.Auth;

/// <summary>Email/password credentials posted to <c>POST /api/auth/login</c>.</summary>
public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
