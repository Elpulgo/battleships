
using System;
using System.Collections.Generic;

namespace BlazorApp.Server.Managers
{
    public class ConnectionManager
    {
        private readonly Dictionary<Guid, string> _connections =
            new Dictionary<Guid, string>();

        public int Count => _connections.Count;

        public void Add(Guid key, string connectionId)
        {
            lock (_connections)
            {
                _connections.TryAdd(key, connectionId);
            }
        }

        public string GetConnection(Guid key)
        {
            if (_connections.TryGetValue(key, out var connectionId))
                return connectionId;

            return string.Empty;
        }

        public void Remove(Guid key)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(key))
                {
                    _connections.Remove(key);
                }
            }
        }

        public void Reset()
        {
            lock (_connections)
            {
                _connections.Clear();
            }
        }
    }
}