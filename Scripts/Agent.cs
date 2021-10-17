using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// A class to hold all relevant info for an agent
public class Agent : Character
{
	// The list of actions that can be performed 
	Action[] actions;

	// reference to the animator
    [HideInInspector]
    public Animator animator;

	[Tooltip("Adjusts the range in which this object can detect the player")]
	[Range(1, 100)]
	public float aggroRange = 5f;

	[Tooltip("Displays the current action during runtime")]
	public Action currentAction;

	[Tooltip("Requires a NavMeshAgent component")]
	public NavMeshAgent navMeshAgent;

	// The player this object is currently targeting (updates at run time)
    [HideInInspector]
    public Transform target;

	// distance to target
	public float distance;

	// List of all players alive in the game world (updates at run time)
	[HideInInspector]
	public GameObject[] alivePlayers;

	// List of all enemies alive in the game world (updates at run time)
	[HideInInspector]
	public GameObject[] aliveEnemies;

	//particle reference for shield getting hit
	public ParticleSystem shieldHitPFX;

	// Lists for the boss summoning ability
    private readonly List<GameObject> summonPrefabs = new List<GameObject>();
    private readonly List<Transform> summonLocations = new List<Transform>();

	// used for initialization
	override public void Start()
    {
		// get all action-derived classes that are siblings of us
		actions = GetComponents<Action>();
		// disable the navmesh rotation, doesn't work well with what we want
		navMeshAgent.updateRotation = false;
		maxHealth = health;
        animator = GetComponentInChildren<Animator>();
        //James' Dodgy Trails
        foreach (TrailRenderer t in GetComponentsInChildren<TrailRenderer>())
        {
            tr.Add(t);
        }
        if (GetComponentsInChildren<TrailRenderer>().Length == 1)
        {
            tr[0].enabled = trOn;
            trOn = true;
        }
        if (GetComponentsInChildren<TrailRenderer>().Length == 2)
        {
            tr[0].enabled = trOn;
            trOn = true;
            tr[1].enabled = trOn;
            trOn2 = true;
        }
    }
    // Update is called once per frame
    override public void Update()
    {
		UpdateStatus();
        // find best action each frame
        Action best = GetBestAction();
		// if it's different from what we were doing, switch the FSM
		if (best != currentAction)
		{
			if (currentAction)
			{
				currentAction.Exit(this);
			}

			currentAction = best;

			if (currentAction)
			{
				currentAction.Enter(this);
			}
		}
		// update current action
		if (currentAction)
		{
			currentAction.UpdateAction(this);
		}
    }
	// checks all available actions and evaluates each one, returns the best result
	Action GetBestAction()
	{
		Action action = null;
		float bestValue = 0;
		foreach (Action a in actions)
		{
			float value = a.Evaluate(this);
			if (action == null || value > bestValue)
			{
				action = a;
				bestValue = value;
			}
		}
		return action;
	}

