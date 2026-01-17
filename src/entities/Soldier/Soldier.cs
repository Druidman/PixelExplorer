using System.Diagnostics;
using Godot;

public partial class Soldier : Node3D
{



	private World world;
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
		this.world = this.player.world;

	}
	public override void _Ready()
	{
		this.GlobalPosition = this.player.GlobalPosition + startOffsetPos;
	}

	public void MoveAndSlide()
	{	

		Godot.Vector3 globalPos = this.GlobalPosition;
		Godot.Vector3I currentTilePosition = this.world.GetTilePosition(this.GlobalPosition);

		Godot.Vector3I bottomTilePosition = currentTilePosition;
		bottomTilePosition.Y -= 1;

		Godot.Vector3I topTilePosition = currentTilePosition;
		topTilePosition.Y += 1;
		
		// we have tiles positions

		if (!this.world.CheckIfFreeSpace(currentTilePosition))
		// tile we are in is occupied 
		{
			globalPos.Y = topTilePosition.Y;
			this.velocity.Y = 0;
		}

		if (!this.world.CheckIfFreeSpace(bottomTilePosition))
		// under us there is a tile so we won't apply gravity
		{
			this.velocity.Y = 0;
			globalPos.Y = currentTilePosition.Y;
		}
		globalPos += velocity;
		if (globalPos.Y < GameGlobals.StartWorldMiddle.Y)
		{
			globalPos = this.player.GlobalPosition + this.startOffsetPos;
	
		}

		this.GlobalPosition = globalPos;
	}

	public void Tick(float delta, Godot.Vector3 rotation)
	{
		MoveAndSlide();
		this.Rotation = rotation;
		
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
		if (velocity.Y <= -1)
		{
			velocity.Y = -1;
		}
		else {
			velocity.Y -= GameGlobals.GravitySpeed * delta;	
		}

	}

}
