using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WildLife;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnAnimal()
    {
        CreatureManager.instance.SpawnCreature();
    }

    public void GoIntoRandomMode()
    {
        CreatureManager.instance.ActivateCreatureMode(creatureMode.random, creatureType.FireBird, 0);
    }

    public void GoIntoSpecificMode()
    {
        CreatureManager.instance.ActivateCreatureMode(creatureMode.pathSpecific, creatureType.FireBird, 0);
    }

    public void GoIntoPlayerReactMode()
    {
        CreatureManager.instance.ActivateCreatureMode(creatureMode.playerReact, creatureType.FireBird, 0);
    }

    public void KillAllBirds()
    {
        CreatureManager.instance.KillAllCreatures(creatureType.FireBird);
    }
}
