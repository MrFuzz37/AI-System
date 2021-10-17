using System;
using UnityEngine;

public class PatrolAction : Action
{
	private int destinationPoint = 0;

	[Tooltip("A list of points represented as empty gameobjects")]
	public Transform[] points;
	
	public override float Evaluate(Agent agent)
	{
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		// if no players within aggro range, patrol between points
		foreach (GameObject o in agent.alivePlayers)
		{
			float distance = Vector3.Distance(o.transform.position, transform.position);
			if (distance < agent.aggroRange)
			{
				return 0f;
			}
		}
		return 1f;
	}
	public override void UpdateAction(Agent agent)
	{
		// next waypoint is selected when close to the current waypoint
		if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < 1f)
		{
			GotoNextPoint(agent);
		}
	}
	public override void Enter(Agent agent)
	{
		// gives continuous movement
		agent.navMeshAgent.autoBraking = false;
		GotoNextPoint(agent);
	}
	public override void Exit(Agent agent)
	{
        destinationPoint = 0;
        agent.navMeshAgent.autoBraking = true;
	}
	// Enemy will go to the next waypoint
	private void GotoNextPoint(Agent agent)
	{
		// error handling if no waypoints
		if (points.Length == 0)
		{
			Debug.Break();
			throw new Exception("No waypoints set to patrol!");
		}
        agent.target = points[destinationPoint];
        agent.FaceTarget(agent);
		agent.navMeshAgent.destination = points[destinationPoint].position;
		// choose next point in array, cycles to the start to loop path
		destinationPoint = (destinationPoint + 1) % points.Length;
	}
}
