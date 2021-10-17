using UnityEngine;
using UnityEngine.AI;

// FSM state for fleeing
public class FleeAction : Action
{
	[Tooltip("How close a player needs to be before fleeing")]
	public float fleeRange;

	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		// is player within flee range
		if (agent.FindClosest(agent.alivePlayers) <= fleeRange)
		{
			// return a high value
			return 6f;
		}
		return 0;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		// if player is within flee range, run opposite direction
		if (agent.FindClosest(agent.alivePlayers) <= agent.aggroRange)
		{			
			transform.rotation = Quaternion.LookRotation(transform.position - agent.target.position);
			Vector3 runTo = transform.position + transform.forward * agent.navMeshAgent.speed;
			NavMesh.SamplePosition(runTo, out NavMeshHit hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

			agent.FaceTarget(agent);
            agent.navMeshAgent.SetDestination(hit.position);
		}
        foreach (Status s in agent.statusEffects)
        {
			// stop movement
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
		agent.animator.SetBool("Flee", true);
		agent.navMeshAgent.isStopped = false;
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{
		agent.animator.SetBool("Flee", false);
		agent.navMeshAgent.isStopped = true;
	}
}
