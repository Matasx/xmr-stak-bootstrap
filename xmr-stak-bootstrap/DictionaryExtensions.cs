using System;
using System.Collections.Generic;

namespace xmr_stak_bootstrap
{
    public static class DictionaryExtensions
    {
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            throw new Exception($"Cannot find key '{key}'.");
        }
    }
}