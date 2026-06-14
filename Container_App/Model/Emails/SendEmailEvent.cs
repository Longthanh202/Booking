namespace Container_App.Model.Emails
{
    public class SendEmailEvent
    {
        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
