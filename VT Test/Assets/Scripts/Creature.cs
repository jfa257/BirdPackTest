using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WildLife
{
    public abstract class Creature : MonoBehaviour
    {
        public float flySpeed;
        public float walkSpeed;
        public bool landed = true; // birds will spawn landed by default. Also there will be creatureActions that will be performed or not depending on this value.
        public int healthPoints;
        public int nextPathIndex;
        public Animator creatureAnimator;
        public creatureActions lastAction;
        public creatureMode status;
        public NavMeshAgent agent;
        public PathArray randomPath;
        public PathArray specificPath;
        public float minDistanceToTarget;

        public bool NearToTarget()
        {
            return agent.remainingDistance <= minDistanceToTarget;
        }

        public void GoOnRandomPath(PathArray randomPath)
        {
            if (status == creatureMode.random)
            {
                nextPathIndex = Random.Range(0, randomPath.targetsOnPath.Count - 1); // to avoid start always at 0 index location
                agent.SetDestination(randomPath.targetsOnPath[nextPathIndex]);
                Debug.Log("GOING TO TARGET NUMBER: " + randomPath.targetsOnPath[nextPathIndex].ToString() + " WITH INDEX: " + nextPathIndex);
                StartCoroutine(WaitForDestination(randomPath, status));
            }
            else
            {
                Debug.Log("CREATURE STOPPED RANDOM MODE");
            }
        }


        private IEnumerator WaitForDestination(PathArray path, creatureMode mode)
        {
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => agent.remainingDistance <= minDistanceToTarget && agent.remainingDistance > 0);
            if (mode == creatureMode.random)
            {
                GoOnRandomPath(path);
            }
            else
            {
                nextPathIndex++;
                GoOnSpecificPath(path);
            }
            yield return null;
        }

        public IEnumerator EnablePlayerListener()
        {
            while (status == creatureMode.playerReact)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    ExecuteAction(creatureActions.attack);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    ExecuteAction(creatureActions.defend);
                }
                else if (Input.GetKey(KeyCode.S) && lastAction != creatureActions.summoned)
                {
                    lastAction = creatureActions.summoned;
                    ExecuteAction(creatureActions.summoned);
                    Debug.Log("RUNNING SUMMON");
                }
                else
                {
                    //...
                }
                yield return null;
            }
        }

        public void GoOnSpecificPath(PathArray specificPath)
        {
            if (status == creatureMode.pathSpecific)
            {
                nextPathIndex = nextPathIndex == specificPath.targetsOnPath.Count ? 0 : nextPathIndex;
                agent.SetDestination(specificPath.targetsOnPath[nextPathIndex]);
                Debug.Log("GOING TO TARGET NUMBER: " + specificPath.targetsOnPath[nextPathIndex].ToString() + " WITH INDEX: " + nextPathIndex);
                StartCoroutine(WaitForDestination(specificPath, status));
            }
            else
            {
                Debug.Log("CREATURE STOPPED PATH MODE");
            }
        }

        public void GoOnPlayerReact()
        {
            StartCoroutine(EnablePlayerListener());
        }

        public void DebugGoOnSpecificPath()
        {
            if (status == creatureMode.pathSpecific)
            {
                nextPathIndex = nextPathIndex == CreatureManager.instance.debugArray.Count ? 0 : nextPathIndex;
                agent.SetDestination(CreatureManager.instance.debugArray[nextPathIndex].position);
                StartCoroutine(WaitForDestination(null, status));
            }
            else
            {
                Debug.Log("CREATURE STOPPED PATH MODE");
            }
        }

        public void SubscribeToManager()
        {
            CreatureManager.instance.activeCreatures.Add(gameObject);
        }

        public void ExecuteCurrentMode(PathArray requiredPath = null, creatureActions requiredAction = creatureActions.idle)
        {
            // StopAllCoroutines();
            nextPathIndex = 0;
            lastAction = creatureActions.idle;
            if (status == creatureMode.random)
            {
                GoOnRandomPath(requiredPath);
            }
            else if (status == creatureMode.playerReact)
            {
                // fire logic for creature, in this case bird's player reaction
                GoOnPlayerReact();
            }
            else if (status == creatureMode.pathSpecific)
            {
                GoOnSpecificPath(requiredPath);
            }
        }
        public void ExecuteAction(creatureActions calledAction)
        {
            // here in this function, we access the animator controller reference and activate or deactivate as needed the booleans, triggers or update float values required to 
            // go into the desired animation state.
            if (calledAction == creatureActions.attack)
            {
                // activate attack animation state
            }
            else if (calledAction == creatureActions.summoned)
            {
                // activate summoned animation state
                GoToPlayerLocation();
            }
            else if (calledAction == creatureActions.die)
            {
                // activate die animation state
            }
            //.....
        }

        public void GoToPlayerLocation()
        {
            agent.SetDestination(CreatureManager.instance.playerLocation.position);
            agent.autoBraking = true;
        }
        public void AutoKillCall()
        {
            ExecuteAction(creatureActions.die);
            Debug.Log("terminate initiated");
        }

        public bool CreatureIsAlive()
        {
            return healthPoints > 0;
        }
    }
}
