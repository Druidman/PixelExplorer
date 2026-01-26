using Godot;

public partial class AttackPlayerAction : PlayerAction
{
	[Export]
	MeshInstance3D arrow;

	public override void OnEnd()
	{
		this.player.soldierManager.SetDestroyObjective(null);
		this.arrow.Visible = false;
	}
	public override void OnStart()
	{
		
	}
	public override void HandleInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton inputEventMouseButton)
		{
			var spaceState = GetWorld3D().DirectSpaceState;
			var cam = GetViewport().GetCamera3D();
			var mousePos = GetViewport().GetMousePosition();

			var origin = cam.ProjectRayOrigin(mousePos);
			var end = origin + cam.ProjectRayNormal(mousePos) * 1000; // TODO add normal length
			var query = PhysicsRayQueryParameters3D.Create(origin, end);
			query.CollideWithAreas = true;

			var result = spaceState.IntersectRay(query);	
			Godot.GodotObject godotObject = (Godot.GodotObject)result["collider"];
			if (godotObject is IBuilding buildingObject)
			{
				this.player.soldierManager.SetDestroyObjective(buildingObject); // TODO
				arrow.GlobalPosition = buildingObject.GlobalPosition + new Godot.Vector3(0,5,0);
				arrow.Visible = true;
				
			}
			else
			{
				this.player.soldierManager.SetDestroyObjective(null);
				arrow.Visible = false;
			}

		}
	}

	public override void Update(double delta)
	{
		
	}
}
