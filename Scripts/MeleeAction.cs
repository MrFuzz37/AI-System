using System;
using UnityEngine;

public class MeleeAction : Action
{
	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		foreach (GameObject o in agent.alivePlayers)
		{
			// if any player is within attacking range 
			agent.distance = Vector3.Distance(o.transform.position, transform.position);
			// if no ability is assigned throw exception
			if (GetComponent<Character>().abilities.Length <= 0)
			{
				Debug.Break();
				throw new Exception("No ability assigned!");
			}
			else if (agent.distance <= GetComponent<Character>().abilities[0].attackRange)
			{
				// return a high value
				return 10f;
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
