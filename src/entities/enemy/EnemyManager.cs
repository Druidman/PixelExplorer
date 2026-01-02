using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager
{
	// Called when the node enters the scene tree for the first time.

	PackedScene enemyScene = null;
	Player player;
	World world;

	List<Enemy> enemies = new List<Enemy>();
	public EnemyManager(Player player, World world)
	{

		this.player = player;
		this.world = world;
		this.enemyScene = GD.Load<PackedScene>("res://src/entities/enemy/enemy.tscn");

		SpawnEnemy(this.player.Position);
	}

	public void SpawnEnemy(Godot.Vector3 position)
	{
		Enemy enemy = enemyScene.Instantiate<Enemy>();
		
		enemy.Position = position;

		this.world.CallDeferred(Node3D.MethodName.AddChild, enemy);

		enemies.Add(enemy);
	}

	public void UpdateEnemies()
	{
		foreach (Enemy enemy in this.enemies)
		{
			Godot.Vector3 direction = this.player.Position - enemy.Position;

			enemy.moveDirection = direction.Normalized();	
		}

	}
}
