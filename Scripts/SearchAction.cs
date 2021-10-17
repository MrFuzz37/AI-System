using UnityEngine;

// FSM state for searching
public class SearchAction : Action
{
	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		// get a list of all active players and enemies
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		agent.aliveEnemies = GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.ToArray();
		foreach (GameObject o in agent.aliveEnemies)
		{
			// if an enemy is too close to follow, stop following
			if (transform != o.transform)
			{
				agent.distance = Vector3.Distance(o.transform.position, transform.position);
				if (agent.distance <= 4)
					return 0;
			}
		}
		// while no players are currently alive all enemies remain idle
		if (agent.aliveEnemies.Length > 0)
		{
			return 4f;
		}
		return 0f;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		// find the closest nearby enemy
		agent.FindClosest(agent.aliveEnemies);
		agent.navMeshAgent.isStopped = false;
		agent.FaceTarget(agent);
		// move towards ally
		agent.navMeshAgent.SetDestination(agent.target.transform.position - (agent.target.transform.forward * 2f));
	}
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
		agent.animator.SetBool("Flee", true);
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{
		agent.animator.SetBool("Flee", false);
	}
}
