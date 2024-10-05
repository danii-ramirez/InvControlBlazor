namespace InvControl.Shared.Models
{
    public class LoginUserResponse
    {
        public LoginUserResponse() { }
        public LoginUserResponse(Status status) => Status = status;
        public LoginUserResponse(Status status, Dictionary<string, List<string>> errors) => (Status, Errors) = (status, errors);

        public int IdUsuario { get; set; }
        public Status Status { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }

    public enum Status
    {
        Success,
        Failed,
        Blocked,
        ResetPassword
    }
}
