using System.Collections.Generic;

public partial class SoldierManager : Godot.Node3D
{
	public List<Soldier> soldiers = new List<Soldier>();


	Godot.Vector3 soldierPos = new Godot.Vector3(0,0,0);
	Godot.Vector3 soldierPosIncrement = new Godot.Vector3(3,0,0);
	int SoldierPosRotationAngle = 0;

	public Godot.Vector3 soldiersRotation = new Godot.Vector3(0,0,0);
	Godot.Vector3 rotationOffset = new Godot.Vector3(0,Godot.Mathf.DegToRad(90),0);

	int SoldierLayerAmount = 0;

	Player player;

	public void Initialize(Player player)
	{
		this.player = player;
	}
	
	public void SpawnSoldier()
	{
		if ((float)SoldierPosRotationAngle / 360f == SoldierPosRotationAngle / 360)
		{
			soldierPos += soldierPosIncrement;
			SoldierLayerAmount += 6;
		}
		Soldier soldier = GameGlobals.soldierScene.Instantiate<Soldier>();
		soldier.Initialize(this.player, soldierPos.Rotated(Godot.Vector3.Up,Godot.Mathf.DegToRad(SoldierPosRotationAngle)));
		AddChild(soldier);
		SoldierPosRotationAngle += 360 / SoldierLayerAmount;
		this.soldiers.Add(soldier);

	}
	public void Update(float delta, Godot.Vector3 rotation)
	{

		this.soldiersRotation = rotation  + rotationOffset;
		foreach (Soldier soldier in soldiers)
		{
			soldier.Tick(delta, this.soldiersRotation);
		}
	}
}
