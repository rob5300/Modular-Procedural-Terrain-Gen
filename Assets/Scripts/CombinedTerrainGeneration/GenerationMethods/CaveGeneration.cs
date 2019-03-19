using Assets.Scripts.CombinedTerrainGeneration.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombinedTerrainGeneration.GenerationMethods
{
    public class CaveGeneration : GenerationMethod
    {
        [Configurable]
        public int Radius = 10;
        [Configurable]
        public int Length = 250;
        [Configurable]
        public bool RandomStartPos = false;
        [Configurable]
        public float BeginXRatio = 0.5f;
        [Configurable]
        public float BeginZRatio = 0.5f;
        [Configurable]
        public float DepthRatio = 0.5f;
        [Configurable]
        public bool DepthAboveBase = true;
        [Configurable]
        public float DivergeX = 0.1f;
        [Configurable]
        public float DivergeY = 0.2f;
        [Configurable]
        public float DivergeZ = 0.3f;
        [Configurable]
        public bool RandomDiverge = true;
        [Configurable]
        public float RandomConvergeMax = 1f;

        private System.Random _random;

        public CaveGeneration()
        {
            _random = new System.Random();
        }

        public override Volumetric3 Generate(Volumetric3 startData)
        {
            //Clamp values to ratio ranges.
            if (RandomStartPos)
            {
                BeginXRatio = (float)_random.NextDouble();
                BeginXRatio = (float)_random.NextDouble();
            }
            else
            {
                BeginXRatio = Mathf.Clamp01(BeginXRatio);
                BeginZRatio = Mathf.Clamp01(BeginZRatio);
            }
            DepthRatio = Mathf.Clamp01(DepthRatio);

            //Our current position for the meta ball.
            Vector3Int currentPos = GetStartPosition(startData);

            int caveY = Mathf.CeilToInt(currentPos.y * DepthRatio);
            if (DepthAboveBase && caveY <= Radius * 2) caveY = (Radius * 2) + 1;

            //Create entrance, choose random vear direction to use.
            //We will extract data in a sphere untill we hit the y target.

            //Create diverge values:
            float xDiverge = 0;
            float zDiverge = 0;
            float xAmount = RandomDiverge ? (float)_random.NextDouble() * 2 - 1 * RandomConvergeMax : DivergeX;
            float zAmount = RandomDiverge ? (float)_random.NextDouble() * 2 - 1 * RandomConvergeMax : DivergeX;
            while (currentPos.y >= caveY)
            {
                RemoveFromArea(currentPos, ref startData);
                currentPos.y--;
                //Increase diverge amounts.
                xDiverge += xAmount;
                zDiverge += zAmount;
                if (xDiverge >= 1)
                {
                    currentPos.x -= 1;
                    xDiverge -= 1;
                }
                if (zDiverge >= 1)
                {
                    currentPos.z -= 1;
                    zDiverge -= 1;
                }
            }

            //Create a path that goes in a random direction and curves

            //Setup Y divergence.
            //float yDiverge = 0;
            //float yAmount = RandomDiverge ? (float)_random.NextDouble() * 2 - 1 * RandomConvergeMax : DivergeY;
            zDiverge = 0;
            xDiverge = 0;
            Vector3Int tunnelStart = currentPos;

            //Create a random normalized direction.
            Vector3 direction = new Vector3((float)_random.NextDouble() * 2 - 1, 0, (float)_random.NextDouble() * 2 - 1).normalized;

            float x = 0;
            float z = 0;
            while(Vector3Int.Distance(currentPos, tunnelStart) < Length && startData.InBounds(currentPos))
            {
                //While we are below the length and are in bounds, make a tunnel.
                RemoveFromArea(currentPos, ref startData);
                x += direction.x;
                z += direction.z;
                //Positive values
                if(x > 1)
                {
                    x -= 1;
                    currentPos.x += 1;
                }
                if(z > 1)
                {
                    z -= 1;
                    currentPos.z += 1;
                }

                //Negative values
                if (x < -1)
                {
                    x += 1;
                    currentPos.x -= 1;
                }
                if (z < -1)
                {
                    z += 1;
                    currentPos.z -= 1;
                }
            }
            
            return startData;
        }

        private void RemoveFromArea(Vector3Int position, ref Volumetric3 data)
        {
            for (int x = position.x - Radius; x < position.x + Radius; x++)
            {
                for (int y = position.y - Radius; y < position.y + Radius; y++)
                {
                    for (int z = position.z - Radius; z < position.z + Radius; z++)
                    {
                        if(Mathf.FloorToInt(Vector3Int.Distance(new Vector3Int(x,y,z), position)) <= Radius){
                            //We are in the sphere range, check if we are in bounds as well.
                            if(data.InBounds(new Vector3Int(x,y,z))) data.SetData(x, y, z, 0);
                        }
                    }
                }
            }
        }

        private Vector3Int GetStartPosition(Volumetric3 startData)
        {
            int x = 0;
            int y = 0;
            int z = 0;
            x = Mathf.CeilToInt(startData.Width * BeginXRatio);
            z = Mathf.CeilToInt(startData.Length * BeginZRatio);
            for (int potentialY = startData.Height - 1; potentialY >= 0 ; potentialY--)
            {
                y = potentialY;
                if (startData.GetData(x,y,z) == 1) break;
            }
            return new Vector3Int(x, y, z);
        }

    }
}
