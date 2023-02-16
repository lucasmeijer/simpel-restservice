var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
app.UseDeveloperExceptionPage();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
//}

Secrets.Intialize(builder.Configuration);

var openAISummarizer = new OpenAISummarizer();
var azureOCR = new AzureFormRecognizer();

app.MapPost("/summarize", async (HttpRequest request) =>
{
    var ocrResult = await azureOCR.ImageToText(request.Body);
    var summary = await openAISummarizer.Summarize(ocrResult);
    return Results.Text(summary);
});

app.MapGet("/exception", () =>
{
    throw new ArgumentException("EXCEPTION_TEST_LUCAS");
});

app.MapGet("/notfound", () =>
{
    return Results.NotFound();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
    