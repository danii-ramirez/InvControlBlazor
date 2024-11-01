namespace InvControl.Shared.Helpers
{
    internal static class ExpresionesRegulares
    {
        internal const string SOLO_LETRAS = @"^[a-zA-Z]+$";
        internal const string LETRAS_CE = @"^[\p{L}]+$";
        internal const string LETRAS_ESPACIOS_CE = @"^[\p{L}\s]+$";
    }
}
