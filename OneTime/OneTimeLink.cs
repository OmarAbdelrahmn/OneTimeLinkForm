namespace OneTime;

public class OneTimeLink
{
    public int Id { get; set; } 
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; set; }
}
