using Microsoft.AspNetCore.SignalR;

namespace WebChatSignalR.SignalRHub
{
    public class VideoChatHub : Hub
    {

        public async Task SendSessionDescription(string targetConnectionId, string sdp)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSessionDescription", Context.ConnectionId, sdp);
        }

        // Method to handle incoming ICE candidates
        public async Task SendIceCandidate(string targetConnectionId, string candidate)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

    }
}
