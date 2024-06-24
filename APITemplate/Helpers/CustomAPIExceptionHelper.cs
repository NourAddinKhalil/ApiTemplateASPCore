namespace APITemplate.Helpers
{
    public class CustomAPIExceptionHelper : Exception
    {
        public new string? Message { get; private set; }
        public int StatusCode { get; private set; } = 500;

        public CustomAPIExceptionHelper(string message, int statusCode)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
