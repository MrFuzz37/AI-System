using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// FSM state for wandering
public class WanderAction : Action
{
	// timer for when to pick a new position
	private float timer;
	[Tooltip("How far away the enemy will find a target to wander to")]
	public float wanderRadius;
	[Tooltip("Time it takes before the enemy will find a new wander target")]
	public float wanderTimer;

	// reference to base speed
    public float prevSpeed;

	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		// get a list of enemies
		agent.aliveEnemies = GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.ToArray();
        List<GameObject> archers = new List<GameObject>();

        foreach (GameObject o in agent.aliveEnemies)
		{
            if (o.ToString().Contains("Skeleton Archer"))
            {
				// add all archers to a list
                archers.Add(o);
            }
		}
		// if only 2 archers are left alive
        if (agent.aliveEnemies.Length <= 2 && archers.Count > 1)
        {
			// they both wander around
            return 5f;
        }
		// otherwise if we are the last alive
        else if (agent.aliveEnemies.Length < 2)
        {
			// we wander
            return 5f;
        }
        return 3f;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		timer += Time.deltaTime;
		// if timer is greater than wandertimer, find new wander target
		if (timer >= wanderTimer)
		{
			Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
			agent.navMeshAgent.SetDestination(newPos);
			timer = 0;
		}
        foreach (Status s in agent.statusEffects)
        {
			// stop movement if these effects are active
            if (s.name == "Freeze" || s.name == "Stun")
            {
                agent.navMeshAgent.isStopped = true;
            }
            else
            {
                agent.navMeshAgent.isStopped = false;
            }
        }

    }
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
		agent.navMeshAgent.isStopped = false;
		prevSpeed = agent.navMeshAgent.speed;
		agent.navMeshAgent.speed = 1.5f;
		timer = wanderTimer;
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{
		agent.navMeshAgent.isStopped = true;
        agent.navMeshAgent.speed = prevSpeed;
	}
	// finds a random location within a specific range
	public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
	{
		Vector3 randDirection = Random.insideUnitSphere * distance;
		randDirection += origin;
		NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, distance, layermask);
		return navHit.position;
	}
	// visual debugging info
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, wanderRadius);
	}
}
