using System.Diagnostics;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=MyApp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IAudioService, AudioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseRouting();
app.MapControllers();

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

app.MapPost("/api/music/upload", async (HttpRequest request) =>
    {
        var form = await request.ReadFormAsync();
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        var uploadPath = Path.Combine("Uploads", file.FileName.Replace(' ', '_'));
        var outputDir = Path.Combine("SeparatedTracks");

        Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));
        Directory.CreateDirectory(outputDir);

        await using (var stream = new FileStream(uploadPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var scriptPath = Path.Combine("scripts", "spleeter_script.py");
        var pythonPath = "/opt/anaconda3/envs/py37/bin/python3";
        var ffmpegPath = "/usr/local/bin/ffmpeg";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{scriptPath} {uploadPath} {outputDir}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Environment =
                { { "PATH", $"{Environment.GetEnvironmentVariable("PATH")}:{Path.GetDirectoryName(ffmpegPath)}" } }
        };

        string result;
        string error;
        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();
            result = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Error: {error}");
                return Results.Json(new { error = $"Error processing file: {error}" }, statusCode: 500);
            }
        }
        
        Console.WriteLine($"Result: {result}");

        var trackNames = new[] { "vocals", "drums", "bass", "other" };
        var trackUrls = trackNames.Select(name => 
            new 
                { 
                    name, 
                    url = Path.Combine("SeparatedTracks",
                                        Path.GetFileNameWithoutExtension(file.FileName.Replace(' ', '_')), $"{name}.wav") 
                })
                .ToList();

        return Results.Ok(new 
        { 
            message = "File uploaded and processed successfully.", 
            tracks = trackUrls 
        });
    })
    .WithName("UploadFile");

app.MapPost("/api/music/youtube", async (HttpRequest request) =>
    {
        var form = await request.ReadFormAsync();
        var youtubeUrl = form["url"].ToString();

        if (string.IsNullOrEmpty(youtubeUrl))
        {
            return Results.BadRequest("No YouTube URL provided.");
        }

        var outputDir = Path.Combine("Downloads", "YouTube");
        Directory.CreateDirectory(outputDir);

        var scriptPath = Path.Combine("scripts", "youtube_to_mp3.py");
        var pythonPath = "/opt/anaconda3/bin/python3";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{scriptPath} \"{youtubeUrl}\" \"{outputDir}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        string result;
        string error;
        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();
            result = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();
        
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Error: {error}");
                return Results.Json(new { error = $"Error processing YouTube URL: {error}" }, statusCode: 500);
            }
        }

        Console.WriteLine($"Result: {result}");

        // Find the downloaded MP3 file
        var directoryInfo = new DirectoryInfo(outputDir);
        var file = directoryInfo.GetFiles("*.mp3").OrderByDescending(f => f.CreationTime).FirstOrDefault();
        if (file == null)
        {
            return Results.NotFound("No MP3 file found.");
        }

        var fileUrl = $"/Downloads/YouTube/{file.Name}";

        return Results.Ok(new { message = "YouTube video converted successfully.", url = fileUrl });
    })
    .WithName("ConvertYouTubeToMp3");

app.MapPost("/api/music/upload-and-separate", async (HttpRequest request) =>
    {
        var form = await request.ReadFormAsync();
        var youtubeUrl = form["url"].ToString();

        if (string.IsNullOrEmpty(youtubeUrl))
        {
            return Results.BadRequest("No YouTube URL provided.");
        }

        var outputDir = Path.Combine("Downloads", "YouTube");
        var separateDir = Path.Combine("SeparatedTracks");
        Directory.CreateDirectory(separateDir);

        var scriptPath = Path.Combine("scripts", "youtube_to_spleeter.py");
        var pythonPath = "/opt/anaconda3/envs/py37/bin/python3";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{scriptPath} \"{youtubeUrl}\" \"{outputDir}\" \"{separateDir}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        string result;
        string error;
        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();
            result = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Error: {error}");
                return Results.Json(new { error = $"Error processing YouTube URL: {error}" }, statusCode: 500);
            }
        }

        Console.WriteLine($"Result: {result}");

        // Generate URLs for the separated tracks
        var trackNames = new[] { "vocals", "drums", "bass", "other" };
        var baseOutputPath =
            Path.Combine("SeparatedTracks", Path.GetFileNameWithoutExtension(result).Replace(".mp3", ""));
        var trackUrls = trackNames.Select(name =>
            new { name, url = $"/SeparatedTracks/{Path.GetFileNameWithoutExtension(result)}/{name}.wav" }).ToList();

        return Results.Ok(new
        {
            message = "YouTube video converted and processed successfully.",
            tracks = trackUrls
        });
    })
    .WithName("ConvertYouTubeToMp3AndSeparate");

app.Run();