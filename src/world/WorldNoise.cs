using System;
using Godot;

public class WorldNoise
{
	FastNoiseLite noise = new FastNoiseLite();
	Random random = new Random();
	public WorldNoise() {
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		noise.Seed = random.Next();
		
	}

	public float GetValue(float x, float z)
	{
		
		float y = noise.GetNoise2D(x,z);
		// y is in -1 to 1
		y =  (y + 1f) / 2f;

		y += DefaultHeight(y);

		return y;
	}
	private float DefaultHeight(float y)
	{
		


		return 0;
		
		
	}
}
