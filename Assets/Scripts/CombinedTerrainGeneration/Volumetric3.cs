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
        public float[] Voxels;

        public Volumetric3(int length, int width, int height)
        {
            Width = width;
            Length = length;
            Height = height;
            Voxels = new float[Width * Height * Length];
            InitializeArray();
        }

        protected virtual void InitializeArray()
        {
            for (int i = 0; i < Voxels.Length; i++)
            {
                Voxels[i] = 0;
            }
        }

        public float GetData(int x, int y, int z)
        {
            int idx = x + y * Width + z * Width * Height;
            return Voxels[idx];
        }

        public void SetData(int x, int y, int z, float value)
        {
            Voxels[x + y * Width + z * Width * Height] = value;
        }
    }
}
