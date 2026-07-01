namespace TopUpSimulation.Framework.Common.Exceptions
{
    public class TopUpException : Exception
    {
        public string Code
        {
            get;
            private set;
        }
        public TopUpResultException Error { get; private set; }
        public TopUpException()
        {
        }

        public TopUpException(string msg)
            : base(msg)
        {
            Error = new TopUpResultException("200",msg);
        }
        public TopUpException(TopUpResultException exception) : base(exception.message)
        {
            Code = exception.code;
            Error = exception;
        }

        public TopUpException(string message, string code = "200", Exception innerException = null)
            : base(message, innerException)
        {
            Code = code;
            Error = new TopUpResultException(code, message);
        }

        public TopUpException(string message, Exception innerException)
            : this(message, "200", innerException)
        {
        }
    }
}