using Godot;

public abstract partial class WorldObjectPlacerObject : Node3D
{
	public abstract Godot.Vector3 positionOffset { get; }
}
