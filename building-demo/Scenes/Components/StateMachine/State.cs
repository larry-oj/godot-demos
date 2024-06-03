using Godot;

namespace buildingdemo.Scenes.Components.StateMachine;

public partial class State : Node
{
	[Signal]
	public delegate void TransitionedEventHandler(State state, State newState);
    
	protected Node2D Parent;

	public void Init(Node2D parent)
	{
		Parent = parent;
	}

	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Process(double delta) {}
	public virtual void PhysicsProcess(double delta) {}
}