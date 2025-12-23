using Godot;
using System;


public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float DecelerationSpeed = Speed * 0.1f;
	public const float JumpForce = 10;
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;
	MeshInstance3D character;
	CollisionShape3D characterCollider;
	Camera camera;

	Godot.Vector3 destiny;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Confined;
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera3D");

		destiny = this.Position;

		
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("exit"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if (inputEvent.IsPressed() && inputEvent is InputEventMouseButton)
		{
			
			Godot.Vector2 mousePos = GetViewport().GetMousePosition(); 

			Godot.Vector3 origin = this.camera.ProjectRayOrigin(mousePos);
			Godot.Vector3 end = origin + this.camera.ProjectRayNormal(mousePos) * 1000;


			PhysicsDirectSpaceState3D space_state = this.GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
			var result = space_state.IntersectRay(query);
			if (result.Count > 0)
			{
				var hitPos = (Godot.Vector3)result["position"];	

				var angle = new Vector2(this.Position.X, this.Position.Z).AngleToPoint(new Vector2(hitPos.X, hitPos.Z));
				this.character.RotateY(-(angle + this.character.Rotation.Y));
				this.characterCollider.RotateY(-(angle + this.character.Rotation.Y));
				

				


				this.destiny = hitPos;
				
				
				
			}
			
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Implement path finder
		
		MoveAndSlide();

	}
}
