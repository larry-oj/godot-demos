using Godot;
using Godot.Collections;

namespace buildingdemo.Resources.Buildings;

public partial class BuildingResource : Resource
{
	/// <summary>
	/// Scene of the building.
	/// </summary>
	[Export] public PackedScene Scene { get; protected set; }
	/// <summary>
	/// Atlas coordinates of the <b>normal tile</b>.
	/// </summary>
	[Export] public Vector2I AtlasCoordinates { get; protected set; }
	/// <summary>
	/// Dimensions of the building. Represented by an array of offset coordinates.
	/// </summary>
	[Export] public Array<Vector2I> Dimensions { get; protected set; }
}