using Godot;

public partial class Soldier : CharacterBody3D
{
	private Player player;

	private Godot.Vector3 relativeToPlayer;
	
	private float stopDistance = 0.2f;

	public void Initialize(Player player, Godot.Vector3 relativeToPlayer)
	{
		this.player = player;
		this.relativeToPlayer = relativeToPlayer;

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = Velocity;
		
		if (this.IsOnFloor() )
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= GameGlobals.GravitySpeed * (float)delta;
		}

		Godot.Vector3 destination = this.player.GlobalPosition + this.relativeToPlayer;
		Godot.Vector3 move = destination - this.GlobalPosition;
		move = move.Normalized();


		if ((this.GlobalPosition - destination).LengthSquared() > this.stopDistance * this.stopDistance)
		{
			velocity.X = move.X * GameGlobals.PlayerSpeed;

			velocity.Z = move.Z * GameGlobals.PlayerSpeed;
		}
		else
		{
			velocity.X = 0;

			velocity.Z = 0;
		}

		if (IsOnWall())
		{
			velocity.Y += 1;
		}

		Velocity = velocity;

		
		MoveAndSlide();
	}

}
