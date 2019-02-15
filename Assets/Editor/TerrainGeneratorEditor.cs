using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private TerrainGenerator t;

    public void Awake()
    {
        t = (TerrainGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate")) { t.Generate(); t.CalcLOD(); }
    }
}
