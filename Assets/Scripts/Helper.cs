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
    private const int MainTilesetColumns = 16;
    private const int MainTilesetRows = 16;

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
            "Grass1" => 83,
            "Grass2" => 84,
            "Heart" => 88,
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
        var column = index % MainTilesetColumns;
        var row = index / MainTilesetColumns;

        if (index < 0 || row >= MainTilesetRows)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Unsupported main tileset index.");
        }

        return new Rect(
            column * MainTilesetSpriteSize,
            (MainTilesetRows - 1 - row) * MainTilesetSpriteSize,
            MainTilesetSpriteSize,
            MainTilesetSpriteSize);
    }

}
