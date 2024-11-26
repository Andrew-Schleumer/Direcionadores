namespace Placemarks.Exceptions
{
    public class FilteringException : Exception
    {
        public FilteringException() { }

        public FilteringException(string message) : base(message) { }

        public FilteringException(string message, Exception inner) : base(message, inner) { }
    }
}
