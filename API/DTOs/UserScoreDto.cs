namespace API.DTOs;

public class UserScoreDto
{
    public string UserName { get; set; } = "";
    public int TotalScore { get; set; }
    public int NumOfGames { get; set; }
}