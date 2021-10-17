using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAction : Action
{
	[HideInInspector]
	public float timer = 0;

	// cooldown stuff
	public float attackCooldown = 3;

	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		// attack cooldown timer
		timer -= Time.deltaTime;
		foreach (GameObject o in agent.alivePlayers)
		{
			// if any player is within sight range
			agent.distance = Vector3.Distance(o.transform.position, transform.position);
			if (agent.distance <= agent.aggroRange)
			{
				// and attack off cooldown
				if (timer <= 0)
				{
					// return a high value
					return 6f;
				}
			}
		}
		return 0;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		// constantly look at player target without moving
		agent.navMeshAgent.velocity = Vector3.zero;
		agent.FaceTarget(agent);
	}
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
		agent.animator.SetTrigger("Attack");
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{

	}
}
