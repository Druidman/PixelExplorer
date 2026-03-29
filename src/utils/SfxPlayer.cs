using Godot;

public partial class SfxPlayer : AudioStreamPlayer
{
	public override void _Ready()
	{
	this.Bus = "SFX";
	this.VolumeDb = this.VolumeDb + 15;
  }

	
}
