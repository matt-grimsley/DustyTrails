using Godot;
using System;

public partial class StaminaBar : ColorRect
{
    public ColorRect Value;

    public override void _Ready()
    {
        Value = GetNode<ColorRect>("Value");
    }

    public void UpdateStaminaUI(float stamina, float maxStamina)
    {
        var size = Value.Size;
        size.X = 98 * (stamina / maxStamina);
        Value.Size = size;
    }

}
