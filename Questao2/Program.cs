using System.Text.Json;

public class Program
{
    /// <summary>
    /// Url fornecida para os endpoints da WebApi.
    /// </summary>
    const string BaseUrlApi = "https://jsonmock.hackerrank.com/api/football_matches";
    /// <summary>
    /// Recurso adotado para a utilização de sockets.
    /// </summary>
    /// <remarks>
    /// Vamos utilizar um comportamento estático, com o objetivo de evitar criar novas
    /// instâncias, que potencialmente vão exaurir os recursos(sockets) escassos 
    /// disponíveis do sistema operacional, para assim efetuar seu reaproveitamento.
    /// </remarks>
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(BaseUrlApi)
    };
    /// <summary>
    /// Ponto de entrada da aplicação com suporte assíncrono.
    /// </summary>
    /// <remarks>
    /// Como vamos consumir algumas funcionalidades assíncronas, 
    /// vamos habilitar esse recurso de forma nativa.
    /// </remarks>
    public static async Task Main() => await GetScoresAsync();
    private static async Task GetScoresAsync()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int? totalGoals = await GetTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoalsAsync(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        Console.WriteLine("Pressione qualquer tecla para finalizar.");

        Console.ReadKey();
        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }
    private static async Task<int?> GetTotalScoredGoalsAsync(string team, int year)
    {
        int? totalGoals = 0;

        int pagingCountTeam1 = 1, pagingCountTeam2 = 1;

        for (int i = 1, j = 1; i <= pagingCountTeam1 || j <= pagingCountTeam2; i++, j++)
        {
            if (i <= pagingCountTeam1)
            {
                var team1Request = await GetScoredGoalsPerPageAsync(team1: team, year, page: i);
                totalGoals += team1Request?.Data?.Sum(t => int.Parse(t.Team1Goals!));
                if (team1Request?.Total_Pages > i) pagingCountTeam1++;
            }

            if (j <= pagingCountTeam2)
            {
                var team2Request = await GetScoredGoalsPerPageAsync(year: year, team2: team, page: j);
                totalGoals += team2Request?.Data?.Sum(t => int.Parse(t.Team2Goals!));
                if (team2Request?.Total_Pages > i) pagingCountTeam2++;
            }
        }

        return totalGoals;
    }    
    private static async Task<TeamScoreResponse?> GetScoredGoalsPerPageAsync(string? team1 = null, int? year = null,
        string? team2 = null, int? page = null)
    {
        using HttpRequestMessage? message = BuildGetMessage(team1, year, team2, page);

        using var response = await _httpClient.SendAsync(message!);

        if (response.IsSuccessStatusCode)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();

            var teamResponse = JsonSerializer.Deserialize<TeamScoreResponse>(jsonContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            return teamResponse;
        }

        return null;
    }
    [Serializable]
    class TeamScoreResponse
    {
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public List<TeamScore> Data { get; set; } = null!;
    }
    [Serializable]
    class TeamScore
    {
        public string Competition { get; set; } = null!;
        public int Year { get; set; }
        public string Round { get; set; } = null!;
        public string Team1 { get; set; } = null!;
        public string Team2 { get; set; } = null!;
        public string Team1Goals { get; set; } = null!;
        public string Team2Goals { get; set; } = null!;
    }

    /// <summary>     
    /// </summary> 
    /// <param name="team1">Filtra o nome do time 1 da partida.</param>
    /// <param name="year">Filtra o ano de pesquisa.</param>
    /// <param name="team2">Filtra o nome do time 2 da partida.</param>
    /// <param name="page">Filtra o número da página de resultados.</param>
    /// <returns>
    /// Objeto resultante para compor a mensagem de requisição.
    /// </returns>
    /// <remarks>
    /// Convencionado com base na documentação fornecida.
    /// Modelo de resposta:
    /// {"page":1,"per_page":10,"total":3,"total_pages":1,"data":[
    /// {"competition":"UEFA Champions League", "year":2015,"round":"GroupC","team1":"Galatasaray","team2":"Atletico Madrid","team1goals":"0","team2goals":"2"},
    /// {"competition":"UEFA Champions League","year":2015,"round":"GroupC","team1":"Galatasaray","team2":"SL Benfica","team1goals":"2","team2goals":"1"},
    /// {"competition":"UEFA Champions League","year":2015,"round":"GroupC","team1":"Galatasaray","team2":"Astana","team1goals":"1","team2goals":"1"}
    /// ]}
    /// </remarks>
    private static HttpRequestMessage? BuildGetMessage(string? team1 = null, int? year = null,
        string? team2 = null, int? page = null)
    {
        List<KeyValuePair<string, object?>> pieces = new()
        {
            new(nameof(team1), team1),
            new(nameof(year), year),
            new(nameof(team2), team2),
            new(nameof(page), page)
        };

        if (!pieces.Any(x => x.Value is not null))
        {
            return null;
        }

        List<string> items = new();

        foreach (var piece in pieces)
        {
            if (piece.Value is not null)
            {
                items.Add(string.Concat(piece.Key, "=", piece.Value));
            }
        }

        var queryStringUri = string.Concat("?", string.Join("&", items), "\r\n");

        return new HttpRequestMessage(HttpMethod.Get, queryStringUri);
    }
}