using Godot;
using System;

public partial class HealthBar : ColorRect
{
    public ColorRect Value;
    public override void _Ready()
    {
        Value = GetNode<ColorRect>("Value");
    }

    public void UpdateHealthUI(float health, float maxHealth)
    {
        var size = Value.Size;
        size.X = 98 * (health / maxHealth);
        Value.Size = size;
    }
}
