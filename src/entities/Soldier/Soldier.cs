using System.Diagnostics;
using Godot;

public partial class Soldier : Node3D
{

	[Export]
	RayCast3D BottomRayCast;
	[Export]
	RayCast3D LeftRayCast;
	[Export]
	RayCast3D RightRayCast;
	private Player player;

	private Godot.Vector3 relativeToPlayer;
	Godot.Vector3 destination;
	Godot.Vector3 move;

	Godot.Vector3 velocity;
	float GroundCheckOffset = 0.1f;

	
	
	private float stopDistance = 1.5f;

	Godot.Vector3 startOffsetPos = new Godot.Vector3(0,10,0);


	Godot.Vector3 LeftRayCastTarget;
	Godot.Vector3 RightRayCastTarget;
	public void Initialize(Player player, Godot.Vector3 relativeToPlayer)
	{
		this.player = player;
		this.relativeToPlayer = relativeToPlayer;

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition + startOffsetPos;

		this.LeftRayCastTarget = this.LeftRayCast.TargetPosition;
		this.RightRayCastTarget = this.RightRayCast.TargetPosition;
	}

	public void MoveAndSlide()
	{

		this.BottomRayCast.TargetPosition = new Godot.Vector3(0,velocity.Y,0);
		// this.LeftRayCast.TargetPosition = this.LeftRayCastTarget + new Godot.Vector3(velocity.X,0,velocity.Z);
		// this.RightRayCast.TargetPosition = this.RightRayCastTarget + new Godot.Vector3(velocity.X,0,velocity.Z);; 

		this.BottomRayCast.ForceRaycastUpdate();
		this.LeftRayCast.ForceRaycastUpdate();
		this.RightRayCast.ForceRaycastUpdate();
		
		bool isGroundUnder = this.BottomRayCast.IsColliding();
		bool isWallInFront = this.LeftRayCast.IsColliding() || this.RightRayCast.IsColliding();

		// Godot.Vector3 pos = this.GlobalPosition;
		if (isGroundUnder){
			Vector3 point = BottomRayCast.GetCollisionPoint();
			GlobalPosition = new Vector3(
				GlobalPosition.X,
				point.Y + GroundCheckOffset,
				GlobalPosition.Z
			);
			velocity.Y = 0;
			// velocity.Y = this.BottomRayCast.GetCollisionPoint().Y - this.GlobalPosition.Y + 0.1f;
		}

		if (isWallInFront && (this.velocity.X != 0 || this.velocity.Z != 0))
		{
			velocity.Y = GameGlobals.PlayerJumpForce * 0.2f;
		}

		this.GlobalPosition += velocity;

	}


	public override void _PhysicsProcess(double delta)
	{
		this.Rotation = this.player.soldiersRotation;
		
		Godot.Vector3 destination = this.player.GlobalPosition + this.relativeToPlayer;
		Godot.Vector3 direction = (destination - this.GlobalPosition).Normalized();

		if (this.GlobalPosition.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
		{
			direction *= 0;
		}
		else
		{
			direction *= GameGlobals.PlayerSpeed;
		}
		

		velocity.X = direction.X * (float)delta;
		velocity.Z = direction.Z * (float)delta;
		velocity.Y -= GameGlobals.GravitySpeed * (float)delta;

		MoveAndSlide();

	}

}
