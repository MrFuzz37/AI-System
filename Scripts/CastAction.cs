using UnityEngine;

// FSM state for casting as the boss
public class CastAction : Action
{
	// cooldown
	public float castTime;

	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
    {
		castTime -= Time.deltaTime;
        agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
        if (agent.alivePlayers.Length > 0)
        {
            // if player within aggro range, seek towards them
            foreach (GameObject o in agent.alivePlayers)
            {
				if (agent.target != null)
				{
					// if any player is in sight and attack is off cooldown
					float distance = Vector3.Distance(o.transform.position, transform.position);
					if (distance < agent.aggroRange && castTime <= 0)
					{
						// return a random value
						return Random.Range(2.5f, 5f);
					}
				}
				return 0f;
            }
        }
        return 0f;
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
		agent.animator.SetTrigger("Cast");
		castTime = 10;
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
    {

    }
}
