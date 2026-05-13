using FastEndpoints;
using MediatR;
using MyEnglish.Api.Mappings;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Features.UpdateUserProfile;

namespace MyEnglish.Api.Endpoints.Users;

public class UpdateUserProfileEndpoint : Endpoint<UpdateUserProfileRequest, UserDto>
{
    private readonly IMediator _mediator;

    public UpdateUserProfileEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/users/me");
        Roles(); // Requires authentication
        Summary(s =>
        {
            s.Summary = "Update current user profile";
            s.Description = "Updates the authenticated user's profile information";
            s.ExampleRequest = new UpdateUserProfileRequest(
                "John",
                "Doe",
                Domain.Abstractions.Enums.Genders.Male,
                new DateTime(1990, 1, 1));
        });
    }

    public override async Task HandleAsync(UpdateUserProfileRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = req.ToCommand(userId);
        var response = await _mediator.Send(command, ct);
        await SendOkAsync(response, ct);
    }
}