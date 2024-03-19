using Discord;
using Discord.WebSocket;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI_API;

class Program
{
    private DiscordSocketClient client;
    private Config config;
    private ChatBot openai;

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

        openai = new ChatBot(config.OpenAIToken);
        var discordtoken = config.DiscordToken;
        await client.LoginAsync(TokenType.Bot, discordtoken);
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

        // Message is from a guild
        if (message.Channel is IGuildChannel)
        {

            bool botIsMentioned = message.MentionedUsers.Any(user => user.Id == client.CurrentUser.Id);
            // If the bot or everyone is mentioned...
            if (botIsMentioned || message.MentionedEveryone)
            {

                // Retrieve the last 10 messages sent in the channel before the received message
                var messages = await message.Channel.GetMessagesAsync(message.Id, Direction.Before, limit: 10).FlattenAsync();
                

                string history = "";
                // Process the retrieved messages and concats them to a string
                foreach (var msg in messages)
                {
                    history += $"[{msg.Timestamp}] {msg.Author}: {msg.Content}\n";
                    Console.WriteLine($"[{msg.Timestamp}] {msg.Author}: {msg.Content}");
                }
                // Passes the current message and history of messages
                var typing = message.Channel.EnterTypingState();
                var response = openai.Conversate($"[{message.Timestamp}] {message.Author}: {message.Content}", history);
                await message.Channel.SendMessageAsync(response.Result);
                typing.Dispose();
            }
        }
    }
}
