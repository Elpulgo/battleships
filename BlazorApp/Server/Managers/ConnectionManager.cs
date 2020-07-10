
using System.Collections.Generic;
using System.Linq;

namespace BlazorApp.Server.Managers
{
    public class ConnectionManager<T>
    {
        private readonly Dictionary<T, string> _connections =
            new Dictionary<T, string>();

        public int Count => _connections.Count;

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                _connections.TryAdd(key, connectionId);
            }
        }

        public string GetConnection(T key)
        {
            if (_connections.TryGetValue(key, out var connectionId))
                return connectionId;

            return string.Empty;
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(key))
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}