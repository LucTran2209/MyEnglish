using FastEndpoints;
using MediatR;
using MyEnglish.Api.Mappings;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Features.RefreshToken;

namespace MyEnglish.Api.Endpoints.Auth;

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, AuthResponse>
{
    private readonly IMediator _mediator;

    public RefreshTokenEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Refresh access token";
            s.Description = "Uses refresh token to generate new access and refresh tokens";
            s.ExampleRequest = new RefreshTokenRequest("refresh_token_here");
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var command = req.ToCommand();
        var response = await _mediator.Send(command, ct);
        await SendOkAsync(response, ct);
    }
}