using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WildLife;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public GameObject warningDialog;
    public static List<string> warningMessages = new List<string> {"Must Create bird first!\n Okay", "you already have one bird!\n Okay", " Press S on keyboard to reach player!\nOkay" };
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
        if (CreatureManager.instance.activeCreatures.Count == 0)
        {
            CreatureManager.instance.SpawnCreature();
        }
        else
        {
            ShowWarningMsg(1);
        }
    }

    public void GoIntoRandomMode()
    {
        if (CreatureManager.instance.activeCreatures.Count > 0)
        {
            CreatureManager.instance.ActivateCreatureMode(creatureMode.random, creatureType.FireBird, 0);
        }
        else
        {
            ShowWarningMsg(0);
        }
    }

    public void GoIntoSpecificMode()
    {
        if (CreatureManager.instance.activeCreatures.Count > 0)
        {
            CreatureManager.instance.ActivateCreatureMode(creatureMode.pathSpecific, creatureType.FireBird, 0);
        }
        else
        {
            ShowWarningMsg(0);
        }
    }

    public void GoIntoPlayerReactMode()
    {
        if (CreatureManager.instance.activeCreatures.Count > 0)
        {
            CreatureManager.instance.ActivateCreatureMode(creatureMode.playerReact, creatureType.FireBird, 0);
            ShowWarningMsg(2);
        }
        else
        {
            ShowWarningMsg(0);
        }
    }

    public void KillAllBirds()
    {
        if (CreatureManager.instance.activeCreatures.Count > 0)
        {
            CreatureManager.instance.KillAllCreatures(creatureType.FireBird);
        }
        else
        {
            ShowWarningMsg(0);
        }
    }

    public void ShowWarningMsg(int warningIndex)
    {
        warningDialog.GetComponentInChildren<Text>().text = warningMessages[warningIndex];
        warningDialog.SetActive(true);
    }

    public void HideWarningMsg ()
    {
        warningDialog.SetActive(false);
    }

}
