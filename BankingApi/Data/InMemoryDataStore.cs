using BankingApi.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BankingApi.Data;

public class InMemoryDataStore<T> where T : Entity
{
    private readonly ConcurrentDictionary<string, T> _data = new();

    public virtual T Get(string id)
    {
        if (_data.TryGetValue(id, out T value))
        {
            return value;
        }

        return null;
    }

    public virtual bool Set(string id, T data)
    {
        return _data.TryAdd(id, data);
    }

    public virtual bool Remove(string id)
    {
        return _data.TryRemove(id, out _);
    }

    public virtual bool ContainsKey(string id)
    {
        return _data.ContainsKey(id);
    }

    public virtual ICollection<T> GetAll()
    {
        return _data.Values;
    }
}
