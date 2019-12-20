using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WildLife;
/// <summary>
///  In this class, we will define the necessary methods to control birds behavior. This class extends from Creature base class,
///  which contains more general methods and properties needed for all the other creatures.
/// </summary>
public class FireBird : Creature
{
    private void Start()
    {
        SubscribeToManager();
        creatureAnimator = new Animator();
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
