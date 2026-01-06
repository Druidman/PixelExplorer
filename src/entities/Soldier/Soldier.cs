using System.Diagnostics;
using Godot;

public partial class Soldier : Node3D
{

	[Export]
	RayCast3D BottomRayCast;
	[Export]
	ShapeCast3D ShapeCast;
	
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

	public void MoveAndSlide(double delta)
	{
		Vector3 motion = velocity * (float)delta;
		if (motion == Vector3.Zero)
			return;

		Vector3 localMotion =
			GlobalTransform.Basis.Inverse() * motion;

		ShapeCast.TargetPosition = localMotion;

		if (ShapeCast.IsColliding())
		{
			Vector3 normal = ShapeCast.GetCollisionNormal(0);
			motion = motion.Slide(normal);
		}

		GlobalPosition += motion;
	}

	// public void MoveAndSlide()
	// {
	// 	Godot.Vector3 motion = this.velocity;

		
		
	// 	this.ShapeCast.TargetPosition = velocity;
	// 	this.ShapeCast.ForceShapecastUpdate();

	// 	if (this.ShapeCast.IsColliding())
	// 	{
	// 		Godot.Vector3 normal= this.ShapeCast.GetCollisionNormal(0);
	// 		motion = motion.Slide(normal);
	// 	}

	// 	this.GlobalPosition += motion;





	// 	// this.BottomRayCast.TargetPosition = new Godot.Vector3(0,velocity.Y,0);

	// 	// this.BottomRayCast.ForceRaycastUpdate();
	// 	// if (this.BottomRayCast.IsColliding()){
	// 	// 	GlobalPosition = new Vector3(
	// 	// 		GlobalPosition.X,
	// 	// 		BottomRayCast.GetCollisionPoint().Y + GroundCheckOffset,
	// 	// 		GlobalPosition.Z
	// 	// 	);
	// 	// }

	// 	// if (this.GlobalPosition.Y < -20){
	// 	// 	this.GlobalPosition = this.player.GlobalPosition + startOffsetPos; 
	// 	// }

	// }

	public override void _PhysicsProcess(double delta)
	{
		Rotation = player.soldiersRotation;

		Vector3 destination = player.GlobalPosition + relativeToPlayer;
		Vector3 toTarget = destination - GlobalPosition;

		float sqrDist = toTarget.LengthSquared();

		if (sqrDist < stopDistance * stopDistance)
		{
			velocity = Vector3.Zero;
			isMoving = false;
		}
		else
		{
			isMoving = true;
			Vector3 dir = toTarget.Normalized();
			velocity = dir * GameGlobals.PlayerSpeed;
		}

		MoveAndSlide(delta);
	}


	// public override void _PhysicsProcess(double delta)
	// {
	// 	this.Rotation = this.player.soldiersRotation;
		
	// 	Godot.Vector3 destination = this.player.GlobalPosition + this.relativeToPlayer;
	// 	Godot.Vector3 direction = (destination - this.GlobalPosition) * 0.5f;

	// 	if (this.GlobalPosition.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
	// 	{
	// 		direction *= 0;
	// 		isMoving = false;
	// 	}
	// 	else
	// 	{
	// 		isMoving = true;
	// 		direction *= GameGlobals.PlayerSpeed;
	// 	}

	// 	velocity = direction;
		


		

	// 	MoveAndSlide();

	// }

}
