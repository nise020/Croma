using UnityEngine;
using System.Collections.Generic;
using static Enums;


public class ColorSystem
{
    private const int maxGauge = 100;
    private const int defaultGainAmount = 100;


    private Dictionary<COLOR_TYPE, int> colorGauges = new();
    private ColorSkillSystem skillSystem = new ColorSkillSystem();
    private COLOR_TYPE currentColor;

    public ColorSlot colorSlot { get; private set; } = new();


    public ColorSystem(COLOR_TYPE initialColor)
    {
        skillSystem = new ColorSkillSystem();

        foreach (COLOR_TYPE type in System.Enum.GetValues(typeof(COLOR_TYPE)))
        {
            if (type == COLOR_TYPE.None) 
                continue;

            colorGauges[type] = 0;
        }

        colorSlot.OnColorChanged += SetColor;

        GainColor(initialColor); // gain initial color with gauge
    }

    // Sets the current color and changes strategy
    public void SetColor(COLOR_TYPE newColor)
    {
        Debug.Log($"[ColorSystem] SetColor to {newColor}");
        currentColor = newColor;
        skillSystem.ChangeColor(newColor);
    }

    // Returns the currently selected color
    public COLOR_TYPE GetCurrentColor()
    {
        return HasGauge(currentColor, 1) ? currentColor : COLOR_TYPE.None;
    }

    // Gets gauge value of a specific color
    public int GetGauge(COLOR_TYPE type)
    {
        if (type == COLOR_TYPE.None)
            return 1;
        else
            return colorGauges.ContainsKey(type) ? colorGauges[type] : 0;
    }

    // Increases gauge (up to max) for a specific color
    public void GainGauge(COLOR_TYPE type, int amount)
    {
        if (!colorGauges.ContainsKey(type)) 
            return;
        colorGauges[type] = Mathf.Min(colorGauges[type] + amount, maxGauge);
    }

    // Checks if the color gauge meets the threshold
    public bool HasGauge(COLOR_TYPE type, int threshold = 1)
    {
        return colorGauges.ContainsKey(type) && colorGauges[type] >= threshold;
    }

    // Consumes gauge if available; returns success
    public bool UseGauge(COLOR_TYPE type, int amount)
    {
        if (!HasGauge(type, amount)) return false;
        colorGauges[type] -= amount;
        return true;
    }

    public void GainColor(COLOR_TYPE type)
    {
        if (type == COLOR_TYPE.None) return;

        if (colorSlot.Contains(type))
        {
            GainGauge(type, defaultGainAmount); // recover existing gauge
        }
        else
        {
            colorSlot.AddColor(type);           // add to slot
            GainGauge(type, defaultGainAmount); // gain initial gauge
        }
    }

    // Resets gauge of a specific color
    public void ResetGauge(COLOR_TYPE type)
    {
        if (colorGauges.ContainsKey(type))
        {
            colorGauges[type] = 0;
        }
    }

    // Executes active skill if enough gauge
    public void ExecuteActiveSkill(Player player)
    {
        if (!HasGauge(currentColor, 30)) return;

        if (UseGauge(currentColor, 30))
        {
            skillSystem.Execute(player);
        }
    }

    // Ticks passive skill if enough gauge (consumes per second)
    public void TickPassiveSkill(Player player, float deltaTime)
    {
        if (!HasGauge(currentColor, 1)) return;

        UseGauge(currentColor, Mathf.CeilToInt(5 * deltaTime));
        skillSystem.Tick(player, deltaTime);
    }
}