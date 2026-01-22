using System.Diagnostics;
using Godot;

public partial class Soldier : Node3D
{

	static float strength = 0.5f;

	private World world;
	private Player player;

	public Godot.Vector3 relativeToPlayer;

	Godot.Vector3 velocity;
	public Godot.Vector3 destination;
	
	private float stopDistance = 2f;

	public Building destroyObjective = null;

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
		bottomTilePosition.Y -= GameGlobals.TileWidth;

		Godot.Vector3I topTilePosition = currentTilePosition;
		topTilePosition.Y += GameGlobals.TileWidth;
		
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
		UpdateActions();
		MoveAndSlide();
		this.Rotation = rotation;
		Godot.Vector3 direction = (destination - this.GlobalPosition) * 0.5f;
		direction = direction.Normalized();
		destination.Y = this.GlobalPosition.Y;

		if (this.GlobalPosition.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
		{
			direction *= 0;
		}
		else
		{
			direction *= GameGlobals.PlayerSpeed;
		}
		
		velocity.X = direction.X * delta;
		velocity.Z = direction.Z * delta;
		if (velocity.Y <= -GameGlobals.TileWidth)
		{
			velocity.Y = -GameGlobals.TileWidth;
		}
		else {
			velocity.Y -= GameGlobals.GravitySpeed * delta;	
		}

	}

	public void UpdateActions()
	{
		if (this.GlobalPosition.DistanceSquaredTo(destination) <= 25 && IsInstanceValid(destroyObjective)) // squared 5
		{
			this.destroyObjective?.TakeHealth(Soldier.strength);
		}
		
	}
}
