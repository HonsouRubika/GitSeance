using UnityEngine;

[RequireComponent(typeof(MonoStateMachine))]
public abstract class MonoState : MonoBehaviour
{
	[SerializeField] string _stateName;
	public string StateName { get { return _stateName; } }

	public virtual void OnStateEnter() { }
	public virtual void OnStateUpdate() { }
	public virtual void OnStateLateUpdate() { }
	public virtual void OnStateFixedUpdate() { }
	public virtual void OnStateExit() { }
}