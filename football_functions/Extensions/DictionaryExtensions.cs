
using System;
using System.Collections.Generic;
using System.Linq;

namespace football_functions.Extensions;

public static class DictionaryExtensions
{
    public static IOrderedEnumerable<KeyValuePair<T, Y>> GetOrderedTeamBy<T, Y, X>(this Dictionary<T, Y> dic, Func<KeyValuePair<T, Y>, X> func) where T : notnull
    {
        return dic.OrderBy(func);
    }

    public static Dictionary<T, Y> InitializeDictionary<T, Y>() where T : notnull
    {
        return new();
    }

    public static Dictionary<int, Y> Fillictionary<Y>(this Dictionary<int, Y> dic, int capacity) where Y : new()
    {
        for (int i = 0; i < capacity; i++)
        {
            dic.Add(i, new());
        }

        return dic;
    }
}
