namespace Dummy.Infrastructure.Services.EmailService
{
    public class EmailContent
    {
        public string To { get; set; }
        public object Data { get; set; }
        public string EventType { get; set; }
    }
}
