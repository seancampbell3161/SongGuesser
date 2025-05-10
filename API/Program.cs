using API.Data.Repositories;
using API.Extensions;
using API.Interfaces;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IAudioService, AudioService>();
builder.Services.AddScoped<IWaveformService, WaveformService>();
builder.Services.AddScoped<IUserScoreRepository, UserScoreRepository>();
builder.Services.AddScoped<ISongRepository, SongRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "SeparatedTracks")),
    RequestPath = "/SeparatedTracks"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Uploads"
});

var downloadPath = Path.Combine(builder.Environment.ContentRootPath, "Downloads");
if (!Directory.Exists(downloadPath))
{
    Directory.CreateDirectory(downloadPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Downloads")),
    RequestPath = "/Downloads"
});

// debug log middleware
// app.Use(async (context, next) =>
// {
//     Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
//     await next();
//     Console.WriteLine($"Response: {context.Response.StatusCode}");
// });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();