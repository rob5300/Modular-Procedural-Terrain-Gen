using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public class Volumetric3
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Length { get; private set; }
        private float[] _voxels;

        public Volumetric3(int length, int width, int height)
        {
            Width = width;
            Length = length;
            Height = height;
            _voxels = new float[Width * Height * Length];
            InitializeArray();
        }

        protected virtual void InitializeArray()
        {
            for (int i = 0; i < _voxels.Length; i++)
            {
                _voxels[i] = 0;
            }
        }

        public float GetData(int x, int y, int z)
        {
            int idx = x + y * Width + z * Width * Height;
            return _voxels[idx];
        }

        public void SetData(int x, int y, int z, float value)
        {
            _voxels[x + y * Width + z * Width * Height] = value;
        }
    }
}
