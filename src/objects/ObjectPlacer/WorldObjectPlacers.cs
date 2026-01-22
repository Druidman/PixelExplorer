using System.Collections.Generic;
using Godot;
public partial class WorldObjectPlacers : Node3D
{
	public bool isPlacingObject = false;

	WorldObjectPlacer currentObjectPlacerObject;

	[Export]
	public Godot.Collections.Dictionary<string, WorldObjectPlacer> worldObjects = new Godot.Collections.Dictionary<string, WorldObjectPlacer>
	{
	};

	[Export]
	World world;

	[Export]
	Player player;
	

	private void TurnOffObjectPlacerObject()
	{
		if (!isPlacingObject) return;

		this.currentObjectPlacerObject.Visible = false;
		this.currentObjectPlacerObject.ProcessMode = ProcessModeEnum.Disabled;

		this.isPlacingObject = false;
		this.currentObjectPlacerObject = null;
		this.player.canMove = true;
		this.player.isMouseButtonEventFree = true;
		
	
	}
	private void TurnOnObjectPlacerObject(WorldObjectPlacer currentObject)
	{
		if (isPlacingObject) return;

		this.currentObjectPlacerObject = currentObject;
		this.currentObjectPlacerObject.Visible = true;
		this.currentObjectPlacerObject.ProcessMode = ProcessModeEnum.Disabled; 

		this.isPlacingObject = true;
		this.player.canMove = false;
		this.player.isMouseButtonEventFree = false;
		

	}

	private bool PlaceObject()
	{
		
		return this.currentObjectPlacerObject.PlaceObject(this.world, this.player);
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (isPlacingObject && inputEvent is InputEventMouseMotion inputEventMouseMotion)
		{
			HandleObjectPlacing();
		}
		if (isPlacingObject && inputEvent is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsPressed())
			{
				if (PlaceObject())
				{
					TurnOffObjectPlacerObject();
				}
			}
		}
		if (inputEvent is InputEventKey inputEventKey)
		{
			if (inputEventKey.IsPressed() && !isPlacingObject)
			{
				foreach (string Key in this.worldObjects.Keys)
				{
					if (inputEventKey.IsActionPressed(Key))
					{
						TurnOnObjectPlacerObject(this.worldObjects[Key]);
						break;
					}
				}   
			}
			if (inputEventKey.IsReleased() && isPlacingObject)
			{
				TurnOffObjectPlacerObject();
			}
		}
	}

	private Godot.Vector3 GetMouseHitPos()
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		var cam = GetViewport().GetCamera3D();
		var mousePos = GetViewport().GetMousePosition();

		var origin = cam.ProjectRayOrigin(mousePos);
		var end = origin + cam.ProjectRayNormal(mousePos) * 1000; // TODO add normal length
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;

		var result = spaceState.IntersectRay(query);
		Godot.Vector3 hitPos = (Godot.Vector3)result.GetValueOrDefault("position");
		hitPos.Y -= 0.1f; // so that it would point to actual block later '
		return hitPos;
	}
	private void HandleObjectPlacing()
	{

		Godot.Vector3 hitPos = GetMouseHitPos();

		Chunk chunk = this.world.GetChunkAtPos(hitPos);
		if (chunk == null)
		{
			
			return;
		}

		WorldTile tile = chunk.GetTileAtPos((Godot.Vector3I)hitPos);
		if (tile == null)
		{
			return;
		}
		if (tile is not Block)
		{
			return;
		}
		
		
	
		currentObjectPlacerObject.GlobalPosition = this.world.GetTilePosition(hitPos) + new Godot.Vector3(0,0.5f,0) + currentObjectPlacerObject.PositionOffset;	
		
	}
}
