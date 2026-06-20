using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Dtos.Auth;

/// <summary>Admin-supplied details for a new Agent login (<c>POST /api/auth/agents</c>).</summary>
public record CreateAgentRequest(
    [Required, EmailAddress] string Email,
    [Required] string DisplayName,
    [Required] string Password);
