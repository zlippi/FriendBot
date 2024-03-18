using Discord;
using Discord.WebSocket;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private DiscordSocketClient _client;
    private Config config;
    

    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    
    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;


        try
        {
            // Path to JSON file
            string fileName = "config.json";
            
            // Read the file and deserialize it to the Config class
            var configText = await File.ReadAllTextAsync(fileName);
            config = JsonSerializer.Deserialize<Config>(configText);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("The token file was not found.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Error parsing the JSON file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }


        var token = config.token;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");

        return Task.CompletedTask;
    }
}
