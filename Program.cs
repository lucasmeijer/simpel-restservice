using Microsoft.ApplicationInsights.DataContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
//}

Secrets.Intialize(builder.Configuration);

var openAISummarizer = new OpenAISummarizer();
var azureOCR = new AzureFormRecognizer();

app.MapPost("/summarize", async (HttpContext context, IFormFile file) =>
{
    var ocrResult = await azureOCR.ImageToText(file.OpenReadStream());
    
    // Write request body to App Insights
    var requestTelemetry = context.Features.Get<RequestTelemetry>();                              
    requestTelemetry?.Properties.Add("OCRResult", ocrResult);
    
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
    