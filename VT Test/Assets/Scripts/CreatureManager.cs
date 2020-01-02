using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace WildLife
{
    public enum creatureActions { idle, eat, walk, run, fly, attack, defend, hide, evoked, die, getHurt, getLaunched, getCared, summoned }
    public enum creatureType { FireBird, IceDog, SilverCat, StoneBear, SpiritSnake, undefined }
    public enum creatureMode { random, playerReact, pathSpecific }
    public enum creatureDirection { none, up, down, left, right}

    public class CreatureManager : MonoBehaviour
    {
        public static CreatureManager instance { get; private set; }

        public List<GameObject> activeCreatures;
        public List<GameObject> prefabs;
        public List<Transform> debugArray;
        public Transform playerLocation;


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


        public void ActivateCreatureMode(creatureMode mode, creatureType animalType, int creatureIndex)
        {
            if (animalType.ToString() == "FireBird")
            {
                if (mode == creatureMode.random)
                {
                    activeCreatures[creatureIndex].GetComponent<FireBird>().status = creatureMode.random;
                    activeCreatures[creatureIndex].GetComponent<FireBird>().ExecuteCurrentMode(activeCreatures[creatureIndex].GetComponent<FireBird>().GetRandomPath());
                }
                else if (mode == creatureMode.pathSpecific)
                {
                    activeCreatures[creatureIndex].GetComponent<FireBird>().status = creatureMode.pathSpecific;
                    activeCreatures[creatureIndex].GetComponent<FireBird>().ExecuteCurrentMode(activeCreatures[creatureIndex].GetComponent<FireBird>().GetSpecificPath());
                }
                else if (mode == creatureMode.playerReact)
                {
                    activeCreatures[creatureIndex].GetComponent<FireBird>().status = creatureMode.playerReact;
                    activeCreatures[creatureIndex].GetComponent<FireBird>().ExecuteCurrentMode();
                }
            }
            else if (animalType.ToString() == "other animaltype")
            {
                //....similar logic to above
            }
        }


        public void SpawnCreature(creatureType animalType = creatureType.FireBird, int numCreatures = 1)
        {
            for (int i = 0; i < numCreatures; i++)
            {
                GameObject g = prefabs.Find(x => x.GetComponent(animalType.ToString()) != null);
                Instantiate(g);
            }
        }

        public List<GameObject> ModifyCreatureList(GameObject creature, bool remove)
        {
            if (remove)
            {
                if (activeCreatures.Find(x => x == creature) != null)
                {
                    activeCreatures.Remove(creature);
                }
            }
            else
            {
                if (activeCreatures.Find(x => x == creature) == null)
                {
                    activeCreatures.Add(creature);
                }
            }
            return activeCreatures;
        }


        public void KillAllCreatures(creatureType animalType)
        {
            object obj = activeCreatures[0].GetComponent(animalType.ToString());
            List<GameObject> creaturesToKill = activeCreatures.FindAll(x => x.GetComponent(obj.GetType()) != null);

            foreach (GameObject g in creaturesToKill)
            {
                g.GetComponent(obj.GetType()).SendMessage("AutoKillCall");
            }
        }
    }
}
