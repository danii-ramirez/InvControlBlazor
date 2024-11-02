namespace InvControl.Client.Helpers
{
    public class Response
    {
        public Response() { }

        public Response(bool success)
        {
            Success = success;
        }

        public Response(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public Response(bool success, Dictionary<string, List<string>> errors)
        {
            Success = success;
            Errors = errors;
        }

        public Response(bool success, string message, Dictionary<string, List<string>> errors) : this(success)
        {
            Message = message;
            Errors = errors;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
