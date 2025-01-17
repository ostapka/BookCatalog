namespace BookCatalog.Server.AppCore.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public NotFoundException(string message, ErrorCode errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
