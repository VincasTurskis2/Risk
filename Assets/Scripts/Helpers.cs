using UnityEngine;

public static class Helpers{
    // A helper function to convert an int array into a "{a, b, c}" formatted string;
    public static string IntArrayToString(int[] array)
    {
        string result = "{";
        for(int i = 0; i < array.Length; i++)
        {
            result += array[i];
            if(i < array.Length - 1)
            {
                result += ", ";
            }
        }
        result += "}";
        return result;
    }
    
    // A helper function to find the largest element's index in an int array
    public static int ArrayMaxElementIndex(int[] array)
    {
        int max = int.MinValue, index = 0;
        for(int i = 0; i < array.Length; i++)
        {
            if(array[i] > max)
            {
                max = array[i];
                index = i;
            }
        }
        return index;
    }

    // A helper function to get a highlighed version of a color
    public static Color GetHighlighedColorVersion(Color baseColor)
    {
        float h, s, v;
        Color.RGBToHSV(baseColor, out h, out s, out v);
        if(baseColor != ColorPreset.White)
        {
            s += 0.2f;
        }
        else
        {
            return Color.white;
        }
        return Color.HSVToRGB(h, s, v);
    }
}