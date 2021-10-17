using UnityEngine;

// FSM state for summoning
public class SummonAction : Action
{
	// prefabs for summoning
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    public GameObject prefab5;

	// spawn locations
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;

	// cooldown
    public float summonCooldown = 10f;

	// sound effect
    public AudioClip soundEffect;

	// evaluates this action every frame to determine whether this state should be entered
    public override float Evaluate(Agent agent)
	{
		summonCooldown -= Time.deltaTime;
		agent.alivePlayers = GameObject.FindGameObjectsWithTag("Player");
		// while no players are currently alive all enemies remain idle
		if (agent.alivePlayers.Length > 0)
		{
			// if there is 2 or less enemies remaining
			if (summonCooldown <= 0 && GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.Count <= 2)
			{
				return 6f;
			}
			// if there is 5 or less enemies remaining
			else if (summonCooldown <= 0 && GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.Count <= 5)
			{
				// Has a random chance to summon
				float value = Random.Range(0, 5000);
				if (value == 37)
					return 6;
			}
		}
		return 0f;
    }
	// if we are currently in this state update this state
    public override void UpdateAction(Agent agent)
	{

    }
	// run any special code upon entering 
    public override void Enter(Agent agent)
	{
		// stop moving
        agent.navMeshAgent.isStopped = true;
		agent.navMeshAgent.angularSpeed = 0;
        agent.animator.SetTrigger("Summon");
		summonCooldown = 10;
	}
	// run any special code upon exiting 
    public override void Exit(Agent agent)
	{

    }
}
