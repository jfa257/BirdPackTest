using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace WildLife
{

    public abstract class Creature : MonoBehaviour
    {
        public float flySpeed;
        public float walkSpeed;
        public bool landed = true;
        public int healthPoints;
        public int nextPathIndex;
        public Animator creatureAnimator;
        public creatureActions lastAction;
        public creatureMode status;
        public NavMeshAgent agent;
        public PathArray randomPath;
        public PathArray specificPath;
        public float minDistanceToTarget;
        public bool colliderOnPath = false;
        public List<int> indexes = new List<int> { 0, 1, 2, 3, 4 };
        public List<int> temporalIndexes = new List<int>();
        public Vector3 startPosForAgent;
        public float baseOffsetValue;
        private Coroutine heightCoroutine;
        public creatureDirection curDirection;
        private int randomIndexLast = 0;
        private float localSpeed = 0.023f;



        private void Start()
        {
            baseOffsetValue = agent.baseOffset;
        }

        public PathArray GetProperPathArray()
        {
            switch (status)
            {
                case creatureMode.pathSpecific:
                    return specificPath;
                case creatureMode.random:
                    return randomPath;
                case creatureMode.playerReact:
                    return null;
                default:
                    return null;
            }
        }

        public bool WithinHeightBounds()
        {

            if(status == creatureMode.pathSpecific)
            {
                if (curDirection == creatureDirection.down)
                {
                    return transform.position.y >= specificPath.targetsOnPath[nextPathIndex].y;
                }
                else
                {
                    return transform.position.y <= specificPath.targetsOnPath[nextPathIndex].y;
                }
            }
            else if (status == creatureMode.random)
            {
                if (curDirection == creatureDirection.down)
                {
                    return transform.position.y >= randomPath.targetsOnPath[nextPathIndex].y;
                }
                else
                {
                    return transform.position.y <= randomPath.targetsOnPath[nextPathIndex].y;
                }
            }
            else 
            {
                if (curDirection == creatureDirection.down)
                {
                    return transform.position.y >= CreatureManager.instance.playerLocation.position.y;
                }
                else
                {
                    return transform.position.y <= CreatureManager.instance.playerLocation.position.y;
                }
            }
        }

        public float GetModifierValue ()
        {
            return 0;
        }

        public IEnumerator UpdateAgentHeight()
        {
            while (!agent.isStopped)
            {
                if (!NearToTarget() && WithinHeightBounds() || status == creatureMode.playerReact && WithinHeightBounds())
                {
                    if (curDirection == creatureDirection.up)
                    {
                        agent.baseOffset += (localSpeed / Vector3.Distance(startPosForAgent, agent.destination)) * (1.0f / Time.deltaTime) * Time.fixedDeltaTime;
                    }
                    else if (curDirection == creatureDirection.down)
                    {
                        agent.baseOffset -= (localSpeed / Vector3.Distance(startPosForAgent, agent.destination)) * (1.0f / Time.deltaTime) * Time.fixedDeltaTime;
                    }
                }
                yield return null;
            }
        }


        public IEnumerator GoToNextTarget(int targetIndex)
        {
            while (!colliderOnPath)
            {
                transform.position = Vector3.MoveTowards(transform.position, GetProperPathArray().targetsOnPath[targetIndex], 0.1f);
                yield return null;
            }
            yield return null;
        }

        public IEnumerator SteeringOut()
        {
            while (colliderOnPath)
            {
                yield return null;
            }
        }

        public creatureDirection UpdateDirection()
        {
            return status == creatureMode.playerReact ? Mathf.Round(transform.position.y) > Mathf.Round(CreatureManager.instance.playerLocation.position.y) ? creatureDirection.down : creatureDirection.up :
                   Mathf.Round(transform.position.y) > Mathf.Round(status == creatureMode.random ? randomPath.targetsOnPath[nextPathIndex].y : specificPath.targetsOnPath[nextPathIndex].y) ? creatureDirection.down : creatureDirection.up;
        }

        public bool NearToTarget()
        {
            return agent.remainingDistance <= minDistanceToTarget;
        }


        public void GoOnRandomPath(PathArray randomPath)
        {
            
            if (status == creatureMode.random)
            {
                if (randomIndexLast >= randomPath.targetsOnPath.Count || randomIndexLast == 0)
                {
                    temporalIndexes = new List<int>();
                    temporalIndexes = indexes.OrderBy(x => Random.Range(0,randomPath.targetsOnPath.Count-1)).ToList();
                    randomIndexLast = 0;
                    Debug.Log("INDEX NOW IS: " + indexes.Count + "index value is: " + temporalIndexes[randomIndexLast]);
                }

               

                nextPathIndex = temporalIndexes[randomIndexLast];
               
                agent.SetDestination(randomPath.targetsOnPath[nextPathIndex]);
                startPosForAgent = transform.position;
                randomIndexLast++;
                StartCoroutine(WaitForDestination(randomPath, status));
            }
            else
            {
                Debug.Log("CREATURE STOPPED RANDOM MODE");
            }
        }
        
        public void ActivateHeightVariation()
        {
            if (heightCoroutine == null || agent.isStopped)
            {
                if (heightCoroutine != null)
                {
                    StopCoroutine(heightCoroutine);
                }
                heightCoroutine = StartCoroutine(UpdateAgentHeight());
            }
        }

        private IEnumerator WaitForDestination(PathArray path, creatureMode mode)
        {
            curDirection = UpdateDirection();
            ActivateHeightVariation();
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => NearToTarget() && agent.remainingDistance > 0);
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
                Debug.Log("GOING TO TARGET NUMBER: " + nextPathIndex + " WITH INDEX: " + nextPathIndex);
                StartCoroutine(WaitForDestination(specificPath, status));
                //ActivateHeightVariation();
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
            nextPathIndex = 0;
            lastAction = creatureActions.idle;
            if (status == creatureMode.random)
            {
                GoOnRandomPath(requiredPath);
            }
            else if (status == creatureMode.playerReact)
            {
                GoOnPlayerReact();
            }
            else if (status == creatureMode.pathSpecific)
            {
                GoOnSpecificPath(requiredPath);
            }
        }
        public void ExecuteAction(creatureActions calledAction)
        {
            if (calledAction == creatureActions.attack)
            {
                // activate attack animation state
            }
            else if (calledAction == creatureActions.summoned)
            {
                // activate summoned animation state
                GoToPlayerLocation();
                ExecuteAction(creatureActions.idle);
            }
            else if (calledAction == creatureActions.die)
            {
                // activate die animation state
            }
            //.....
        }

        public void GoToPlayerLocation()
        {
            curDirection = UpdateDirection();
            agent.SetDestination(CreatureManager.instance.playerLocation.position);
            ActivateHeightVariation();
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
