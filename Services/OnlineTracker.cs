// Services/OnlineTracker.cs
using System.Collections.Concurrent;

public interface IOnlineTracker
{
    int Add(string connectionId);
    int Remove(string connectionId);
    int Count { get; }
}

public class OnlineTracker : IOnlineTracker
{
    private readonly ConcurrentDictionary<string, byte> _conns = new();
    public int Add(string connectionId)
    {
        _conns.TryAdd(connectionId, 0);
        return _conns.Count;
    }

    public int Remove(string connectionId)
    {
        _conns.TryRemove(connectionId, out _);
        return _conns.Count;
    }

    public int Count => _conns.Count;
}
