namespace InvControl.Client.Helpers
{
    public class Estado
    {
        public Estado(bool? value, string description)
        {
            Value = value;
            Description = description;
        }

        public bool? Value { get; set; }
        public string Description { get; set; }
    }
}
