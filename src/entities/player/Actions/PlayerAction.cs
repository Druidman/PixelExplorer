using Godot;

public abstract partial class PlayerAction : Node3D
{
    [Export]
    public string actionName;

    [Export]
    public bool blocksMovement = false;

	protected Player player;

    public override void _Ready()
    {
        Node parent = GetParent();
        if (parent is not Player) throw new System.Exception("Player actions are allowed only for Player");
        this.player = (Player)parent;
    }

    public abstract void HandleInput(InputEvent inputEvent);
    public abstract void Update(double delta);

    public abstract void OnStart();
    public abstract void OnEnd();

}