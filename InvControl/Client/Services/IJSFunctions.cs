namespace InvControl.Client.Services
{
    public interface IJSFunctions
    {
        Task DownloadFile(string fileName, byte[] data);
    }
}
