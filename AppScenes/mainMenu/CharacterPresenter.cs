using Godot;

public partial class CharacterPresenter : SubViewport
{
  [Export]
  private Node3D playerInstance = null;
  public override void _Process(double delta)
  {
	if (this.playerInstance == null) return;

	this.playerInstance.Rotation = this.playerInstance.Rotation + new Godot.Vector3(0, 1f * (float)delta,0);
  }
}
