using FastEndpoints;
using MediatR;
using MyEnglish.Api.Mappings;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;

namespace MyEnglish.Api.Endpoints.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, AuthResponse>
{
    private readonly IMediator _mediator;

    public RegisterEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Creates a new user account and returns authentication tokens";
            s.ExampleRequest = new RegisterRequest(
                "user@example.com",
                "Password123!",
                "Password123!",
                "John",
                "Doe",
                Domain.Abstractions.Enums.Genders.Male,
                new DateTime(1990, 1, 1));
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var query = req.ToQuery();
        var command = query.ToCommand();
        var response = await _mediator.Send(command, ct);
        await SendOkAsync(response, ct);
    }
}