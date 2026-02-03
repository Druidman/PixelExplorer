using Godot;

public abstract class Movement
{
    protected Player player = null;
    protected Godot.Vector3 velocity;
    protected Movement(Player player)
    {
        this.player = player;
    }

    public Godot.Vector3 characterBodyRotation = new Godot.Vector3();
    protected void RotateCharacterBody(float angle)
	{
		this.player.character.RotateY(angle);
		this.player.characterCollider.RotateY(angle);

		this.characterBodyRotation = this.player.character.Rotation;
	}
    protected void RotateCharacterFacingMousePointer(){
		

		var mousePointPos = GetMousePointInWorldPos();
		var Player2DPos = new Godot.Vector2(this.player.Position.X, this.player.Position.Z);

		var angle = Player2DPos.AngleToPoint(mousePointPos);
		RotateCharacterBody(-(angle + this.player.character.Rotation.Y));
		
	}

	public Godot.Vector2 GetMousePointInWorldPos(){
		Godot.Vector2 mousePos = this.player.GetViewport().GetMousePosition(); 

		var Player2DPos = new Godot.Vector2(this.player.Position.X, this.player.Position.Z);
		

		return Player2DPos + (mousePos - (DisplayServer.WindowGetSize() / 2));
	}

    protected void ApplyGravity(double delta)
    {
        // Add the gravity.
		if (!GameGlobals.DebugMode)
		{	
			
			if (this.player.IsOnFloor() )
			{
				velocity.Y = 0;
			}
			else
			{
				velocity.Y -= GameGlobals.GravitySpeed * (float)delta;
			}
		}
    }

    protected void UpdateVelocity()
    {
        this.player.Velocity = velocity;
    }
    
    public abstract void HandleInputEvent(InputEvent ev);
    public abstract void HandleProcess(double delta);
}