using FastEndpoints;
using MediatR;
using MyEnglish.Api.Mappings;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;

namespace MyEnglish.Api.Endpoints.Login;

public class LoginEndpoint : Endpoint<LoginRequest, AuthResponse>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Login with email and password";
            s.Description = "Authenticates user credentials and returns authentication tokens";
            s.ExampleRequest = new LoginRequest("user@example.com", "Password123!");
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var query = req.ToQuery();

        var command = query.ToCommand();

        var response = await _mediator.Send(command, ct);

        await SendOkAsync(response, ct);
    }
}