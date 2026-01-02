using Godot;

public abstract class Movement
{
    protected Player player = null;
    protected Movement(Player player)
    {
        this.player = player;
    }
    
    public abstract void HandleInputEvent(InputEvent ev);
    public abstract void HandleProcess(double delta);
}