	// keeps the enemy facing towards the currently targeted player at all times
	public void FaceTarget(Agent agent)
    {
		if (agent != null)
		{
			Vector3 relativePos = target.position - transform.position;
			Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);

			Quaternion LookAtRotationOnly_Z = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

			transform.rotation = LookAtRotationOnly_Z;
		}
    }
	// finds the closest enemy or player and sets them as the new target
	public float FindClosest(GameObject[] targets)
	{
		float closest = 1000000;
		if (targets.Length > 0)
		{
			// find the closest player/enemy
			foreach (GameObject o in targets)
			{
                if (o != gameObject && !o.ToString().Contains("Skeleton Archer"))
                {
                    float value = Vector3.Distance(o.transform.position, transform.position);
                    if (value < closest)
                    {
                        closest = value;
                        target = o.transform;
                    }
                }
			}
		}
		return closest;
	}
	// finds the furthest player and sets them as the new target
	public float FindFurthestPlayer()
	{
		float furthest = 0;
		if (alivePlayers.Length > 0)
		{
			// finds the furthest player and targets them
			foreach (GameObject o in alivePlayers)
			{
				float distance = Vector3.Distance(o.transform.position, transform.position);
				if (distance > furthest && distance <= aggroRange)
				{
					furthest = distance;
					target = o.transform;
				}
			}
		}
		if (furthest != 0)
		{
			return furthest;
		}
		return 10000;
	}
	// applies damage to the enemy and checks if they are killed
	override public void ApplyDamage(float dam)
	{
		// subtract from health
		health = Mathf.Clamp(health - dam, 0, maxHealth);

		if (health <= 0)
		{
			List<GameObject> temp = GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned;

			for (int i = 0; i < temp.Count; ++i)
			{
				// if they are killed, remove them from the active list
				if (gameObject == temp[i].gameObject)
				{
					temp.RemoveAt(i);
				}
			}

            Instantiate(deathPS, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), deathPS.transform.rotation);
            Destroy(gameObject);
		}
	}
	// animation event for hit detection on all enemy attacks
	public override void HitBox()
	{
		bool blocked = false;
		Vector3 attack = (transform.position + transform.forward * (abilities[0].attackRange / 2));
		Collider[] col = Physics.OverlapSphere(attack, abilities[0].attackRange / 2, (1 << 8));

		if (col.Length != 0)
		{
			foreach (Collider c in col)
			{
				Character target = c.GetComponent<Character>();
				if (target != null)
				{
					foreach (Status s in target.statusEffects)
					{
						// if player is blocking
						if (s.name == "Block")
						{
							// and facing in the direction of the enemy
							if (Physics.SphereCast(target.transform.position, 0.5f, target.transform.forward, out RaycastHit hit, 100))
							{
								if (hit.collider.tag == tag)
								{
									// no damage is applied
                                    AudioSource.PlayClipAtPoint(target.abilities[1].soundHit, transform.position);
                                    Instantiate(shieldHitPFX, target.transform);
                                    blocked = true;
								}
							}
						}
					}
					// if player is not blocking, apply the damage
					if (blocked != true)
					{
                        foreach (Renderer r in c.GetComponentsInChildren<Renderer>())
                        {
                            r.material.SetColor("_EmissionColor", Color.black);
                        }

                        target.GetComponent<PlayerController>().enabled = false;
						abilities[0].Apply(this, target);
						break;
					}
				}
			}
		}
	}
	// animation event playing sound for all enemy attacks whether they hit or not
    public override void SoundEffect()
    {
        AudioSource.PlayClipAtPoint(abilities[0].sound, transform.position);
    }
	// randomly selects enemies in a list to summon and assigns them a spawn location
    public override void Summon()
    {
		// prefabs that can be summoned
        summonPrefabs.Add(GetComponent<SummonAction>().prefab1);
        summonPrefabs.Add(GetComponent<SummonAction>().prefab2);
        summonPrefabs.Add(GetComponent<SummonAction>().prefab5);
        summonPrefabs.Add(GetComponent<SummonAction>().prefab4);
        summonPrefabs.Add(GetComponent<SummonAction>().prefab3);

        List<GameObject> enemiesToSummon = new List<GameObject>();
        float value;
		// get the four enemies to summon
        for (int i = 0; i < 4; i++)
        {
            float bestValue = 0;
            foreach (GameObject o in summonPrefabs)
            {
				// calculates a random value
                value = o.GetComponent<Agent>().Calculate();
				// if higher than the best value, assign it as the new best value
                if (value > bestValue)
                {
                    bestValue = value;
					// if there is already an enemy in the list to summon
                    if (enemiesToSummon.Count > i)
                    {
						// remove the old best and replace with the new one
                        enemiesToSummon.RemoveAt(i);
                        enemiesToSummon.Add(o);
                    }
                    else
                    {
						// else add the best to the list
                        enemiesToSummon.Add(o);
                    }
                }
            }
        }
		// if the spawn locations are free
        if (SpawnLocation())
        {
            AudioSource.PlayClipAtPoint(GetComponent<SummonAction>().soundEffect, transform.position);
            int index = 0;
			// summon each enemy in the list
            foreach (GameObject o in enemiesToSummon)
            {
				GameObject.Find("enemyManager").GetComponent<enemyManager>().enemiesSpawned.Add(Instantiate(o, summonLocations[index].position, summonLocations[index].rotation));
                index++;
            }
        }
    }
	// returns a random number for summoning randomness
    public float Calculate()
    {
        return Random.Range(1, 200);
    }
	// determines whether the spawning locations are free from collisions, returns true no collision detected
    public bool SpawnLocation()
    {
		// spawn points for the summon
        summonLocations.Add(GetComponent<SummonAction>().spawnPoint1);
        summonLocations.Add(GetComponent<SummonAction>().spawnPoint2);
        summonLocations.Add(GetComponent<SummonAction>().spawnPoint3);
        summonLocations.Add(GetComponent<SummonAction>().spawnPoint4);

        foreach (Transform t in summonLocations)
        {
            Collider[] colEnemy = Physics.OverlapSphere(t.position, 0.3f, (1 << 8));
            Collider[] colPlayer = Physics.OverlapSphere(t.position, 0.3f, (1 << 9));
            if (colPlayer.Length > 0 || colEnemy.Length > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }
	// animation event for firing projectiles
    public override void FireProjectile(string name)
    {
		if (CompareTag("Skeleton Archer"))
		{
            AudioSource.PlayClipAtPoint(abilities[0].sound, transform.position);
            GameObject arrow = Instantiate(abilities[0].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation * Quaternion.Euler(0f, 180f, 0f));
			// sets the sender as the caster for applying damage later
			arrow.GetComponent<projectileMovement>().sender = gameObject;
		}
		if (CompareTag("Skeleton Caster"))
		{
            AudioSource.PlayClipAtPoint(abilities[0].sound, transform.position);
            GameObject orb = Instantiate(abilities[0].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
			// sets the sender as the caster for applying damage later
			orb.GetComponent<projectileMovement>().sender = gameObject;
		}
    }
	// animation event for firing the kings projectiles
	public override void FireKingProjectile()
	{
		float offset = 20f;
		GameObject orb = Instantiate(abilities[1].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
		GameObject orb2 = Instantiate(abilities[1].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation * Quaternion.Euler(0f, offset, 0f));
		GameObject orb3 = Instantiate(abilities[1].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation * Quaternion.Euler(0f, -offset, 0f));
		GameObject orb4 = Instantiate(abilities[1].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation * Quaternion.Euler(0f, -offset * 2, 0f));
		GameObject orb5 = Instantiate(abilities[1].objectEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation * Quaternion.Euler(0f, offset * 2, 0f));
		orb.GetComponent<projectileMovement>().sender = gameObject;
		orb2.GetComponent<projectileMovement>().sender = gameObject;
		orb3.GetComponent<projectileMovement>().sender = gameObject;
		orb4.GetComponent<projectileMovement>().sender = gameObject;
		orb5.GetComponent<projectileMovement>().sender = gameObject;

	}
	// animation event to reset the cooldown on base attacks
	public override void ResetCooldown()
	{
		GetComponent<RangedAction>().timer = GetComponent<RangedAction>().attackCooldown;
	}
	// visual debugging info
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, GetComponent<Agent>().aggroRange);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, GetComponent<FleeAction>().fleeRange);

		Gizmos.color = Color.green;
		Vector3 direction = (target.transform.position - transform.position).normalized;
		Gizmos.DrawWireSphere(transform.position + direction * 1.5f, 0.8f);
	}
}
