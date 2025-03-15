using System.Diagnostics;
using API.DTOs;
using API.Interfaces;

namespace API.Services;

public class WaveformService : IWaveformService
{
    public async Task CreateWaveforms(SeparateResult separatedTracksResult)
    {
        var scriptPath = Path.Combine("scripts", "generate_waveform.py");
        var pythonPath = "/opt/anaconda3/bin/python3";

        try
        {
            foreach (var track in separatedTracksResult.Tracks)
            {
                var trackPath = "/Users/seancampbell/Documents/source/repos/SongGuesser/API" + track.Path;
                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"{scriptPath} \"{trackPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(startInfo);
                var output = await process?.StandardOutput.ReadToEndAsync()!;
                var error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }

                if (string.IsNullOrWhiteSpace(output)) continue;

                // track.WaveformData = JsonSerializer.Deserialize<List<WaveformData>>(output)!;
                track.WaveformData = output;

                // var path = Path.Combine(Path.GetPathRoot(track.Path) ?? "SeparatedTracks", "waveforms",
                //     Path.GetFileNameWithoutExtension(track.Path) + ".json");
                //
                // await File.WriteAllTextAsync(path, output);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}