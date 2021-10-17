using UnityEngine;

// FSM state for seeking
public class SeekAction : Action
{
	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		// if player within aggro range, seek towards them
		foreach (GameObject o in agent.alivePlayers)
		{
			agent.distance = Vector3.Distance(o.transform.position, transform.position);
			if (agent.distance < agent.aggroRange)
			{
				if (agent.distance <= agent.navMeshAgent.stoppingDistance)
				{
					return 0f;
				}
				return 2f;
			}
		}
		return 0f;
	}
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{
		// keep finding the closest enemy to target
		if (agent.FindClosest(agent.alivePlayers) <= agent.aggroRange)
		{
			agent.navMeshAgent.SetDestination(agent.target.position);
			agent.FaceTarget(agent);
		}
		foreach (Status s in agent.statusEffects)
		{
			// stop movement during these effects
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
		agent.animator.SetBool("Seek", true);
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{
        agent.navMeshAgent.isStopped = true;
        agent.animator.SetBool("Seek", false);
    }   
}
