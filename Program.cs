
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using StreamApi.Options;
using StreamApi.Models;
using Models;
using Services;

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

// MVC / Swagger / CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stream API", Version = "v1" });
    // простая схема для визуала
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "Bearer {token}"
    });
});
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseRouting();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.MapControllers();

app.Run();
