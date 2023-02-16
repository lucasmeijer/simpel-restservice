using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;

public class OpenAISummarizer
{
    readonly IOpenAIService _service = new OpenAIService(new()
    {
        ApiKey = Secrets.Get("OpenAIServiceOptions__ApiKey")
    });

    public async Task<string> Summarize(string ocrResult)
    {
        var completionResult = await _service.Completions.CreateCompletion(new()
        {
            Prompt = ocrResult + @"=================================
Summarize this essence of this letter into different sentences.
Each sentence should be:
- dutch, at A2/1F level
- informal
- address the reader
- friendly

return JSON format with keys for:
'sender',  short name of the sender
'call_to_action',
'call_to_action_is_call': indicates if the call to action is to make a phonecall
'phone_number',  the phonenumber should not have any additional punctuation or characters
“summary_sentences” with as subfields 'emoji' and 'text'.

Make sure you return a valid JSON syntax.",
            Model = Models.TextDavinciV3,
            MaxTokens = 2000
        });

        if (!completionResult.Successful)
            throw new SummarizeException("GTP3 summarize prompt was unsuccessful. "+completionResult.Error?.Message);
        
        return completionResult.Choices.FirstOrDefault()!.Text;
    }
}

class SummarizeException : Exception
{ 
    public SummarizeException(string message) : base(message)
    {
    }
}