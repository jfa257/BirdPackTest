using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WildLife;

public class FireBird : Creature
{
    private void Start()
    {
        SubscribeToManager();
    }

    public PathArray GetRandomPath()
    {
        return randomPath;
    }

    public PathArray GetSpecificPath()
    {
        return specificPath;
    }
}
