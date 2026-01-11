using Godot;

public class MovementKeyboardMouse : Movement
{

    

    public MovementKeyboardMouse(Player player) : base(player){}


    
    public override void HandleInputEvent(InputEvent ev)
    {
        if (ev is InputEventMouseMotion eventMouseMotion)
		{
			RotateCharacterFacingMousePointer();
			
			
		}
    }
    public override void HandleProcess(double delta)
    {
        this.velocity = this.player.Velocity;

		this.ApplyGravity(delta);
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

		movement = movement.Rotated(Godot.Vector3.Up, this.characterBodyRotation.Y);


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

		this.UpdateVelocity();

		
    }
}