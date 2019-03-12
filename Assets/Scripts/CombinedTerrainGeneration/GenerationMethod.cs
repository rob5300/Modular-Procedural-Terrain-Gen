using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public abstract class GenerationMethod : Method
    {
        public abstract Volumetric3 Generate(Volumetric3 startData);
    }
}
