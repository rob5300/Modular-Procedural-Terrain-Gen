using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureCreator : MonoBehaviour {

    [Range(2, 512)]
    public int Resolution = 256;
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


    private Texture2D _texture;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, false);
            _texture.name = "Procedural Texture";
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.filterMode = FilterMode.Trilinear;
            _texture.anisoLevel = 9;
            _meshRenderer.material.mainTexture = _texture;
        }
        FillTexture();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            FillTexture();
        }
    }

    public void FillTexture()
    {
        //If the resolution field has changed, resize the texture.
        if (_texture.width != Resolution)
        {
            _texture.Resize(Resolution, Resolution);
        }

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
                _texture.SetPixel(x, y, Color.white * sample);
            }
        }
        _texture.Apply();
    }
}
