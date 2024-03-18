using Discord;
using Discord.WebSocket;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private DiscordSocketClient client;
    private Config config;
    

    static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    
    public async Task MainAsync()
    {
        client = new DiscordSocketClient();
        client.Log += LogAsync;
        client.Ready += ReadyAsync;
        client.MessageReceived += MessageReceivedAsync;


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
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

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
        Console.WriteLine($"{client.CurrentUser} is connected!");

        return Task.CompletedTask;
    }

    // Event handler for the MessageReceived event
    private async Task MessageReceivedAsync(SocketMessage message)
    {
        // Ignore messages from the bot itself to prevent loops
        if (message.Author.Id == client.CurrentUser.Id) return;

        // Respond to the message
        if (message.Content == "!hello")
        {
            await message.Channel.SendMessageAsync("Hello!");
        }
    }
}
