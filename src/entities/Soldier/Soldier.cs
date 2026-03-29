using System;
using System.Diagnostics;
using Godot;

public partial class Soldier : Area3D
{
	[Export]
	Godot.Timer attackTimer;

	bool hasCustomDestination = false;
	Godot.Vector3 customDestination = default(Godot.Vector3);
	
	static float strength = 2;

	public float Health {get; private set;} = 2;

	private World world;
	public Player player;

	public Godot.Vector3 relativeToPlayer;

	Godot.Vector3 velocity;
	
	private float stopDistance = 2f;

	public IBuilding destroyObjective = null;

	Godot.Vector3 startOffsetPos = new Godot.Vector3(0,10,0);
	Godot.Vector3 startingGlobalPos = default(Godot.Vector3);
	Action onRemove = ()=>{};
	public void Initialize(Player player, Godot.Vector3 relativeToPlayer, World world = null, Godot.Vector3 startingGlobalPos = default(Godot.Vector3), Action onRemove = default(Action))
	{
		this.player = player;
		this.relativeToPlayer = relativeToPlayer;
		this.startingGlobalPos = startingGlobalPos;
		this.onRemove = onRemove;
		if (world != null)
		{
			this.world = world;
		}
		else
		{
			this.world = this.player.world;	
		}
	}

	public void TakeHealth(float delta)
	{
		if (delta < 0)
		{
			return;
		}

		this.Health -= delta;
		if (this.Health <= 0){
			this.Kill();
		}
	}

	private void Kill()
	{
		if (onRemove != default(Action))
		{
			onRemove();	
		}
		
		if (this.player != null) {
			this.player.soldierManager.RemoveSoldier(this);
			if (GameGlobals.dieSound != null) {
				GameGlobals.dieSound.Stop(); // funny effect
				GameGlobals.dieSound.Play();
			};
		}
		
		GetParent()?.RemoveChild(this);
		QueueFree();
	}
	public override void _Ready()
	{
		if (this.player != null) this.GlobalPosition = this.player.GlobalPosition + startOffsetPos;
		else this.GlobalPosition = this.startingGlobalPos + startOffsetPos;

		
		
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
		else if (!this.world.CheckIfFreeSpace(bottomTilePosition))
		// under us there is a tile so we won't apply gravity
		{
			this.velocity.Y = 0;
			globalPos.Y = currentTilePosition.Y;
		}
		globalPos += velocity;
		if (globalPos.Y < GameGlobals.StartWorldMiddle.Y)
		{
			if (this.player == null)
			{
				globalPos = this.GlobalPosition + this.startOffsetPos;
			}
			else
			{
				globalPos = this.player.GlobalPosition + this.startOffsetPos;
			}
	
		}

		this.GlobalPosition = globalPos;
	}

	public void Tick(float delta, Godot.Vector3 rotation)
	{
		UpdateActions();
		MoveAndSlide();
		this.Rotation = rotation;
		Godot.Vector3 destination;
		if (destroyObjective == null)
		{
			if (player == null)
			{
				// so this is enemy
				if (!hasCustomDestination)
				{
					customDestination = this.world.GetRandomBlockPosInWorld();
					hasCustomDestination = true;	
				}
				destination = customDestination;
				
			}
			else {
				hasCustomDestination = false;
				destination = this.player.GlobalPosition + relativeToPlayer;	
			}
		}
		else
		{
			hasCustomDestination = false;
			destination = destroyObjective.GlobalPosition;
		}
	
		Godot.Vector3 direction = (destination - this.GlobalPosition) * 0.5f;
		direction = direction.Normalized();
		destination.Y = this.GlobalPosition.Y;

		if (this.GlobalPosition.DistanceSquaredTo(destination) < this.stopDistance * this.stopDistance)
		{
			if (hasCustomDestination) hasCustomDestination = false;
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
		
		if (destroyObjective != null ) // squared 5
		{
			if (this.GlobalPosition.DistanceSquaredTo(destroyObjective.GlobalPosition) <= 25 && destroyObjective.healthPoints > 0)
			{
				if (this.attackTimer.IsStopped()) this.attackTimer.Start();
			}
		}
		else
		{
			if (!this.attackTimer.IsStopped()) this.attackTimer.Stop();
		}
		
	}
	public void OnAttack()
	{
		this.destroyObjective?.TakeHealth(Soldier.strength);
	}
}
