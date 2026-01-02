using Godot;


public partial class Camera : Camera3D
{



	public override void _Ready()
	{
		CharacterBody3D player = (CharacterBody3D)GetParent();
		LookAt(player.GlobalPosition);
	}

}
