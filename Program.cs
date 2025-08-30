using Microsoft.OpenApi.Models;
using StreamApi.Options;
using StreamApi.Services;
using StreamApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.Configure<HlsOptions>(builder.Configuration.GetSection("Hls"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<PublishKeys>(builder.Configuration.GetSection("PublishKeys"));

// Services
builder.Services.AddScoped<IStreamService, HlsFileSystemStreamService>();

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
builder.Services.Configure<PublishKeys>(builder.Configuration.GetSection("PublishKeys"));

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
