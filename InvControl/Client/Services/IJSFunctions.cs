namespace InvControl.Client.Services
{
    public interface IJSFunctions
    {
        Task downloadFileFromStream(string fileName, byte[] data);
    }
}
