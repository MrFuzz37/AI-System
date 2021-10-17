using UnityEngine;

// FSM state for spawning of the boss
public class SpawnAction : Action
{
	// Time before spawning is complete
    public float spawnTime = 10f;

	// evaluates this action every frame to determine whether this state should be entered
    public override float Evaluate(Agent agent)
	{
		spawnTime -= Time.deltaTime;
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		// if spawning is not yet complete 
        if (spawnTime > 0 && agent.alivePlayers.Length > 0)
        {
			// stay in this empty state
            return 500f;
        }
		// otherwise ignore this state
        return 0f;
    }
	// if we are currently in this state update this state
    public override void UpdateAction(Agent agent)
	{
        spawnTime -= Time.deltaTime;
    }
	// run any special code upon entering 
    public override void Enter(Agent agent)
	{

    }
	// run any special code upon exiting 
    public override void Exit(Agent agent)
	{

    }
}
