using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChatSignalR.DataAccess;
using WebChatSignalR.Models;

namespace WebChatSignalR.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _appDbContext;

        public ChatHub(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SendMessage(string user, string message) 
        {
            await SaveMessageDb(user,message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SaveMessageDb(string user, string message)
        {
            //if (user == null && message == null)
            //{
                
            //}
            var msg = new Message
            {
                User = user,
                Content = message,
                Timestamp = DateTime.Now
            };
            _appDbContext.Messages.Add(msg);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task LoadMessages() 
        {
            var messages = _appDbContext.Messages
                .OrderBy(x => x.Timestamp)
                .Select(x => new {x.User, x.Content})
                .ToList();

            await Clients.Caller.SendAsync("LoadMessages", messages);
        }
    }
}