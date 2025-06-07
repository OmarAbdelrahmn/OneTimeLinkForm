namespace OneTime;

public class FormResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TUserName { get; set; } = string.Empty;
    public bool IsWorking { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.Now;

}
