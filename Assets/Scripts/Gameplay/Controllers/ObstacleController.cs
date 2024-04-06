using System.Collections.Generic;
using UnityEngine;
using static CubeSpriteManager;

public class ObstacleController : MonoBehaviour
{
    private int health = 1;

    private CubeController cubeController;

    private CubeType obstacleType;

    void Start()
    {
        cubeController = GetComponent<CubeController>();
    }

    void OnEnable()
    {
        GridManager.OnCubeMatch += HandleCubeMatches;
    }

    void OnDisable()
    {
        GridManager.OnCubeMatch -= HandleCubeMatches;
    }

    public void SetObstacleType(CubeType type)
    {
        obstacleType = type;
        switch (type)
        {
            case CubeType.Stone:
                SetHealth(1);
                break;
            case CubeType.Box:
                SetHealth(1);
                break;
            case CubeType.Vase:
                SetHealth(2);
                break;
            default:
                SetHealth(1);
                break;
        }
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    private void HandleCubeMatches(List<Vector2Int> matchPositions)
    {
        foreach (Vector2Int matchPosition in matchPositions)
        {
            if (HandleCubeMatch(matchPosition))
            {
                break;
            }
        }
    }

    private bool HandleCubeMatch(Vector2Int matchPosition)
    {
        // Check if this obstacle is adjacent to the match
        if (IsAdjacentTo(matchPosition))
        {
            GetHit();
            return true;
        }
        return false;
    }

    public void GetHit()
    {
        if (cubeController.type == CubeType.Stone)
        {
            return;
        }
        if (cubeController.type == CubeType.Vase)
        {
            cubeController.SetCubeType(CubeType.VaseBroken);
        }
        health--;
        if (health <= 0)
        {
            HandleDestroy();
        }
    }

    public void GetTNTHit()
    {
        if (cubeController.type == CubeType.Vase)
        {
            cubeController.SetCubeType(CubeType.VaseBroken);
        }
        health--;
        if (health <= 0)
        {
            HandleDestroy();
        }
    }

    private void HandleDestroy()
    {
        PlayDestructionEffect();
        GridManager.Instance.GetGrid()[cubeController.GetX(), cubeController.GetY()] = null;
        GridManager.Instance.GetGridLayout()[cubeController.GetX(), cubeController.GetY()] = null;

        GridManager.OnCubeMatch -= HandleCubeMatches;

        if (cubeController.type == CubeType.VaseBroken)
            GameManager.Instance.ObstacleCleared(CubeType.Vase);
        else
            GameManager.Instance.ObstacleCleared(cubeController.type);
        CubeFactory.Instance.ReturnCube(gameObject);

        Destroy(this);
    }

    private bool IsAdjacentTo(Vector2Int position)
    {
        int x = cubeController.GetX();
        int y = cubeController.GetY();
        // Check if this obstacle is at left, right, up, or down of the match
        return (position.x == x && Mathf.Abs(position.y - y) == 1) ||
              (position.y == y && Mathf.Abs(position.x - x) == 1);
    }

    public void PlayDestructionEffect()
    {
        GameObject particleInstance = ParticlePoolManager.Instance.GetParticle(ObstacleTypeToParticleType());
        particleInstance.transform.position = transform.position;
        particleInstance.SetActive(true);

        ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
        ps.Play();
    }

    private ParticleType ObstacleTypeToParticleType()
    {
        switch (obstacleType)
        {
            case CubeType.Stone:
                return ParticleType.Stone;
            case CubeType.Box:
                return ParticleType.Box;
            case CubeType.Vase:
                return ParticleType.Vase;
            default:
                return ParticleType.Stone;
        }
    }
}
