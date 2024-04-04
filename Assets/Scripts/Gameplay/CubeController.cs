using System.Collections;
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

    public bool IsMovable
    {
        get
        {
            return type != CubeType.Box && type != CubeType.Stone;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
    }

    public void Initialize(CubeType cubeType, int x, int y)
    {
        type = cubeType;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type);

        this.x = x;
        this.y = y;
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
        tntHint = false;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type, false);
    }

    public void SetCubeType(CubeType cubeType, bool tntHint)
    {
        type = cubeType;
        this.tntHint = tntHint;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(cubeType, tntHint);
    }

    public void ActivateTNTHint()
    {
        tntHint = true;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type, tntHint);
    }

    public void DeactivateTNTHint()
    {
        tntHint = false;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type, tntHint);
    }

    public void OnMouseDown()
    {
        if (type == CubeType.Stone || type == CubeType.Box || type == CubeType.Vase)
            return;
        CheckForMatches();
    }

    public void ConvertToTNT()
    {
        type = CubeType.TNT;
        spriteRenderer.sprite = CubeSpriteManager.Instance.GetSprite(type);
    }

    public void CheckForMatches()
    {
        GridManager.Instance.OnCubeClicked(this);
    }

    public void PlayDestructionEffect()
    {
        GameObject particleInstance = ParticlePoolManager.Instance.GetParticle(ParticleType.Cube);
        particleInstance.transform.position = transform.position;
        particleInstance.SetActive(true);

        ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
        ps.Play();

        ParticleSystem.MainModule mainModule = ps.main;
        // mainModule.startColor = cubeTypeData.color; // Set particle color

        var textureSheetAnimation = ps.textureSheetAnimation;
        // textureSheetAnimation.SetSprite(0, cubeTypeData.destructionSprite); // Set particle sprite

        // Play the particle system
        ps.Play();

    }
}
