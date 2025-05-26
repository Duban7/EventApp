namespace Services.Exeptions
{
    public class InternalErrorException : Exception
    {
        public InternalErrorException(string message) : base(message) { }
    }
}
