using OpenAI.GPT3.Extensions;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOpenAIService();

var app = builder.Build();
var openAiService = app.Services.GetRequiredService<IOpenAIService>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("/poem", async () =>
{
    var completionResult = await openAiService.Completions.CreateCompletion(new()
    {
        Prompt = "Once upon a time",
        Model = Models.TextDavinciV3
    });

    if (completionResult.Successful)
        return Results.Text(completionResult.Choices.FirstOrDefault()?.Text);
    
    if (completionResult.Error == null)
        throw new Exception("Unknown Error");

    return Results.Problem($"{completionResult.Error.Code}: {completionResult.Error.Message}");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
    