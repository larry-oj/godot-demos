using buildingdemo.Scenes.Components.StateMachine;
using Godot;

namespace buildingdemo.Scenes;

public partial class Game : State
{
	[Export] private State _buildingState;

	public override void Enter()
	{
		((ObjectPlacing)Parent).ModeLabel.Text = this.Name;
	}
	
	public override void Process(double delta)
	{
		if (!Input.IsActionJustPressed("toggle_building")) return;
		EmitSignal(State.SignalName.Transitioned, this, _buildingState);
	}
}