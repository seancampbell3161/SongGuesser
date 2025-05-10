using System.Diagnostics;
using System.Text.Json;
using API.DTOs;
using API.DTOs.Constants;
using API.Interfaces;
using FFMpegCore;

namespace API.Services;

public class AudioService(IWaveformService waveformService) : IAudioService
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
        using (var process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
            result = await process.StandardOutput.ReadToEndAsync();
            error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

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
        var filePath = result
            .Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault()?
            .Trim();
        var fileName = filePath?.Replace("Downloads/YouTube/", "") ?? "";

        if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName))
            return new ConvertResult()
            {
                Error = $"No MP3 file found"
            };
        
        var directoryInfo = new DirectoryInfo(outputDir);
        var file = directoryInfo.GetFiles(fileName).FirstOrDefault();
        if (file == null)
        {
            return new ConvertResult()
            {
                Error = $"No MP3 file found"
            };
        }

        return new ConvertResult()
        {
            Message = "Successfully converted video to MP3",
            FilePath = filePath
        };

    }

    public async Task<SeparateResult> SeparateTracksAsync(IFormFile file)
    {
        var uploadPath = Path.Combine("Uploads", file.FileName.Replace(' ', '_'));
        var outputDir = Path.Combine("SeparatedTracks");

        Directory.CreateDirectory(Path.GetDirectoryName(uploadPath) ?? Guid.NewGuid().ToString());
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
        using (var process = new Process())
        {
            process.StartInfo = startInfo;
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
                new TrackDto
                {
                    Name = name,
                    Path = Path.Combine("SeparatedTracks",
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

        try
        {
            if (convertedMp3.FilePath.StartsWith('/'))
            {
                convertedMp3.FilePath = convertedMp3.FilePath[1..];
            }

            var filePath = convertedMp3.FilePath;
            var fileName = convertedMp3.FilePath.Split("/").LastOrDefault() ?? string.Empty;

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

            string result;
            string error;
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                result = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

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
                    new TrackDto
                    {
                        Name = name,
                        Path = Path.Combine("/SeparatedTracks",
                            Path.GetFileNameWithoutExtension(fileName), $"{name}.wav")
                    })
                .ToList();

            return new SeparateResult()
            {
                Message = "File uploaded and processed successfully",
                Tracks = trackUrls
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new SeparateResult
            {
                Error = ex.Message
            };
        }
    }

    public async Task<SeparateResult> ConvertAndSeparateTracksAsync(string url)
    {
        var convertedResult = await ConvertToMp3Async(url);

        if (convertedResult.Error != null)
        {
            return new SeparateResult()
            {
                Error = convertedResult.Error
            };
        }
            
        var separatedTracksResult = await SeparateTracksAsync(convertedResult);

        if (separatedTracksResult.Error != null) { return new SeparateResult { Error = separatedTracksResult.Error }; }
        
        // await waveformService.CreateWaveforms(separatedTracksResult);
        
        SeparateResult finalResult = await ConvertAudioFormat(separatedTracksResult.Tracks, AudioFormats.Mp3);
        
        return finalResult;
    }

    private async Task<SeparateResult> ConvertAudioFormat(List<TrackDto> tracks, string format)
    {
        var result = new SeparateResult();

        foreach (var track in tracks)
        {
            var path = $"{track.Path}".Replace(Path.GetExtension(track.Path), $".{format}");
            
            try
            {
                var oldPath = track.Path.Remove(0, 1);
                var isConverted = await FFMpegArguments.FromFileInput(oldPath)
                    .OutputToFile($"{path}".Remove(0, 1))
                    .ProcessAsynchronously();

                if (!isConverted)
                {
                    result.Error = $"Error processing file: {track.Path}";
                    return result;
                }
                
                File.Delete(oldPath);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
            }
            
            result.Tracks.Add(new TrackDto
            {
                Name = track.Name,
                Path = path
            });
        }

        return result;
    }
}