using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public enum CubeType { Red, Green, Blue, Yellow, TNT, Box, Stone, Vase, Random, Default }
    [HideInInspector] public CubeType type;

    public void Initialize(CubeType cubeType)
    {
        type = cubeType;
        // Additional initialization logic here, e.g., setting the cube color based on type
    }

    private void OnMouseDown()
    {
        CheckForMatches();
    }

    public void CheckForMatches()
    {
        List<CubeController> matchedCubes = GridManager.Instance.FindMatchesAt(transform.position, type);
        if (matchedCubes.Count >= 3) // Minimum match size
        {
            GridManager.Instance.RemoveCubes(matchedCubes);
        }
    }
}
