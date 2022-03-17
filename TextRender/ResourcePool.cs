using System;
using System.Collections.Generic;

namespace TextRender;

class ResourcePool<TValue, TKey> : DisposableManager where TValue : IDisposable
{
    Func<TValue, TKey> ToKey;
    Func<TKey, TValue> FromKey;
   

    Dictionary<TKey, List<TValue>> Rented = new Dictionary<TKey, List<TValue>>();
    Dictionary<TKey, List<TValue>> Unrented = new Dictionary<TKey, List<TValue>>();

    public ResourcePool(Func<TValue,TKey> toKeyValue, Func<TKey,TValue> FromKeyValue)
    {
        ToKey = toKeyValue;
        FromKey = FromKeyValue;
    }

    public void Dispose()
    {
        foreach (KeyValuePair<TKey, List<TValue>> bucket in Rented)
        {
            foreach (var item in bucket.Value)
            {
                item.Dispose();
            }
            Rented.Clear();
        }
        Rented.Clear();
    }

    public TValue Acquire(TKey KeyValue)
    {
        if (Unrented.ContainsKey(KeyValue))
        {
            if (Unrented[KeyValue].Count >= 1)
            {
                TValue ReturnValue = Unrented[KeyValue][0];
                Unrented[KeyValue].RemoveAt(0);
                if (Rented.ContainsKey(KeyValue) == false)
                {
                    Rented.Add(KeyValue, new List<TValue>());
                }
                Rented[KeyValue].Add(ReturnValue);
                return ReturnValue;   
            }
            else
            {
                TValue ReturnValue = FromKey(KeyValue);
                if (Rented.ContainsKey(KeyValue) == false)
                {
                    Rented.Add(KeyValue, new List<TValue>());
                }
                Rented[KeyValue].Add(ReturnValue);
                return ReturnValue;  
            }
        }
        else
        {
            TValue ReturnValue = FromKey(KeyValue);
            Unrented.Add(KeyValue, new List<TValue>() {});
            if (Rented.ContainsKey(KeyValue) == false)
            {
                Rented.Add(KeyValue, new List<TValue>());
            }
            Rented[KeyValue].Add(ReturnValue);
            return FromKey(KeyValue);
        }
    }

    public void ReleaseAll()
    {
        foreach (KeyValuePair<TKey, List<TValue>> bucket in Rented)
        {
            foreach (TValue items in bucket.Value)
            {
                Unrented[ToKey(items)].Add(items);
            }
        }
        Rented.Clear();
    }

    public void Release(TValue value, TKey key)
    {
        if (Rented.ContainsKey(key))
        {
            Rented.Remove(key);
            if (Unrented.ContainsKey(key) == false)
            {
                Unrented.Add(key, new List<TValue>() {value});
            }
        }
    }
}