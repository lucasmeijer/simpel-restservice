using System.Text;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

public class AzureFormRecognizer
{
    private readonly DocumentAnalysisClient _client;

    public AzureFormRecognizer()
    {
        _client = new(
            new(Secrets.Get("AzureFormRecognizer__Endpoint")),
            new AzureKeyCredential(Secrets.Get("AzureFormRecognizer__Key"))
            );
    }
    
    public async Task<string> ImageToText(Stream imageStream)
    {
        if (!imageStream.CanSeek)
        {
            var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            ms.Position = 0;
            imageStream = ms;
        }
        
        var analysisOperation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", imageStream);
        var analysis = analysisOperation.Value;

        var sb = new StringBuilder();
        foreach (var paragraph in analysis.Paragraphs)
        {
            sb.AppendLine($"===PARAGRAPH:");
            sb.AppendLine(paragraph.Content);
        }

        return sb.ToString();
    }
}