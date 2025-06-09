using System.Net.Http.Json;
using System.Text;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Assignments;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIService;

public class AICommunicateService : IAICommunicateService
{
    private readonly string apiKey = "AIzaSyDKaIkRaedr7v-K69fm1W5eD2KA-nmbvlE";

    private readonly string endpoint =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro-preview-03-25:generateContent?key=";
    private const char FirstDelimiter = '%';
    private const char SecondDelimiter = '$';
    private const char ThirdDelimiter = '&';
    public async Task<EvaluateAssignmentRequest?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount)
    
    {
        var content = "";
        await using (FileStream fstream = File.OpenRead(Directory.GetCurrentDirectory() + "/message.txt"))
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
        var convert = JsonConvert.DeserializeObject<AIReturnViewModel>(doc!);
        if (convert == null) return null;
        var resp = new EvaluateAssignmentRequest
        {
            FluencyScore = convert.FluencyScore,
            GrammarScore = convert.GrammarScore,
            GeneralComments = convert.GeneralComments,
            EvaluationTextComments = []
        };
        var actualIndex = 0;
        for(var i = 0; i < convert.MarkedEssayText.Length; i++)
        {
           if(convert.MarkedEssayText[i] == FirstDelimiter)
           {
               var startIndex = actualIndex;
               var str = new StringBuilder();
               i++;
               for(; convert.MarkedEssayText[i] != FirstDelimiter; i++)
               {
                   str.Append(convert.MarkedEssayText[i]);
               }
               var (statusId, comment, textLength) = CustomParser(str.ToString());
               resp.EvaluationTextComments.Add(new EvaluationTextCommentModal()
               {
                   StatusId = statusId,
                   Comment = comment,
                   StartIndex = startIndex,
                   EndIndex = startIndex + textLength,
               });
               actualIndex += textLength;
           }
           else
           {
               actualIndex++;
           }
        }
        return resp;
    }

    private (int, string, int) CustomParser(string txt)
    {
        var actualTxtLength = 0;
        var commentCont = "";
        for(var i = 0; i < txt.Length; i++)
        {
            if(txt[i] == SecondDelimiter)
            {
                i++;
                for(;txt[i] != SecondDelimiter; i++) commentCont += txt[i]; 
                
            } else actualTxtLength++;
        }
        var (statusId, comment) =  ParseCommentSide(commentCont); 
        return (statusId, comment, actualTxtLength);
    }
    private (int, string) ParseCommentSide(string txt)
    {
        var statusId = 0;
        var comment = "";
        for(var i = 0; i < txt.Length; i++)
        {
            if(txt[i] == ThirdDelimiter)
            {
                var statId = "";
                i++;
                for(;txt[i] != ThirdDelimiter; i++) statId += txt[i];
                statusId = int.Parse(statId);

            } else comment+=txt[i];
        }
        return (statusId, comment);
    }
}