using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CubeSpriteManager;

public class CubeController : MonoBehaviour
{
    [HideInInspector] public CubeType type;
    private bool tntHint = false;

    private Image spriteRenderer;

    private int x;
    private int y;

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
    }

    public void Initialize(CubeType cubeType)
    {
        type = cubeType;
        // Additional initialization logic here, e.g., setting the cube color based on type
    }

    public void SetXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetCubeType(CubeType cubeType)
    {
        type = cubeType;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type);
    }

    public void SetCubeType(CubeType cubeType, bool tntHint)
    {
        type = cubeType;
        this.tntHint = tntHint;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(cubeType, tntHint);
    }

    public void OnMouseDown()
    {
        Debug.Log("Cube clicked: " + x + ", " + y);
        CheckForMatches();
    }

    public void CheckForMatches()
    {
        GridManager.Instance.OnCubeClicked(this);
    }
}
