public partial class MusicPlayer : Godot.AudioStreamPlayer
{
  public override void _Ready()
	{
	this.Bus = "MUSIC";
  }
}
