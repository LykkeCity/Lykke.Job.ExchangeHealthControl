﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using MoreLinq;

namespace Lykke.Job.ExchangeHealthControl.Services.Caches
{
    /// <summary>
    /// Immutable thread-safe generic cache based on two embedded dictionaries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericDoubleDictionaryCache<T> : IGenericDoubleDictionaryCache<T>
        where T: class, IDoubleKeyedObject, ICloneable
    {
        private Dictionary<string, Dictionary<string, T>> _cache;

        private static readonly object LockObj = new object();

        protected GenericDoubleDictionaryCache()
        {
            ClearAll();
        }
        
        public T Get(string partitionKey, string rowKey)
        {
            lock (LockObj)
            {
                return _cache.ContainsKey(partitionKey) 
                    ? _cache[partitionKey].ContainsKey(rowKey)
                        ? (T)_cache[partitionKey][rowKey].Clone()
                        : null
                    : null;
            }
        }

        public IReadOnlyList<T> Get(string partitionKey)
        {
            lock (LockObj)
            {
                return _cache.ContainsKey(partitionKey) 
                    ? _cache[partitionKey].Values.Select(x => (T)x.Clone()).ToList()
                    : new List<T>();
            }
        }

        public IReadOnlyList<T> GetAll()
        {
           lock(LockObj)
           {
               return _cache.Values.SelectMany(x => x.Values).Select(x => (T)x.Clone()).ToList();
           }
        }

        private void SetNoLock(T item)
        {
            if(!_cache.TryGetValue(item.GetPartitionKey, out var exchange))
            {
                _cache[item.GetPartitionKey] = new Dictionary<string, T>();
                exchange = _cache[item.GetPartitionKey];
            }

            exchange[item.GetRowKey] = (T)item.Clone();
        }

        public void Set(T item)
        {
            if (item == null)
                return;
            
            lock (LockObj)
            {
                SetNoLock(item);
            }
        }

        public void SetAll(IEnumerable<T> items)
        {
            if (items == null)
                return;
            
            lock (LockObj)
            {
                items.Where(x => x != null).ForEach(SetNoLock);
            }
        }

        public void Clear(string partitionKey, string rowKey)
        {
            lock (LockObj)
            {
                _cache.TryGetValue(partitionKey, out var inner);
                inner?.Remove(rowKey);
            }
        }

        public void ClearAll()
        {
            lock (LockObj)
            {
                _cache = new Dictionary<string, Dictionary<string, T>>();
            }
        }

        public void Initialize(IEnumerable<T> items)
        {
            ClearAll();
            SetAll(items);
        }
    }
}
