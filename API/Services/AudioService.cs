using System.Diagnostics;
using API.DTOs;
using API.Interfaces;

namespace API.Services;

public class AudioService : IAudioService
{
    public async Task<ConvertResult> ConvertToMp3Async(string url)
    {
        var outputDir = Path.Combine("Downloads", "YouTube");
        Directory.CreateDirectory(outputDir);

        var scriptPath = Path.Combine("scripts", "youtube_to_mp3.py");
        var pythonPath = "/opt/anaconda3/bin/python3";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{scriptPath} \"{url}\" \"{outputDir}\"",
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
                return new ConvertResult()
                {
                    Error = $"Error processing YouTube URL: {error}"
                };
            }
        }

        Console.WriteLine($"Result: {result}");

        // Find the downloaded MP3 file
        var directoryInfo = new DirectoryInfo(outputDir);
        var file = directoryInfo.GetFiles("*.mp3").OrderByDescending(f => f.CreationTime).FirstOrDefault();
        if (file == null)
        {
            return new ConvertResult()
            {
                Error = $"No MP3 file found"
            };
        }

        var fileUrl = $"/Downloads/YouTube/{file.Name}";

        return new ConvertResult()
        {
            Message = "Successfully converted video to MP3",
            FilePath = fileUrl
        };
    }

    public async Task<SeparateResult> SeparateTracksAsync(IFormFile file)
    {
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
                return new SeparateResult()
                {
                    Error = $"Error processing file: {error}"
                };
            }
        }
        
        Console.WriteLine($"Result: {result}");

        var trackNames = new[] { "vocals", "drums", "bass", "other" };
        var trackUrls = trackNames.Select(name => 
            new Track
                { 
                    Name = name, 
                    Url = Path.Combine("SeparatedTracks",
                                        Path.GetFileNameWithoutExtension(file.FileName.Replace(' ', '_')), $"{name}.wav") 
                })
                .ToList();

        return new SeparateResult()
        {
            Message = "File uploaded and processed successfully",
            Tracks = trackUrls
        };
    }

    public async Task<SeparateResult> SeparateTracksAsync(ConvertResult convertedMp3)
    {
        if (convertedMp3?.FilePath == null)
        {
            return new SeparateResult() { Error = "URL cannot be null" };
        }

        if (convertedMp3.FilePath.StartsWith("/"))
        {
            convertedMp3.FilePath = convertedMp3.FilePath.Substring(1);
        }

        var filePath = convertedMp3.FilePath;
        var fileName = convertedMp3.FilePath.Split("/").LastOrDefault();
        
        var outputDir = Path.Combine("SeparatedTracks");
        
        Directory.CreateDirectory(outputDir);

        var scriptPath = Path.Combine("scripts", "spleeter_script.py");
        var pythonPath = "/opt/anaconda3/envs/py37/bin/python3";
        var ffmpegPath = "/usr/local/bin/ffmpeg";

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"{scriptPath} {filePath} {outputDir}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Environment =
                { { "PATH", $"{Environment.GetEnvironmentVariable("PATH")}:{Path.GetDirectoryName(ffmpegPath)}" } }
        };
        
        // wait for mp3 file to download
        // const int maxRetries = 20;
        // const int delayMilliseconds = 3000;
        // int retryCount = 0;
        //
        // while (!File.Exists(convertedMp3.FilePath) && retryCount < maxRetries)
        // {
        //     await Task.Delay(delayMilliseconds);
        //     retryCount++;
        // }

        // if (!File.Exists(convertedMp3.FilePath))
        // {
        //     return new SeparateResult() { Error = "MP3 file not found after waiting." };
        // }

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
                return new SeparateResult()
                {
                    Error = $"Error processing file: {error}"
                };
            }
        }
        
        Console.WriteLine($"Result: {result}");

        var trackNames = new[] { "vocals", "drums", "bass", "other" };
        var trackUrls = trackNames.Select(name => 
            new Track
                { 
                    Name = name, 
                    Url = Path.Combine("/SeparatedTracks",
                                        Path.GetFileNameWithoutExtension(fileName), $"{name}.wav")
                })
                .ToList();

        return new SeparateResult()
        {
            Message = "File uploaded and processed successfully",
            Tracks = trackUrls
        };
    }

    public async Task<SeparateResult> ConvertAndSeparateTracksAsync(string url)
    {
        var convertedResult = await ConvertToMp3Async(url);

        if (convertedResult.Error == null)
        {
            var separatedTracksResult = await SeparateTracksAsync(convertedResult);
            return separatedTracksResult;
        }

        return new SeparateResult()
        {
            Error = convertedResult.Error
        };
    }
}