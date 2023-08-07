using UnityEngine;
public static class ColorPreset
{
    public static readonly Color Green = new Color(200f / 255f, 1, 200f/255f, 1);
    public static readonly Color Pink = new Color(1, 200f/255f, 200f/255f, 1);
    public static readonly Color Purple = new Color(200f/255f, 200f/255f, 1, 1);
    public static readonly Color Cyan = new Color(200f/255f, 1, 1, 1);
    public static readonly Color Magenta = new Color(1, 200f/255f, 1, 1);
    public static readonly Color Yellow = new Color(1, 1, 200f/255f, 1);

    public static Color GetColorPreset(PlayerColor colorEnum)
    {
        switch(colorEnum)
        {
            case PlayerColor.Green:
                return Green;
            case PlayerColor.Pink:
                return Pink;
            case PlayerColor.Purple:
                return Purple;
            case PlayerColor.Cyan:
                return Cyan;
            case PlayerColor.Magenta:
                return Magenta;
            case PlayerColor.Yellow:
                return Yellow;
            default:
                return Color.white;
        }
    }
    public static Color GetColorPreset(int colorNum)
    {
        switch(colorNum)
        {
            case 0:
                return Green;
            case 1:
                return Pink;
            case 2:
                return Purple;
            case 3:
                return Cyan;
            case 4:
                return Magenta;
            case 5:
                return Yellow;
            default:
                return Color.white;
        }
    }


}

public enum PlayerColor
{
    Green, Pink, Purple, Cyan, Magenta, Yellow
}