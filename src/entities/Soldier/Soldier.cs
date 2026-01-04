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

	Godot.Vector3 rotationOffset = new Godot.Vector3(0,Mathf.DegToRad(90),0);
	
	private float stopDistance = 0.2f;

	public void Initialize(Player player, Godot.Vector3 relativeToPlayer)
	{
		this.player = player;
		this.relativeToPlayer = relativeToPlayer;

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition + new Godot.Vector3(0,10,0);
	}

	public override void _PhysicsProcess(double delta)
	{
		this.Rotation = this.player.characterCollider.Rotation - rotationOffset;
		
		this.BottomRayCast.ForceRaycastUpdate();
		bool isGroundUnder = this.BottomRayCast.IsColliding();

		if (isGroundUnder)
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

		this.GlobalPosition += velocity * (float)delta;


		if (isGroundUnder && velocity.Y == 0)
		{
			this.GlobalPosition = new Godot.Vector3(this.GlobalPosition.X,this.BottomRayCast.GetCollisionPoint().Y + 0.1f,this.GlobalPosition.Z);
		}

		this.LeftRayCast.ForceRaycastUpdate();
		this.RightRayCast.ForceRaycastUpdate();

		bool isWallInFront = this.LeftRayCast.IsColliding() || this.RightRayCast.IsColliding();

		

		if (isWallInFront && (velocity.X != 0 || velocity.Z != 0))
		{
			velocity.Y += GameGlobals.PlayerJumpForce * 5;
			
		}
		


	}

}
