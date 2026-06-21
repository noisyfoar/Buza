using System.Net.Http.Headers;
using System.Net.Http.Json;
using Buza.Dtos;

namespace Buza.Tests.TestInfrastructure;

public static class AuthTestHelper
{
    public static async Task<string> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            Login = "admin",
            Password = "admin123"
        });

        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));

        return auth.Token;
    }

    public static void UseBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
