using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export] 
    public int Speed { get; set; } = 400;

    public Vector2 ScreenSize { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;

        velocity.Y += Input.IsActionPressed("move_up") 
            ? -1 
            : Input.IsActionPressed("move_down") 
                ? 1
                : 0;

        velocity.X += Input.IsActionPressed("move_right")
            ? 1
            : Input.IsActionPressed("move_left")
                ? -1
                : 0;

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }

        if (velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.Y > 0;
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x : Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private void OnBodyEntered(Node2D body)
    {
        Hide();
        EmitSignal(SignalName.Hit);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}
