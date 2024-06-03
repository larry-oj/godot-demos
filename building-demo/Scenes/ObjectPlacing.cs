using System;
using System.Collections.Generic;
using System.Linq;
using buildingdemo.Resources.Buildings;
using buildingdemo.Scenes.Buildings;
using buildingdemo.Scenes.Components.StateMachine;
using Godot;
using Godot.Collections;

namespace buildingdemo.Scenes;

public partial class ObjectPlacing : Node2D
{
	// nodes
	public TileMap TileMap { get; private set; }
	public Camera2D Camera { get; private set; }
	public Label ModeLabel { get; private set; }
	
	private StateMachine _stateMachine;
	
	// data
	public Array<Scenes.Buildings.Building> PlacedBuildings = new();
	
	public override void _Ready()
	{
		TileMap = GetNode<TileMap>("%TileMap");
		Camera = GetNode<Camera2D>("%Camera");
		ModeLabel = GetNode<Label>("%ModeLabel");
		_stateMachine = GetNode<StateMachine>("%StateMachine");
		
		_stateMachine.Init(this);
	}
	
	public override void _Process(double delta)
	{
		_stateMachine.Process(delta);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_stateMachine.PhysicsProcess(delta);
	}
}	