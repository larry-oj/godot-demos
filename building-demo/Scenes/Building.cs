using System.Linq;
using buildingdemo.Resources.Buildings;
using buildingdemo.Scenes.Components.StateMachine;
using Godot;
using Godot.Collections;

namespace buildingdemo.Scenes;

public partial class Building : State
{
	// exports
	[Export] private State _gameState;
	[Export] private Array<BuildingResource> _buildings;
	private ObjectPlacing _parent;
	
	// cursor
	private int _selectedBuilding = 0;
	private Vector2I _lastSelectedTile;

	public override void Enter()
	{
		_parent = Parent as ObjectPlacing;
		_parent!.ModeLabel.Text = this.Name;
	}
	
	public override void Process(double delta)
	{
		var flag = false;
		var selectedTile = _parent.TileMap.GetMouseTile();

		if (Input.IsActionJustPressed("toggle_building"))
		{
			_parent.TileMap.EraseCell(2, _lastSelectedTile);
			EmitSignal(State.SignalName.Transitioned, this, _gameState);
			return;
		}
		HandleInput(selectedTile, ref flag);
		
		if (_lastSelectedTile == selectedTile && !flag) return;
		_parent.TileMap.EraseCell(2, _lastSelectedTile);
		_lastSelectedTile = selectedTile;
		_parent.TileMap.SetCell(2, _lastSelectedTile, 2, _buildings[_selectedBuilding].AtlasCoordinates);
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
		
		_parent.PlacedBuildings.Remove(building);
		building.QueueFree();
	}

	private void PlaceBuilding(Vector2I selectedTile)
	{
		var building = _buildings[_selectedBuilding].Scene.Instantiate<Scenes.Buildings.Building>();
		building.GlobalPosition = _parent.TileMap.MapToGlobal(selectedTile);
		building.TakeSpace(selectedTile, _buildings[_selectedBuilding].Dimensions);
		_parent.PlacedBuildings.Add(building);
		_parent.TileMap.CallDeferred(Node.MethodName.AddChild, building);
	}
	
	private bool IsFreeSpace(Vector2I tile, Array<Vector2I> dimensions)
	{
		var toOccupy = new Array<Vector2I>();
		foreach (var d in dimensions)
		{
			toOccupy.Add(tile + d);
		}

		return _parent.PlacedBuildings
			.SelectMany(building => building.OccupiedCoordinates)
			.All(occupiedTile => !toOccupy.Contains(occupiedTile));
	}
	
	private Scenes.Buildings.Building GetBuilding(Vector2I tile)
	{
		return _parent.PlacedBuildings.FirstOrDefault(building => building.OccupiedCoordinates.Contains(tile));
	}
}