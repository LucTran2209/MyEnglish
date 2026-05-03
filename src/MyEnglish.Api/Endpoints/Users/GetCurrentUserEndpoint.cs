using FastEndpoints;
using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Features.GetUserById;

namespace MyEnglish.Api.Endpoints.Users;

public class GetCurrentUserRequest
{
    // Empty request for GET endpoint
}

public class GetCurrentUserEndpoint : Endpoint<GetCurrentUserRequest, UserDto>
{
    private readonly IMediator _mediator;

    public GetCurrentUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/users/me");
        Roles(); // Requires authentication
        Summary(s =>
        {
            s.Summary = "Get current user information";
            s.Description = "Returns the authenticated user's profile information";
        });
    }

    public override async Task HandleAsync(GetCurrentUserRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserByIdQuery(userId);
        var userDto = await _mediator.Send(query, ct);

        if (userDto == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(userDto, ct);
    }
}