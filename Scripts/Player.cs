using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 50;

    public bool IsAttacking = false;
    public Vector2 NewDirection;
    public AnimatedSprite2D AnimatedSprite;


    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_attack"))
        {
            IsAttacking = true;
            AnimatedSprite.Play("attack_" + GetDirectionString());
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector2.Zero;

        direction.X = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        direction.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

        //If input is digital, normalize it for diagonal movement
        if (Math.Abs(direction.X) == 1 && Math.Abs(direction.Y) == 1)
            direction = direction.Normalized();

        if (Input.IsActionPressed("ui_sprint"))
            Speed = 100;
        else if (Input.IsActionJustReleased("ui_sprint"))
            Speed = 50;

        var movement = Speed * direction * (float)delta;

        if (!IsAttacking)
        {
            MoveAndCollide(movement);
            PlayerAnimations(direction);
        }

        if (!Input.IsAnythingPressed())
            if(!IsAttacking)
                AnimatedSprite.Play("idle_" + GetDirectionString());

    }

    public void PlayerAnimations(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            //Play walk because we are moving
            NewDirection = direction;
            AnimatedSprite.Play("walk_" + GetDirectionString());
        }

        else
        {
            //Play idle because we are still
            AnimatedSprite.Play("idle_" + GetDirectionString());
        }

    }

    public void OnAnimatedSprite2DAnimationFinished()
    {
        IsAttacking = false;
    }

    public string GetDirectionString()
    {
        var normalizedDirection = NewDirection.Normalized();
        var defaultDirection = "side";

        if (normalizedDirection.Y > 0)
            return "down";
        else if (normalizedDirection.Y < 0)
            return "up";
        else if (normalizedDirection.X > 0)
        {
            //sprite is drawn to the right
            AnimatedSprite.FlipH = false;
            return "side";
        }
        else if (normalizedDirection.X < 0)
        {
            //flip sprite for left
            AnimatedSprite.FlipH = true;
            return "side";
        }

        return defaultDirection;
    }
}
