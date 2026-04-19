using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class Helper
{
    private const int MainTilesetUppercaseStart = 21;
    private const int MainTilesetLowercaseStart = 47;
    private const int MainTilesetNumberStart = 73;
    private const string MainTilesetSpritePrefix = "MainTileset_";
    private const float MainTilesetSpriteSize = 16f;

    public static HeartState RotateHeartState(this HeartState state, int shift)
    {
        var len = Enum.GetNames(typeof(HeartState)).Length;
        var currentState = (int)state;
        return (HeartState)((currentState + shift) % len);
    }

    public static T RotateEnum<T>(T enm, int shift) where T : Enum
    {
        var len = Enum.GetNames(typeof(T)).Length;
        var currentState = (int)((object)enm);
        
        return (T)Enum.ToObject(typeof(T), (currentState + shift) % len);
    }

    public static int GetTileSetByChar(char value)
    {
        if (value is >= 'A' and <= 'Z')
        {
            return MainTilesetUppercaseStart + (value - 'A');
        }

        if (value is >= 'a' and <= 'z')
        {
            return MainTilesetLowercaseStart + (value - 'a');
        }

        if (value is >= '0' and <= '9')
        {
            return MainTilesetNumberStart + (value - '0');
        }

        throw new Exception();
    }

    public static int GetTileSetByString(string value)
    {
        //TODO: phase
        return value switch
        {
            "Hash" => 0,
            "OpenCross" => 1,
            "2VL" => 2,
            "2HL" => 3,
            "Corner1" => 4,
            "Corner2" => 5,
            "Corner3" => 6,
            "Corner4" => 7,
            "T1" => 8,
            "T2" => 9,
            "T3" => 10,
            "T4" => 11,
            "Box" => 12,
            "Dot" => 13,
            "Hourglass" => 14,
            "Warden" => 15,
            "Circle" => 16,
            "Cross" => 17,
            "HL" => 18,
            "VL" => 19,
            _ => 0
        };
    }

    public static int GetMainTilesetIndex(string value)
    {
        if (value is {Length:1})
        {
            return GetTileSetByChar(value[0]);
        }

        return GetTileSetByString(value);
    }

    public static string GetMainTilesetSpriteName(string value)
    {
        return $"{MainTilesetSpritePrefix}{GetMainTilesetIndex(value)}";
    }

    public static Rect GetMainTilesetSpriteRect(int index)
    {
        return index switch
        {
            >= 0 and <= 11 => new Rect(index * MainTilesetSpriteSize, 240f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            >= 12 and <= 26 => new Rect((index - 11) * MainTilesetSpriteSize, 224f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            >= 27 and <= 42 => new Rect((index - 27) * MainTilesetSpriteSize, 208f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            >= 43 and <= 58 => new Rect((index - 43) * MainTilesetSpriteSize, 192f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            >= 59 and <= 74 => new Rect((index - 59) * MainTilesetSpriteSize, 176f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            >= 75 and <= 77 => new Rect((index - 75) * MainTilesetSpriteSize, 160f, MainTilesetSpriteSize, MainTilesetSpriteSize),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Unsupported main tileset index.")
        };
    }

}
