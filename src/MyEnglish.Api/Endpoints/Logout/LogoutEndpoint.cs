using FastEndpoints;
using MediatR;
using MyEnglish.Application.Features.Logout;

namespace MyEnglish.Api.Endpoints.Auth;

public class LogoutEndpoint : Endpoint<LogoutRequest>
{
    private readonly IMediator _mediator;

    public LogoutEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/logout");
        Roles(); // Requires authentication
        Summary(s =>
        {
            s.Summary = "Logout user";
            s.Description = "Revokes refresh tokens. If no refresh token provided, revokes all tokens (logout from all devices)";
            s.ExampleRequest = new LogoutRequest("refresh_token_here");
        });
    }

    public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new LogoutCommand(userId, req.RefreshToken);
        await _mediator.Send(command, ct);
        
        await SendOkAsync(ct);
    }
}