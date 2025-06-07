namespace OneTime.Service;

public interface IFormService
{
    Task<string> GenerateOneTimeLinkAsync();
    Task<bool> ValidateAndUseTokenAsync(string token);
    Task SaveFormResponseAsync(FormResponse response);
    Task<List<FormResponse>> GetAllResponsesAsync();
    Task<byte[]> GenerateExcelReportAsync();
    Task CleanupExpiredLinksAsync();

}
