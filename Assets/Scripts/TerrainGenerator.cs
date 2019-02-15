using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour {

    public NoiseMethodType type;
    public float Frequency = 1f;
    [Range(1, 3)]
    public int dimensions = 3;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1f, 4f)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public float MountainRaduis = 10f;
    [Range(0.1f, 1)]
    public float NoiseScale = 0.5f;
    [Range(0.1f, 1)]
    public float MountainScale = 0.5f;
    public Vector2 MountainPosition;
    public AnimationCurve MountainCurve;

    private int Resolution;
    private Terrain terrain;

    private void Awake()
    {
        terrain = GetComponent<Terrain>();
        Resolution = terrain.terrainData.heightmapResolution;
        MountainPosition = new Vector2((float)Resolution / 2, (float)Resolution / 2);
    }

    public void Generate()
    {
        float[,] terrainheights = new float[Resolution, Resolution];

        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f / Resolution;
        for (int y = 0; y < Resolution; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);

            for (int x = 0; x < Resolution; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                //_texture.SetPixel(x, y, new Color(point.x, point.y, point.z));
                float sample = Noise.Sum(method, point, Frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value)
                {
                    sample = sample * 0.5f + 0.5f;
                }
                terrainheights[y,x] = sample * NoiseScale;
            }
        }
        terrainheights = GenerateMountainAddativley(terrainheights);
        terrain.terrainData.SetHeightsDelayLOD(0, 0, terrainheights);
    }

    public float[,] GenerateMountainAddativley(float[,] heightdata)
    {
        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                float distance = Mathf.Abs(Vector2.Distance(new Vector2(y, x), MountainPosition));
                if (distance > MountainRaduis) continue;
                //We are in the mountain raduis, continue
                float value = MountainCurve.Evaluate(Mathf.Abs((distance / MountainRaduis) - 1)) * MountainScale;
                if (value > heightdata[y, x]) heightdata[y, x] = value;
            }
        }
        return heightdata;
    }

    public void SetFrequency(float value)
    {
        Frequency = value;
        Generate();
    }

    public void SetOctaves(float value)
    {
        octaves = (int)value;
        Generate();
    }

    public void SetMountainRaduis(float value)
    {
        MountainRaduis = value;
        Generate();
    }

    public void SetMountainScale(float value)
    {
        MountainScale = value;
        Generate();
    }

    public void SetNoiseScale(float value)
    {
        NoiseScale = value;
        Generate();
    }

    public void CalcLOD()
    {
        terrain.ApplyDelayedHeightmapModification();
    }

}
