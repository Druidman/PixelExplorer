using Godot;


public partial class Camera : Camera3D
{
	float angle = 90.0f;
	Vector3 startPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startPosition = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateY(Mathf.DegToRad(angle * (float)delta));
		GlobalPosition = startPosition.Rotated(Vector3.Up, Rotation.Y);

	}
}
