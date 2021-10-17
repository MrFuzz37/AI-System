using UnityEngine;

// FSM state for idle
public class IdleAction : Action
{
	// evaluates this action every frame to determine whether this state should be entered
	public override float Evaluate(Agent agent)
	{
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		agent.aliveEnemies = GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.ToArray();
		// while no players are currently alive all enemies remain idle
		if (agent.alivePlayers.Length > 0)
		{
            // if player within aggro range
            foreach (GameObject o in agent.alivePlayers)
            {
                float distance = Vector3.Distance(o.transform.position, transform.position);
                if (distance < agent.aggroRange)
                {
					// no longer idle
                    return 0f;
                }
                else
                {
                    return 0f;
                }
            }
		}
        return 8f;
    }
	// if we are currently in this state update this state
	public override void UpdateAction(Agent agent)
	{

    }
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
        agent.animator.SetBool("Idle", true);
    }
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{       
        agent.animator.SetBool("Idle", false);      
    }
}
