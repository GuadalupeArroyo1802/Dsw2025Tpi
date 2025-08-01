namespace Dsw2025Tpi.Application.Exceptions
{

    public class PriceNullException : Exception
    {
        public PriceNullException()
        {
        }

        public PriceNullException(string? message) : base(message)
        {
        }

        public PriceNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}