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

	bool isMoving = false;
	
	private float stopDistance = 2f;

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

	public void MoveAndSlide()
	{
		
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
			GD.Print("UP VELO!");
			GD.Print(velocity);
			velocity.Y = GameGlobals.PlayerJumpForce * 0.1f;
		}

		this.GlobalPosition += velocity;

		if (this.GlobalPosition.Y < -20){
			this.GlobalPosition = this.player.GlobalPosition + startOffsetPos; 
			velocity *= 0;
		}

	}

	public void Tick(float delta)
	{
		MoveAndSlide();
		this.Rotation = this.player.soldiersRotation;
		
		Godot.Vector3 destination = this.player.GlobalPosition + this.relativeToPlayer;
		Godot.Vector3 direction = (destination - this.GlobalPosition) * 0.5f;
		destination.Y = this.GlobalPosition.Y;

		if (this.GlobalPosition.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
		{
			direction *= 0;
			isMoving = false;
		}
		else
		{
			isMoving = true;
			direction *= GameGlobals.PlayerSpeed;
		}
		
		velocity.X = direction.X * delta;
		velocity.Z = direction.Z * delta;
		velocity.Y -= GameGlobals.GravitySpeed * delta;


		
		this.BottomRayCast.TargetPosition = new Godot.Vector3(0,velocity.Y,0);
		

	}

}
