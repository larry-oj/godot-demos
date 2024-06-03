using System;
using System.Collections.Generic;
using System.Linq;
using buildingdemo.Resources.Buildings;
using buildingdemo.Scenes.Buildings;
using Godot;
using Godot.Collections;

namespace buildingdemo.Scenes;

public partial class ObjectPlacing : Node2D
{
	// nodes
	private TileMap _tileMap;
	private Camera2D _camera;

	// exports
	[Export] private Array<BuildingResource> _buildings;
	
	// cursor
	private int _selectedBuilding = 0;
	private Vector2I _lastSelectedTile;
	
	// data
	private Array<Building> _placedBuildings = new();
	
	public override void _Ready()
	{
		_tileMap = GetNode<TileMap>("%TileMap");
		_camera = GetNode<Camera2D>("Camera");
	}
	
	public override void _Process(double delta)
	{
		var flag = false;
		var selectedTile = _tileMap.GetMouseTile();

		HandleInput(selectedTile, ref flag);
		
		if (_lastSelectedTile == selectedTile && !flag) return;
		_tileMap.EraseCell(2, _lastSelectedTile);
		_lastSelectedTile = selectedTile;
		_tileMap.SetCell(2, _lastSelectedTile, 2, _buildings[_selectedBuilding].AtlasCoordinates);
	}

	private void HandleInput(Vector2I selectedTile, ref bool flag)
	{
		TryNext(selectedTile, ref flag);
		TryPrev(selectedTile, ref flag);
		TryPlace(selectedTile, ref flag);
		TryRemove(selectedTile, ref flag);
	}

	private void TryNext(Vector2I selectedTile, ref bool flag)
	{
		if (!Input.IsActionJustPressed("next_item")) return;
		
		_selectedBuilding++;
		if (_selectedBuilding == _buildings.Count)
			_selectedBuilding = 0;
		flag = true;
	}
	
	private void TryPrev(Vector2I selectedTile, ref bool flag)
	{
		if (!Input.IsActionJustPressed("prev_item")) return;
		
		_selectedBuilding--;
		if (_selectedBuilding < 0)
			_selectedBuilding = _buildings.Count - 1;
		flag = true;
	}
	
	private void TryPlace(Vector2I selectedTile, ref bool flag)
	{
		if (!Input.IsActionJustPressed("place")) return;
		if (!IsFreeSpace(selectedTile, _buildings[_selectedBuilding].Dimensions)) return;

		PlaceBuilding(selectedTile);
	}
	
	private void TryRemove(Vector2I selectedTile, ref bool flag)
	{
		if (!Input.IsActionJustPressed("remove")) return;

		var building = GetBuilding(selectedTile);
		if (building == null) return;
		
		_placedBuildings.Remove(building);
		building.QueueFree();
	}

	private void PlaceBuilding(Vector2I selectedTile)
	{
		var building = _buildings[_selectedBuilding].Scene.Instantiate<Building>();
		building.GlobalPosition = _tileMap.MapToGlobal(selectedTile);
		building.TakeSpace(selectedTile, _buildings[_selectedBuilding].Dimensions);
		_placedBuildings.Add(building);
		_tileMap.CallDeferred(Node.MethodName.AddChild, building);
	}
	
	private bool IsFreeSpace(Vector2I tile, Array<Vector2I> dimensions)
	{
		var toOccupy = new Array<Vector2I>();
		foreach (var d in dimensions)
		{
			toOccupy.Add(tile + d);
		}

		return _placedBuildings
			.SelectMany(building => building.OccupiedCoordinates)
			.All(occupiedTile => !toOccupy.Contains(occupiedTile));
	}
	
	private Building GetBuilding(Vector2I tile)
	{
		return _placedBuildings.FirstOrDefault(building => building.OccupiedCoordinates.Contains(tile));
	}
}	