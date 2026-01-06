using System.Collections.Generic;

public class SoldierManager
{
    public List<Soldier> soldiers = new List<Soldier>();
    
    public void Update(float delta)
    {
        foreach (Soldier soldier in soldiers)
        {
            soldier.Tick(delta);
        }
    }
}