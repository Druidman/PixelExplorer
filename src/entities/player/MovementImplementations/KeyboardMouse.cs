using Godot;

public class MovementKeyboardMouse : Movement
{

    private Godot.Vector3 characterBodyRotation = new Godot.Vector3();

    public MovementKeyboardMouse(Player player) : base(player){}


    private void RotateCharacterBody(float angle)
	{
		this.player.character.RotateY(angle);
		this.player.characterCollider.RotateY(angle);

		this.characterBodyRotation = this.player.character.Rotation;
	}
    private void RotateCharacterFacingMousePointer(){
		Godot.Vector2 mousePos = this.player.GetViewport().GetMousePosition(); 

		var Player2DPos = new Godot.Vector2(this.player.Position.X, this.player.Position.Z);
		var mousePointPos = Player2DPos + (mousePos - (DisplayServer.WindowGetSize() / 2));

		var angle = Player2DPos.AngleToPoint(mousePointPos);
		RotateCharacterBody(-(angle + this.player.character.Rotation.Y));
		
	}
    public override void HandleInputEvent(InputEvent ev)
    {
        if (ev is InputEventMouseMotion eventMouseMotion)
		{
			RotateCharacterFacingMousePointer();
			
			
		}
    }
    public override void HandleProcess(double delta)
    {
        Vector3 velocity = this.player.Velocity;

		
		// Add the gravity.
		if (!GameGlobals.DebugMode)
		{	
			// Area3D area = GetNode<Area3D>("Area");
			// GD.Print(area.GetOverlappingBodies().Count);
			if (this.player.IsOnFloor() )//|| area.GetOverlappingBodies().Count > 1)
			{
				velocity.Y = 0;
			}
			else
			{
				velocity.Y -= GameGlobals.GravitySpeed * (float)delta;
			}
		}
		
		Godot.Vector3 movement = new Godot.Vector3(0.0f,0.0f, 0.0f);
		

		// Handle Jump.
		if (Input.IsActionJustPressed("move_up") && this.player.IsOnFloor())
		{
			velocity.Y = GameGlobals.PlayerJumpForce;
		}

		
		if (Input.IsActionPressed("move_forward"))
		{
			movement.X += 1.0f;
		}
		if (Input.IsActionPressed("move_backward"))
		{
			movement.X += -1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			movement.Z += -1.0f;
		}
		if (Input.IsActionPressed("move_right"))
		{
			movement.Z += 1.0f;
		}
		movement *= GameGlobals.PlayerSpeed;

		movement = movement.Normalized();

		movement = movement.Rotated(Godot.Vector3.Up, this.player.character.Rotation.Y);


		if (movement.Z != 0.0f)
		{
			velocity.Z = movement.Z* GameGlobals.PlayerSpeed;
		}
		else
		{
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, GameGlobals.PlayerDecelerationSpeed);
		}

		if (movement.X != 0.0f)
		{
			velocity.X = movement.X * GameGlobals.PlayerSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, GameGlobals.PlayerDecelerationSpeed);
		}

		this.player.Velocity = velocity;
    }
}