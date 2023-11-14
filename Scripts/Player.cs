using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 50;
    [Signal]
    public delegate void HealthUpdatedEventHandler(float health, float maxHealth);
    [Signal]
    public delegate void StaminaUpdatedEventHandler(float stamina, float maxStamina);

    public bool IsAttacking { get; set; } = false;

    public float Health { get; set; } = 100;
    public float MaxHealth { get; set; } = 100;
    public float RegenHealth { get; set; } = 1;
    public float Stamina { get; set; } = 100;
    public float MaxStamina { get; set; } = 100;
    public float RegenStamina { get; set; } = 5;

    public Vector2 NewDirection;
    public AnimatedSprite2D AnimatedSprite;
    public HealthBar HealthBar;
    public StaminaBar StaminaBar;


    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        HealthBar = GetNode<HealthBar>("UI/HealthBar");
        StaminaBar = GetNode<StaminaBar>("UI/StaminaBar");

        HealthUpdated += HealthBar.UpdateHealthUI;
        StaminaUpdated += StaminaBar.UpdateStaminaUI;

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
        {
            if (Stamina >= 25)
            {
                Speed = 100;
                Stamina -= 5;
                EmitSignal(SignalName.StaminaUpdated, Stamina, MaxStamina);
            }

        }
        else if (Input.IsActionJustReleased("ui_sprint"))
            Speed = 50;

        var movement = Speed * direction * (float)delta;

        if (!IsAttacking)
        {
            MoveAndCollide(movement);
            PlayerAnimations(direction);
        }

        if (!Input.IsAnythingPressed())
            if (!IsAttacking)
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

    public override void _Process(double delta)
    {
        float updatedHealth = Mathf.Min(Health + RegenHealth * (float)delta, MaxHealth);
        if (updatedHealth != Health)
        {
            Health = updatedHealth;
            EmitSignal(SignalName.HealthUpdated, Health, MaxHealth);
        }

        float updatedStamina = Mathf.Min(Stamina + RegenStamina * (float)delta, MaxStamina);
        if (updatedStamina != Stamina)
        {
            Stamina = updatedStamina;
            EmitSignal(SignalName.StaminaUpdated, Stamina, MaxStamina);
        }
    }
}
