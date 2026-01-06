using System.Diagnostics;
using Godot;

public partial class Soldier : Area3D
{

	[Export]
	RayCast3D BottomRayCast;
	[Export]
	RayCast3D LeftRayCast;
	[Export]
	RayCast3D RightRayCast;
	private Player player;

	private Godot.Vector2 relativeToPlayer;
	Godot.Vector3 destination;
	Godot.Vector3 move;

	Godot.Vector3 velocity;
	float GroundCheckOffset = 0.1f;

	bool isMoving = false;
	
	private float stopDistance = 2f;

	Godot.Vector3 startOffsetPos = new Godot.Vector3(0,2,0);

	bool ground = false;
	public void Initialize(Player player, Godot.Vector3 relativeToPlayer)
	{
		this.player = player;
		this.relativeToPlayer = new Godot.Vector2(relativeToPlayer.X,relativeToPlayer.Z);

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition + this.startOffsetPos;
		this.velocity.Y = -GameGlobals.GravitySpeed;
	}

	public void MoveAndSlide()
	{

		this.BottomRayCast.TargetPosition = new Godot.Vector3(0,velocity.Y,0);

		this.BottomRayCast.ForceRaycastUpdate();
		this.LeftRayCast.ForceRaycastUpdate();
		this.RightRayCast.ForceRaycastUpdate();
		
		bool isGroundUnder = this.BottomRayCast.IsColliding();
		bool isWallInFront = this.LeftRayCast.IsColliding() || this.RightRayCast.IsColliding();

		if (isGroundUnder){
			GlobalPosition = new Vector3(
				GlobalPosition.X,
				BottomRayCast.GetCollisionPoint().Y + GroundCheckOffset,
				GlobalPosition.Z
			);
			velocity.Y = 0;
		}

		if (isWallInFront && isMoving)
		{
			velocity.Y = GameGlobals.PlayerJumpForce * 0.1f;
		}

		this.GlobalPosition += velocity;

		if (this.GlobalPosition.Y < -20){
			this.GlobalPosition = this.player.GlobalPosition + startOffsetPos; 
		}

	}


	public override void _PhysicsProcess(double delta)
	{
		this.Rotation = this.player.soldiersRotation;
		
		Godot.Vector2 destination = new Godot.Vector2(this.player.GlobalPosition.X,this.player.GlobalPosition.Z) + this.relativeToPlayer;

		Godot.Vector2 global = new Godot.Vector2(this.GlobalPosition.X,this.GlobalPosition.Z);
		Godot.Vector2 direction = (destination - global) * 0.5f;

		if (global.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
		{
			direction *= 0;
			isMoving = false;
		}
		else
		{
			isMoving = true;
			direction *= GameGlobals.PlayerSpeed;
		}
		
		velocity.X = direction.X;
		velocity.Z = direction.Y;
		// velocity.X = direction.X * (float)delta;
		// velocity.Z = direction.Z * (float)delta;
		// velocity.Y -= GameGlobals.GravitySpeed * (float)delta;

		this.GlobalPosition += velocity * (float)delta;

		
		

		// MoveAndSlide();

	}

	public void OnBodyEntered(Node3D body)
	{
		if (this.ground)
		{
			this.GlobalPosition = new Godot.Vector3(this.GlobalPosition.X,(int)this.GlobalPosition.Y + 1.0f,this.GlobalPosition.Z);
			this.velocity.Y = 0;
		}
		
	}
	public void OnBodyExited(Node3D body)
	{
		
		this.velocity.Y = -GameGlobals.GravitySpeed;
	}

}
