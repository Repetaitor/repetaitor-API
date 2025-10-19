using System.Net.Http.Json;
using System.Text;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.QuizModels;
using Core.Application.Models.RequestsDTO.Assignments;
using Core.Application.Models.ReturnViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIService;

public class AICommunicateService(IConfiguration configuration) : IAICommunicateService
{
    private readonly string? _apiKey = configuration["GeminiApiKey"];

    private readonly string? _endpoint = configuration["AIEndpoint"];
    private const char FirstDelimiter = '%';
    private const char SecondDelimiter = '$';
    private const char ThirdDelimiter = '&';
    public async Task<(EvaluateAssignmentRequest?, QuizViewModel)> GetAIResponse(string essayTitle, string essayText, int expectedWordCount)
    
    {
        string content;
        await using (var fstream = File.OpenRead(Directory.GetCurrentDirectory() + "/message.txt"))
        {
            var buffer = new byte[fstream.Length];
            var readAsync = await fstream.ReadAsync(buffer, 0, buffer.Length);
            if (readAsync == 0) return (null, null!);
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

        var response = await client.PostAsJsonAsync(_endpoint + _apiKey, requestBody);
        if (response.IsSuccessStatusCode)
        {
            content = await response.Content.ReadAsStringAsync();
        }

        var doc = JObject.Parse(content)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()
            .Replace("```json\n", "")
            .Replace("\n```", "");
        var convert = JsonConvert.DeserializeObject<AIReturnViewModel>(doc!);
        if (convert == null) return (null, null!);
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
        var suggSkills = new List<string>();
        if (convert.DetailedGrammarScore.Vocabulary < 80)
        {
            suggSkills.Add("Vocabulary ");
        }

        if (convert.DetailedGrammarScore.SpellingAndPunctuation < 80)
        {
            suggSkills.Add("Spelling and punctuation");
        }

        if (convert.DetailedGrammarScore.Grammar < 80)
        {
            suggSkills.Add("Grammar");
        }

        var quiz = await GetQuizQuestions(suggSkills , "/QuizCreate.txt");
        return (resp, quiz);
    }

    public async Task<QuizViewModel?> GetQuizQuestions(List<string> questionTypes, string promptPath)
    {
        string content;
        await using (var fstream = File.OpenRead(Directory.GetCurrentDirectory() + promptPath))
        {
            var buffer = new byte[fstream.Length];
            var readAsync = await fstream.ReadAsync(buffer, 0, buffer.Length);
            if (readAsync == 0) return null;
            content = Encoding.Default.GetString(buffer).Replace("SkillsNameEnter", string.Join(", ", questionTypes));
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

        var response = await client.PostAsJsonAsync(_endpoint + _apiKey, requestBody);
        if (response.IsSuccessStatusCode)
        {
            content = await response.Content.ReadAsStringAsync();
        }
        var doc = JObject.Parse(content)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()
            .Replace("```json\n", "")
            .Replace("\n```", "");
        var convert = JsonConvert.DeserializeObject<QuizViewModel>(doc!);
        return convert;
    }

    public async Task<string> GetEssayTitle()
    {
        string str;
        await using (var fstream = File.OpenRead(Directory.GetCurrentDirectory() + "/GenEssayTitle.txt"))
        {
            var buffer = new byte[fstream.Length];
            var readAsync = await fstream.ReadAsync(buffer, 0, buffer.Length);
            if (readAsync == 0) return "";
            str = Encoding.Default.GetString(buffer);
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
                        new { text = str }
                    }
                }
            }
        };
        var response = await client.PostAsJsonAsync(_endpoint + _apiKey, requestBody);
        var content = "";
        if (response.IsSuccessStatusCode)
        {
            content = await response.Content.ReadAsStringAsync();
        }

        return JObject.Parse(content)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";
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
    public async Task<string> GetEssayTextFromImage(List<string> images)
    {
        var fullText = string.Empty;
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(200);
        foreach (var imgBase64 in images)
        {
            var typeMime = imgBase64.Split(',')[0].Split(':')[1].Split(';')[0];
            var base64Image = imgBase64.Split(',')[1];
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = "Extract only the raw text from the image. Do not add any comments, explanations, headings, or introductions. Return the text exactly as it appears." },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = typeMime,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var response = await client.PostAsJsonAsync(_endpoint + _apiKey, requestBody);
            var content = "";
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            var result = JObject.Parse(content)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            fullText += result ?? string.Empty;
        }
        return fullText;
    }
}