using System;
using System.Collections.Generic;
using UnityEngine;
using static CubeSpriteManager;

public static class CubeUtils
{
    public static CubeType ConvertStringToCubeType(string typeStr)
    {
        switch (typeStr)
        {
            case "r": return CubeType.Red;
            case "g": return CubeType.Green;
            case "b": return CubeType.Blue;
            case "y": return CubeType.Yellow;
            case "t": return CubeType.TNT;
            case "bo": return CubeType.Box;
            case "s": return CubeType.Stone;
            case "v": return CubeType.Vase;
            case "rand": return CubeType.Random;
            default: return CubeType.Default;
        }
    }

    public static string ConvertCubeTypeToString(CubeType type)
    {
        switch (type)
        {
            case CubeType.Red: return "r";
            case CubeType.Green: return "g";
            case CubeType.Blue: return "b";
            case CubeType.Yellow: return "y";
            case CubeType.TNT: return "t";
            case CubeType.Box: return "bo";
            case CubeType.Stone: return "s";
            case CubeType.Vase: return "v";
            case CubeType.Random: return "rand";
            default: return "d";
        }
    }

    public static List<Vector2Int> CalculateTNTEffectedPositions(Vector2Int position, int radius, int width, int height)
    {
        List<Vector2Int> affectedPositions = new List<Vector2Int>();

        int halfRadius = (int)Math.Floor((double)(radius / 2));

        for (int x = position.x - halfRadius; x <= position.x + halfRadius; x++)
        {
            for (int y = position.y - halfRadius; y <= position.y + halfRadius; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    affectedPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return affectedPositions;
    }

    public static ParticleType ConvertCubeTypeToParticleType(CubeType type)
    {
        switch (type)
        {
            case CubeType.Red: return ParticleType.RedCube;
            case CubeType.Green: return ParticleType.GreenCube;
            case CubeType.Blue: return ParticleType.BlueCube;
            case CubeType.Yellow: return ParticleType.YellowCube;
            case CubeType.TNT: return ParticleType.TNT;
            case CubeType.Box: return ParticleType.Box;
            case CubeType.Stone: return ParticleType.Stone;
            case CubeType.Vase: return ParticleType.Vase;
            case CubeType.Random: return ParticleType.RedCube;
            default: return ParticleType.RedCube;
        }
    }
}