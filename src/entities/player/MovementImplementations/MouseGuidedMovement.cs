using Godot;

public class MouseGuidedMovement : Movement
{


    Godot.Vector3 direction;

    float distanceToMousePointer = 0.0f;
    float distanceToMousePointerSens = 0.1f;

    public MouseGuidedMovement(Player player) : base(player)
    {
        
    }

    public override void HandleInputEvent(InputEvent ev)
    {
        if (ev is InputEventMouseMotion evMouse)
        {
            RotateCharacterFacingMousePointer();

            var mousePointPos = GetMousePointInWorldPos();
            this.distanceToMousePointer = this.player.GlobalPosition.DistanceTo(new Godot.Vector3(mousePointPos.X,this.player.GlobalPosition.Y, mousePointPos.Y));
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

        this.player.speed = this.distanceToMousePointer * this.distanceToMousePointerSens;

        if (this.player.speed > GameGlobals.PlayerSpeed)
        {
            this.player.speed = GameGlobals.PlayerSpeed;
        }

        direction *= this.player.speed;
        
        velocity.X = direction.X;
        velocity.Z = direction.Z;
        this.UpdateVelocity();
    }
    
}