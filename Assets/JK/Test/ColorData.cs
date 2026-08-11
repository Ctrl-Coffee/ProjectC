using UnityEngine;

public class ColorData
{
    public ColorData(string id, Color color) 
    {
        Id = id;
        Color = color;
    }

    public string Id { get; set; }
    public Color Color { get; set; }
}
