using UnityEngine;
using System.Collections.Generic;

public class CubeSpriteManager : MonoBehaviour
{
    public static CubeSpriteManager Instance;

    public enum CubeType { Red, Green, Blue, Yellow, RedTNT, GreenTNT, BlueTNT, YellowTNT, TNT, Box, Stone, Vase, VaseBroken, Random, Default }

    [System.Serializable]
    public struct CubeSpriteEntry
    {
        public CubeType type;
        public Sprite sprite;
    }

    public List<CubeSpriteEntry> cubeSprites;

    private Dictionary<CubeType, Sprite> spritesDictionary = new Dictionary<CubeType, Sprite>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in cubeSprites)
        {
            spritesDictionary[entry.type] = entry.sprite;
        }
    }

    public Sprite GetSprite(CubeType type)
    {
        if (spritesDictionary.ContainsKey(type))
        {
            return spritesDictionary[type];
        }
        else
        {
            Debug.LogWarning($"Sprite for cube type {type} not found.");
            return null;
        }
    }

    public Sprite GetSprite(CubeType type, bool tntHint)
    {
        if (tntHint)
        {
            switch (type)
            {
                case CubeType.Red:
                    type = CubeType.RedTNT;
                    break;
                case CubeType.Green:
                    type = CubeType.GreenTNT;
                    break;
                case CubeType.Blue:
                    type = CubeType.BlueTNT;
                    break;
                case CubeType.Yellow:
                    type = CubeType.YellowTNT;
                    break;
                default:
                    break;
            }
        }
        if (spritesDictionary.ContainsKey(type))
        {
            return spritesDictionary[type];
        }
        else
        {
            Debug.LogWarning($"Sprite for cube type {type} not found.");
            return null;
        }
    }
}
