using System.Collections.Generic;
using Godot;

public partial class CoinManager : ObjectManager<Coin>
{
	public override int ObjectsLimit{get; protected set;} = 1000;

	protected override void InitializeObject(Coin coin)
	{
		coin.Initialize(()=>this.RemoveCoin(coin));
	}

	private void RemoveCoin(Coin coin){
		this.RemoveObject(coin);
		this.GenerateObjects(); // after deletion we regenerate deleted ones
	}

	
	public override void GenerateObjects ()
	{
		this.GenerateObjectsRandomlyOnWorldBlocksSurface();
	}


}
