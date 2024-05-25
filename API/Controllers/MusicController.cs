using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file?.Length == 0) return BadRequest("Must select a file");

        var uploadPath = Path.Combine("Uploads", file.FileName);
        var outputDirectory = Path.Combine("SeparatedTracks", Path.GetFileNameWithoutExtension(file.FileName));

        Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));
        Directory.CreateDirectory(outputDirectory);

        using (var stream = new FileStream(uploadPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"spleeter_script.py {uploadPath} {outputDirectory}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process() { StartInfo = startInfo })
        {
            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return StatusCode(500, $"Error processing file: {error}");
            }
            
            Console.WriteLine($"Result: {result}");
        }
        
        return Ok(new { message = "File uploaded and processed successfully.", downloadUrl = outputDirectory });
    }
}