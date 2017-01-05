namespace Aem
{
    public class Arguments
    {
        public Arguments(Command command, string message, bool urlEncode = false, bool urlDecode = false)
        {
            Command = command;
            Message = message;
            UrlEncode = urlEncode;
            UrlDecode = urlDecode;
        }

        public Command Command { get; }

        public string Message { get; set; }

        public bool UrlEncode { get; }

        public bool UrlDecode { get; }
    }
}
