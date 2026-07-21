using BracketSmasherBackend.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace BracketSmasherBackend.Services;


public class StartGgService
{

    private readonly HttpClient _client;



    public StartGgService(
        HttpClient client,
        IConfiguration configuration)
    {
        _client = client;


        var token =
            configuration["StartGg:Token"];



        if (!string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );
        }
    }





    public async Task<string?> GetTournamentEmailAsync(
        long tournamentId)
    {

        var query =
        """
        query($id: ID!)
        {
            tournament(id:$id)
            {
                primaryContact
                primaryContactType
            }
        }
        """;



        var response =
            await _client.PostAsJsonAsync(
                "",
                new
                {
                    query,

                    variables = new
                    {
                        id = tournamentId
                    }
                });



        if (!response.IsSuccessStatusCode)
            return null;






        var json =
            await response.Content
            .ReadFromJsonAsync<JsonElement>();




        if (
            !json.TryGetProperty(
                "data",
                out var data)
        )
        {
            return null;
        }





        var tournament =
            data
            .GetProperty("tournament");





        var type =
            tournament
            .GetProperty("primaryContactType")
            .GetString();





        if (type != "email")
            return null;





        return tournament
            .GetProperty("primaryContact")
            .GetString();
    }

    public async Task<TournamentInfo?> GetTournamentInfoAsync(long tournamentId)
    {
        var query = """
    query($id: ID!) {
      tournament(id:$id) {
        name
        primaryContact
        primaryContactType
      }
    }
    """;


        var response =
            await _client.PostAsJsonAsync(
                "",
                new
                {
                    query,
                    variables = new
                    {
                        id = tournamentId
                    }
                });


        if (!response.IsSuccessStatusCode)
            return null;


        var json =
            await response.Content
            .ReadFromJsonAsync<JsonElement>();


        var tournament =
            json.GetProperty("data")
            .GetProperty("tournament");


        return new TournamentInfo
        {
            Name =
                tournament
                .GetProperty("name")
                .GetString()
                ?? "",

            Email =
                tournament
                .GetProperty("primaryContact")
                .GetString()
        };
    }

    public async Task<string?> GetEventNameAsync(long eventId)
    {
        var query = """
    query($id: ID!) {
      event(id:$id) {
        name
      }
    }
    """;

        var response =
            await _client.PostAsJsonAsync(
                "",
                new
                {
                    query,
                    variables = new
                    {
                        id = eventId
                    }
                });

        if (!response.IsSuccessStatusCode)
            return null;

        using var stream =
            await response.Content.ReadAsStreamAsync();

        var json =
            await JsonDocument.ParseAsync(stream);

        if (!json.RootElement.TryGetProperty("data", out var data))
            return null;

        if (!data.TryGetProperty("event", out var ev))
            return null;

        return ev.GetProperty("name").GetString();
    }
}