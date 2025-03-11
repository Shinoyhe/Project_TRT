using System.Collections.Generic;
using UnityEngine;

public class Serialize
{
    public static List<Pair<T1, T2>> FromDict<T1, T2>(Dictionary<T1, T2> dictionary)
    {
        if (dictionary == null)
        {
            return null;
        }

        List<Pair<T1, T2>> returnList = new List<Pair<T1, T2>>();

        foreach (KeyValuePair<T1, T2> pair in dictionary)
        {
            returnList.Add(new Pair<T1, T2>(pair.Key, pair.Value));
        }

        return returnList;
    }

    public static Dictionary<T1, T2> ToDict<T1, T2>(List<Pair<T1, T2>> pairs)
    {
        Dictionary<T1, T2> returnDict = new Dictionary<T1, T2>();

        foreach (Pair<T1, T2> pair in pairs)
        {
            returnDict.Add(pair.First, pair.Second);
        }

        return returnDict;
    }
}

[System.Serializable]
public struct Pair<T1, T2>
{
    public T1 First;
    public T2 Second;

    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}