using Assignment.Api.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureWeb(builder.Configuration);
builder.Services.RegisterServices();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(WebExtensions.CorsPolicyName);
app.UseStatusCodePages();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

