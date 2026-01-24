using Godot;

public partial class AttackPlayerAction : PlayerAction
{
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
			if (godotObject is Building buildingObject)
			{
				this.player.soldierManager.SetDestroyObjective(buildingObject); // TODO
				
			}
			else
			{
				this.player.soldierManager.SetDestroyObjective(null);
			}

		}
	}

	public override void Update(double delta)
	{
		
	}
}
