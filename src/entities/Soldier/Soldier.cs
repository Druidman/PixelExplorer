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

	
	
	private float stopDistance = 0.2f;

	Godot.Vector3 startOffsetPos = new Godot.Vector3(0,10,0);

	public void Initialize(Player player, Godot.Vector3 relativeToPlayer)
	{
		this.player = player;
		this.relativeToPlayer = relativeToPlayer;

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition + startOffsetPos;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.Rotation = this.player.soldiersRotation;
		
		this.BottomRayCast.TargetPosition = this.BottomRayCast.TargetPosition + (
			new Godot.Vector3(0,velocity.Y - (GameGlobals.GravitySpeed * (float)delta),0) * (float)delta
		);
		this.BottomRayCast.ForceRaycastUpdate();
		bool isGroundUnderNextFrame = this.BottomRayCast.IsColliding();

		if (isGroundUnderNextFrame)
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= GameGlobals.GravitySpeed * (float)delta;
		}
		
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
		

		velocity.X = direction.X;
		velocity.Z = direction.Z;


		this.LeftRayCast.ForceRaycastUpdate();
		this.RightRayCast.ForceRaycastUpdate();
		
		this.LeftRayCast.TargetPosition = this.LeftRayCast.TargetPosition + (new Godot.Vector3(velocity.X, 0, velocity.Z) * (float)delta);
		this.RightRayCast.TargetPosition = this.RightRayCast.TargetPosition + (new Godot.Vector3(velocity.X, 0, velocity.Z) * (float)delta);

		bool isWallInFrontNextFrame = this.LeftRayCast.IsColliding() || this.RightRayCast.IsColliding();

		if (isWallInFrontNextFrame && (velocity.X != 0 || velocity.Z != 0))
		{
			velocity.Y += GameGlobals.PlayerJumpForce ;
		}

		this.GlobalPosition += velocity * (float)delta;


		if (isGroundUnderNextFrame)
		{
			this.GlobalPosition = new Godot.Vector3(this.GlobalPosition.X,this.BottomRayCast.GetCollisionPoint().Y + 0.1f,this.GlobalPosition.Z);
		}


		if (this.GlobalPosition.Y < -20)
		{
			this.GlobalPosition = this.player.GlobalPosition + this.startOffsetPos;
		}
		


	}

}
