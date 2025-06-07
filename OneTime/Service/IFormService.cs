namespace OneTime.Service;

public interface IFormService
{
    Task<string> GenerateOneTimeLinkAsync();
    Task<bool> ValidateTokenAsync(string token); // NEW: Just validate without consuming
    Task<bool> ValidateAndUseTokenAsync(string token); // EXISTING: Consume the token
    Task SaveFormResponseAsync(FormResponse response);
    Task<List<FormResponse>> GetAllResponsesAsync();
    Task<byte[]> GenerateExcelReportAsync();
    Task CleanupExpiredLinksAsync();
}