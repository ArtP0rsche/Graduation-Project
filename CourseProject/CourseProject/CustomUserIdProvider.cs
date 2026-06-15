namespace CourseProject
{
    public class CustomUserIdProvider : Microsoft.AspNetCore.SignalR.IUserIdProvider
    {
        public string? GetUserId(Microsoft.AspNetCore.SignalR.HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value;
        }
    }
}
