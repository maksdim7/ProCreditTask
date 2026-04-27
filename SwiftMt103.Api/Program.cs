using Microsoft.AspNetCore.Mvc;
using SwiftMt103.Api.Services;
using SwiftMt103.Api.Data;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddSingleton<Mt103Parser>();
builder.Services.AddSingleton<Database>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var db = app.Services.GetRequiredService<Database>();
db.Initialize();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapPost("/api/swift-messages/upload", async ([FromForm] IFormFile file, Mt103Parser parser, Database db) =>
{
    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }

    using var reader = new StreamReader(file.OpenReadStream());
    var content = await reader.ReadToEndAsync();

    logger.Info($"File uploaded: {file.FileName}");

    var parsed = parser.Parse(content);

    parsed.TryGetValue("20", out var refNum);

    logger.Info($"Parsed transaction reference: {refNum}");

    parsed.TryGetValue("32A", out var field32A);
    parsed.TryGetValue("50K", out var ordering);
    parsed.TryGetValue("59", out var beneficiary);

    string currency = "";
    string amount = "";

    if (!string.IsNullOrEmpty(field32A) && field32A.Length > 9)
    {
        currency = field32A.Substring(6, 3);
        amount = field32A.Substring(9);
    }

    db.Insert(refNum, currency, amount, ordering, beneficiary, content);

    logger.Info("Message saved to database");

    return Results.Ok(new
    {
        Message = "Saved successfully",
        ParsedFields = parsed
    });
})
.Accepts<IFormFile>("multipart/form-data")
.DisableAntiforgery();

app.MapGet("/api/swift-messages", (Database db) =>
{
    var data = db.GetAll();
    return Results.Ok(data);
})
.WithName("GetAllMessages")
.WithOpenApi();


app.Run();


