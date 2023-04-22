/*
Copyright © 2022 Redhacker1

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


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