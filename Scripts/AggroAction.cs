using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroAction : Action
{
	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		// if has a target, stays on that target permanently
		if (agent.target != null)
		{
			return 2000f;
		}
		foreach (GameObject e in agent.aliveEnemies)
		{
			// if skeleton king alive, auto aggro 
			if (e.ToString().Contains("Skeleton King"))
			{
				return 2f;
			}
		}
		// if player within aggro range, seek towards them
		foreach (GameObject o in agent.alivePlayers)
		{
			agent.distance = Vector3.Distance(o.transform.position, transform.position);
			if (agent.distance < agent.aggroRange)
			{
				return 2f;
			}
		}
		return 0f;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		// keep seeking target
		agent.navMeshAgent.isStopped = false;
		agent.FaceTarget(agent);
		agent.navMeshAgent.SetDestination(agent.target.position);
	}
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
		// lock onto closest player when entering
		agent.FindClosest(agent.alivePlayers);
		agent.navMeshAgent.isStopped = false;
		agent.animator.SetBool("Seek", true);
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{
		agent.navMeshAgent.isStopped = true;
		agent.animator.SetBool("Seek", false);
	}
}
