using MarchingCubesProject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VoxelTerrainGenerator : MonoBehaviour {

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
    [HideInInspector]
    public Vector2 MountainPosition;
    public AnimationCurve MountainCurve;
    public Material MeshMaterial;

    int width = 500;
    int height = 100;
    int length = 500;
    private int Resolution;
    private List<GameObject> meshes = new List<GameObject>();

    private void Awake()
    {
        Resolution = width;
        MountainPosition = new Vector2((float)Resolution / 2, (float)Resolution / 2);
    }

    public void Start()
    {
        Generate();
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
                terrainheights[y, x] = sample * NoiseScale;
            }
        }

        terrainheights = GenerateMountainAddativley(terrainheights);

        //Generating the mesh from the voxel data.

        Marching marching = new MarchingCubes();
        marching.Surface = 0.0f;

        float[] voxels = new float[width * height * length];

        //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    int idx = x + y * width + z * width * height;

                    float heightValue = terrainheights[x, z] * (height - 1);
                    voxels[idx] = y < heightValue ? 1 : 0;
                }
            }
        }

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        for (int i = 0; i < numMeshes; i++)
        {

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = MeshMaterial;
            go.GetComponent<MeshFilter>().mesh = mesh;

            go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);

            meshes.Add(go);
        }
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

}
