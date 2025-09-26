using System;
using System.Collections.Generic;
using static Enums;

public class ColorSlot
{
    // Stores up to 3 color types
    private List<COLOR_TYPE> colorSlots = new List<COLOR_TYPE>();

    // Returns the first color or None
    public COLOR_TYPE currentColor => colorSlots.Count > 0 ? colorSlots[0] : COLOR_TYPE.None;

    // Event triggered when current color changes
    public Action<COLOR_TYPE> OnColorChanged;

    // Adds a color if not duplicated; rotates if full
    public void AddColor(COLOR_TYPE newColor)
    {
        if (colorSlots.Contains(newColor))
            return;

        COLOR_TYPE beforeColor = currentColor;

        if (colorSlots.Count < 3)
        {
            colorSlots.Add(newColor);
        }
        else
        {
            colorSlots[0] = newColor;

        }

        if (currentColor != beforeColor)
            OnColorChanged?.Invoke(currentColor);
    }

    // Rotates color slots if gauge exists
    public void ColorRotate(Func<COLOR_TYPE, bool> hasGauge)
    {
        colorSlots.RemoveAll(c => !hasGauge(c));

        if (colorSlots.Count > 1)
        {
            var first = colorSlots[0];
            colorSlots.RemoveAt(0);
            colorSlots.Add(first);
        }

        OnColorChanged?.Invoke(currentColor);
    }

    // Returns all slot colors as list
    public List<COLOR_TYPE> GetColors()
    {
        return new(colorSlots);
    }

    // Checks if a color exists in slot
    public bool Contains(COLOR_TYPE color)
    {
        return colorSlots.Contains(color);
    }

    // Removes a specific color from slot
    public void Remove(COLOR_TYPE color)
    {
        colorSlots.Remove(color);
        OnColorChanged?.Invoke(currentColor);
    }

    // Clears all colors from slot
    public void Clear()
    {
        colorSlots.Clear();
        OnColorChanged?.Invoke(COLOR_TYPE.None);
    }

    // Returns slot colors as array of 3
    public COLOR_TYPE[] GetSlotColors()
    {
        COLOR_TYPE[] result = new COLOR_TYPE[3];

        for (int i = 0; i < 3; i++)
        {
            result[i] = (i < colorSlots.Count) ? colorSlots[i] : COLOR_TYPE.None;
        }

        return result;
    }
}