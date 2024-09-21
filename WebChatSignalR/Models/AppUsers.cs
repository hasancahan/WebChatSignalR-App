using System.ComponentModel.DataAnnotations;

namespace WebChatSignalR.Models
{
    public class AppUsers
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime FirstRegisterDate { get; set; } = DateTime.Now;

    }
}
