using System.Net.Http.Json;
using System.Text;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIService;

public class AICommunicateService : IAICommunicateService
{
    private readonly string apiKey = "AIzaSyBVdAnFMvH9fXt9E5uiRYg3EmeW18GHMR0";

    private readonly string endpoint =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro-exp-03-25:generateContent?key=";

    public async Task<AIReturnViewModel?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount)
    
    {
        var content = "";
        await using (FileStream fstream = File.OpenRead(@"C:\Users\dachi\RiderProjects\repetaitor-API\Infrastructure.AIService\message.txt"))
        {
            var buffer = new byte[fstream.Length];
            await fstream.ReadAsync(buffer, 0, buffer.Length);
            content = Encoding.Default.GetString(buffer).Replace("essayTitleValue", essayTitle).Replace("expectedWordCountValue", expectedWordCount.ToString())
                .Replace("essayTextValue", essayText);
        }
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(200);
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = content }
                    }
                }
            }
        };

        var response = await client.PostAsJsonAsync(endpoint + apiKey, requestBody);
        if (response.IsSuccessStatusCode)
        {
            content = await response.Content.ReadAsStringAsync();
        }

        var doc = JObject.Parse(content)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()
            .Replace("```json\n", "")
            .Replace("\n```", "");
        return JsonConvert.DeserializeObject<AIReturnViewModel>(doc!);
    }
}