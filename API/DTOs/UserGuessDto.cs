namespace API.DTOs;

public record UserGuessDto(string Guess, int SongId, string UserId = "");