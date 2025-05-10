using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class WaveformData
{
    public int Id { get; set; }
    public string? Data { get; set; }
    public Track? Track { get; set; }
}