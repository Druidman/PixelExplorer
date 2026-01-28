using Godot;

public partial class MagicTurretBullet : Area3D
{
	Godot.Vector3 targetPosition;
	Godot.Vector3 startPosition;
	float attackDmg = 0f;

	public static float acceptableBulletTargetDistance = 0.5f;

	public static float SPEED = 5f;

	public void Instantiate(Godot.Vector3 targetPosition, Godot.Vector3 startPosition, float attackDmg)
	{
		this.targetPosition = targetPosition;
		this.startPosition = startPosition;
		this.attackDmg = attackDmg;
	}

	public override void _Process(double delta)
	{
		this.GlobalPosition += (targetPosition - startPosition).Normalized() * (float)delta * SPEED;

		if (this.GlobalPosition.DistanceSquaredTo(targetPosition) <= acceptableBulletTargetDistance * acceptableBulletTargetDistance){ // squared
			Godot.Collections.Array<Area3D> areas = this.GetOverlappingAreas();
			foreach (Area3D area in areas)
			{
				if (area is Soldier soldier)
				{
					soldier.TakeHealth(attackDmg);
				}
			}

			GetParent().RemoveChild(this);
			QueueFree();
		}
	}
}
