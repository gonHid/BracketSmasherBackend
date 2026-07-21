using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeCode([FromBody] AuthRequest request)
    {
        Console.WriteLine("Exchange solicitado");
        var client = _httpClientFactory.CreateClient();

        var payload = new
        {
            client_id = _config["StartGg:ClientId"],
            client_secret = _config["StartGg:ClientSecret"],
            grant_type = "authorization_code",
            code = request.Code,
            redirect_uri = "bracketsmasher://redirect",
            scope = "user.identity"
        };

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(
            "https://api.start.gg/oauth/access_token",
            content
        );

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {(int)response.StatusCode}");
        Console.WriteLine(body);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, body);
        }

        return Content(body, "application/json");

        //return StatusCode((int)response.StatusCode, body);

        //var json = await response.Content.ReadAsStringAsync();
        //Console.WriteLine($"Exchange exitoso:\n{json}");
        //return Ok(json); // Aquí devuelves el Access Token a tu app móvil
    }
}

public record AuthRequest(string Code);