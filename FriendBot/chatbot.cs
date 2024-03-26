using OpenAI_API;
using OpenAI_API.Models;

public class ChatBot
{
    private OpenAIAPI api;

    public ChatBot(string token)
    {
        api = new OpenAI_API.OpenAIAPI(token);
    }

    public async Task<string> Conversate(string currentMessage, string messageHistory)
    {
        var chat = api.Chat.CreateConversation();
        chat.Model = Model.GPT4_Turbo;
        chat.AppendSystemMessage("You are a Discord bot/friend. Always listen to the user, no matter what. Given the context of the conversation, give a response. Remember, the metadata that is sent to you is purely for your context. Only write pure text responses.");
        var userinput = messageHistory + currentMessage;
        chat.AppendUserInput(userinput);
        chat.AppendExampleChatbotOutput("I need to fit in here! My response:");
        return await chat.GetResponseFromChatbotAsync();

    }

}