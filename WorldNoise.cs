using Godot;

public class WorldNoise
{
	FastNoiseLite noise = new FastNoiseLite();
	public WorldNoise() {
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		
		
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
		
		// if (y < 0.25f)
		// {
		// 	return 0;
		// }
		// else if (y < 0.40f)
		// {
		// 	return y - 0.25f;
		// }
		// else if (y<0.6f)
		// {
		// 	return 0.55f;
		// }
		// else if (y < 0.7f)
		// {
		// 	return y-0.55f;
		// }
		// else
		// {
		// 	return 0.85f;
		// }

		return 0;
		
		
	}
}
