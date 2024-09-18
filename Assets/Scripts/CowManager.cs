using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowManager : MonoBehaviour
{
    private static CowManager _instance;
    public static CowManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<Cow> cows = new List<Cow>();

    void Awake()
    {
        _instance = this;
    }

    public void RegisterCow(Cow cow)
    {
        if (!cows.Contains(cow))
        {
            cows.Add(cow);
        }
    }

    public void UnregisterCow(Cow cow)
    {
        if (cows.Contains(cow))
        {
            cows.Remove(cow);
        }
    }

    public List<Cow> GetCows()
    {
        return cows;
    }
}
