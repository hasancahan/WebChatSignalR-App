using System.ComponentModel.DataAnnotations;

namespace WebChatSignalR.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
