using Assets.Scripts.CombinedTerrainGeneration.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombinedTerrainGeneration.GenerationMethods
{
    public class SolidGeneration : GenerationMethod
    {
        [Configurable]
        public int Height = 128;

        public override Volumetric3 Generate(Volumetric3 startData)
        {
            for (int x = 0; x < startData.Length; x++)
            {
                for (int z = 0; z < startData.Width; z++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        startData.SetData(x, y, z, 1);
                    }
                }
            }
            return startData;
        }
    }
}
