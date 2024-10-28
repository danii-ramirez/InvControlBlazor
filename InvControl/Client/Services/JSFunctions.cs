using Microsoft.JSInterop;

namespace InvControl.Client.Services
{
    public class JSFunctions : IJSFunctions
    {
        private readonly IJSRuntime js;

        public JSFunctions(IJSRuntime js) => this.js = js;

        public async Task downloadFileFromStream(string fileName, byte[] data)
        {
            var fileStream = new MemoryStream(data);
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
    }
}
