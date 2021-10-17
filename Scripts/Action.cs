using UnityEngine;

// abstract class used for actions performed by AI
public abstract class Action : MonoBehaviour
{
	// evaluates the action and returns a value of priority
	public abstract float Evaluate(Agent a);
	// constantly updates the current function
	public abstract void UpdateAction(Agent a);
	// run any special code upon entering a new action
	public abstract void Enter(Agent a);
	// run any special code upon exiting the current action
	public abstract void Exit(Agent a);
	
}
