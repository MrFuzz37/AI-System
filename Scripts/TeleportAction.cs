using UnityEngine;

// FSM state for teleporting
public class TeleportAction : Action
{
	// cooldown timer
	public float timer = 0;

	// sound effect
    public AudioClip soundEffect;

	// evaluates this action every frame to determine whether this state should be entered
    public override float Evaluate(Agent agent)
	{
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		if (timer <= 0)
		{
			// if the furthest player is within range
			if (agent.FindFurthestPlayer() <= agent.aggroRange)
			{
				if (agent.target != null)
				{
					if (agent.animator.GetCurrentAnimatorStateInfo(0).IsName("Teleport"))
					{
						return 12f;
					}
					else
					{
						return 0f;
					}
				}
			}
		}
		timer -= Time.deltaTime;
		return 0f;
    }
    
	// if we are currently in this state update this state
    public override void UpdateAction(Agent agent)
	{

	}
	// run any special code upon entering 
	public override void Enter(Agent agent)
	{
		// play particle and sounds while teleporting
        Instantiate(agent.abilities[0].particleEffect, agent.transform);
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
		agent.navMeshAgent.Warp(agent.target.transform.position - (agent.target.transform.forward * 1f));
		timer = 2;
	}
	// run any special code upon exiting 
	public override void Exit(Agent agent)
	{

    }
}
