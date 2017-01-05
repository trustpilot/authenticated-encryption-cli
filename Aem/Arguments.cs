namespace Aem
{
    public class Arguments
    {
        public Arguments(Command command, string message, bool urlEncode = false)
        {
            Command = command;
            Message = message;
            UrlEncode = urlEncode;
        }

        public Command Command { get; }

        public string Message { get; set; }

        public bool UrlEncode { get; }
    }
}
