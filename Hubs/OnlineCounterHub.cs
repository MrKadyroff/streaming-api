using Microsoft.AspNetCore.SignalR;

public class OnlineHub : Hub
{
    private readonly IOnlineTracker _tracker;

    public OnlineHub(IOnlineTracker tracker)
    {
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        var count = _tracker.Add(Context.ConnectionId);
        await Clients.All.SendAsync("onlineCount", count);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var count = _tracker.Remove(Context.ConnectionId);
        await Clients.All.SendAsync("onlineCount", count);
        await base.OnDisconnectedAsync(ex);
    }
}
