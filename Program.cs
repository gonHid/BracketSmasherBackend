using BracketSmasherBackend.Data;
using BracketSmasherBackend.Hubs;
using BracketSmasherBackend.Services;
using Microsoft.EntityFrameworkCore;
using Resend;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin() // En desarrollo es aceptable, luego restringe a tu IP
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Add services to the container.
builder.Services.AddHttpClient<StartGgService>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://api.start.gg/gql/alpha");
    });
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    ));
builder.Services.AddResend(options =>
{
    options.ApiToken = builder.Configuration["RESEND_API_KEY"];
});
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MatchService>();
builder.Services.AddHostedService<CleanupService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpClient<StartGgService>();

var app = builder.Build();

app.UseCors("AllowMobileApp");
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<MatchHub>("/matchHub");
app.MapGet("/health", () => Results.Ok("OK"));
app.MapGet("/test-email", async (EmailService email) =>
{
    try
    {
        await email.SendEmailAsync(
            "gon.hidalgo.ch@gmail.com",
            "Prueba",
            "Hola desde Render"
        );

        return Results.Ok("Correo enviado");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.ToString());
    }
});
app.Run();
