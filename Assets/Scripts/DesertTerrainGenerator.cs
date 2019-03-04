using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class DesertTerrainGenerator : MonoBehaviour {

    public float Weight = 1f;
    public float Scale = 10f;
    public float WrapValue = 200f;
    public Vector2 WindDirection;

    private int Resolution;
    private Terrain terrain;

    private void Awake()
    {
        terrain = GetComponent<Terrain>();
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        Resolution = terrain.terrainData.heightmapResolution;

        float[,] terrainheights = new float[Resolution, Resolution];

        for (int y = 0; y < Resolution; y++)
        {
            for (int x = 0; x < Resolution; x++)
            {
                int xa = Mathf.FloorToInt(x % WrapValue);
                int ya = Mathf.FloorToInt(y % WrapValue);
                //I tried to inteprite the expression here, this is probably not correct.
                float dot = Vector3.Dot(WindDirection.normalized, new Vector3(xa, ya) - (Vector3)WindDirection.normalized);
                float ri = ReverseInterpolate(dot);
                float inter = Interpolate(ri);
                terrainheights[y, x] = Weight * inter;

                if ((x == Resolution - 1 && y == Resolution - 1) || (x == 0 && y == 0))
                    Debug.Log("Dot: " + dot + ", Ri: " + ri + ", I: " + inter + " Value: " + terrainheights[y, x]);
            }
        }
        terrain.terrainData.SetHeightsDelayLOD(0, 0, terrainheights);
        terrain.ApplyDelayedHeightmapModification();
    }

    private float WeightFunction(Node node)
    {
        float dot = Vector3.Dot(WindDirection.normalized, new Vector3(node.Position.x, node.Position.y));
        float ri = ReverseInterpolate(dot);
        float inter = Interpolate(ri);
        return Weight * inter;
    }

    private float Interpolate(float value)
    {
        float x1 = 0;
        float x2 = 1;
        float y1 = 0;
        float y2 = Weight;
        float y = y1 + (value - x1) / (x2 - x1) * (y2 - y1);
        return y;
    }

    // (x) -1 > 1 into  (y) 0 > 1
    private float ReverseInterpolate(float x)
    {
        float x1 = -1;
        float x2 = 1;
        float y1 = 0;
        float y2 = 1;
        float y = y1 + (x - x1)/ (x2 - x1) * (y2 - y1);
        return y;
    }

}

public struct Node
{
    public Vector2Int Position;
    public bool Processed;

    public Node(Vector2Int pos)
    {
        Position = pos;
        Processed = false;
    }
}