using Godot;

public class MouseGuidedMovement : Movement
{


    Godot.Vector3 direction;

    public MouseGuidedMovement(Player player) : base(player)
    {
        
    }

    public override void HandleInputEvent(InputEvent ev)
    {
        if (ev is InputEventMouseMotion evMouse)
        {
            RotateCharacterFacingMousePointer();
        }
    }
    public override void HandleProcess(double delta)
    {
        this.ApplyGravity(delta);


        if (!this.player.canMove)
        {
            velocity.X = 0;
            velocity.Z = 0;
            this.UpdateVelocity();
            return;
        }


        if (Input.IsActionJustPressed("move_up") && this.player.IsOnFloor())
		{
			velocity.Y = GameGlobals.PlayerJumpForce;
		}

        direction = new Godot.Vector3(1,0,0).Rotated(Godot.Vector3.Up, this.characterBodyRotation.Y);

        direction *= GameGlobals.PlayerSpeed;
        velocity.X = direction.X;
        velocity.Z = direction.Z;
        this.UpdateVelocity();
    }
    
}