namespace AppServer.Common
{
    public class Response
    {
        public bool Status { get; set; }

        public List<Object>? Data { get; set; }

        public object Message { get; set; }
    }
}