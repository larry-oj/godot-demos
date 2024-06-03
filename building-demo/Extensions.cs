using Godot;

namespace buildingdemo;

public static class Extensions
{
    public static Vector2I GetMouseTile(this TileMap tileMap)
    {
        return tileMap.LocalToMap(tileMap.GetLocalMousePosition());
    }

    public static Vector2 MapToGlobal(this TileMap tileMap, Vector2I map)
    {
        return tileMap.ToGlobal(tileMap.MapToLocal(map));
    }
}