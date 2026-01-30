using System.Net.Http.Headers;

namespace BookHub.BlazorClient.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly AuthStateProvider _authState;

    public AuthMessageHandler(AuthStateProvider authState)
    {
        _authState = authState;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _authState.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
