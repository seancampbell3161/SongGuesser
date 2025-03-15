using API.DTOs;

namespace API.Interfaces;

public interface IWaveformService
{
    Task CreateWaveforms(SeparateResult separatedTracksResult);
}