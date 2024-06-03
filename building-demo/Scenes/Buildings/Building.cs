using Godot;
using Godot.Collections;

namespace buildingdemo.Scenes.Buildings;

public partial class Building : StaticBody2D
{
    /// <summary>
    /// Space taken by the building.
    /// </summary>
    public Array<Vector2I> OccupiedCoordinates { get; protected set; } = new();
    
    public void TakeSpace(Vector2I coordinates, Array<Vector2I> dimensions)
    {
        foreach (var dimension in dimensions)
        {
            OccupiedCoordinates.Add(coordinates + dimension);
        }
    }

    public void FreeSpace()
    {
        OccupiedCoordinates.Clear();
    }
}