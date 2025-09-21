
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using StreamApi.Options;
using StreamApi.Models;
using Models;
using Services;

// Relax Npgsql timestamp behavior and ensure predictable UTC handling
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddScoped<Services.IReportService, Services.ReportService>();

// Options
builder.Services.Configure<HlsOptions>(builder.Configuration.GetSection("Hls"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<PublishKeys>(builder.Configuration.GetSection("PublishKeys"));

// EF Core
builder.Services.AddDbContext<Models.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<Services.IAdService, Services.AdService>();
builder.Services.AddScoped<Services.IStreamService, Services.StreamService>();
builder.Services.AddScoped<Services.IUserService, Services.UserService>();
builder.Services.AddScoped<Services.IMatchService, Services.MatchService>();
builder.Services.AddScoped<Services.HlsFileSystemStreamService>();
builder.Services.AddSingleton<IOnlineTracker, OnlineTracker>();
builder.Services.AddSignalR(o =>
{
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);   // сервер шлёт ping каждые 15с
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // если 60с нет пингов от клиента — разрыв
    o.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

// MVC / Swagger / CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stream API",
        Version = "v1",
        Description = "API для управления спортивными трансляциями"
    });

    // Настройки для nullable типов
    c.SupportNonNullableReferenceTypes();

    // Простая схема для визуала
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "Bearer {token}",
        Scheme = "Bearer"
    });
});
builder.Services.AddSingleton<IOnlineTracker, OnlineTracker>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Добавляем обработку исключений
app.UseExceptionHandler("/error");

// Настройка статических файлов для изображений
app.UseStaticFiles();

app.UseCors();
app.UseRouting();

try
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stream API v1");
        c.RoutePrefix = "swagger";
    });
}
catch (Exception ex)
{
    Console.WriteLine($"Swagger error: {ex.Message}");
}
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(10)
});
app.MapHub<OnlineHub>("/hub/online");


app.MapControllers();

// Добавляем endpoint для обработки ошибок
app.Map("/error", () => "An error occurred");

app.Run();
