using Microsoft.AspNetCore.SignalR;

namespace StudentNameRazorPages.Hubs;

public class NewsHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendNewsUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveNewsUpdate", message);
    }

    public async Task SendNewsCreated(object newsData)
    {
        await Clients.All.SendAsync("NewsCreated", newsData);
    }

    public async Task SendNewsUpdated(object newsData)
    {
        await Clients.All.SendAsync("NewsUpdated", newsData);
    }

    public async Task SendNewsDeleted(string newsId)
    {
        await Clients.All.SendAsync("NewsDeleted", newsId);
    }
}
