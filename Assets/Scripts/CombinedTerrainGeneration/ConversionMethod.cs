using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public abstract class ConversionMethod : Method
    {
        public bool Converted = false;

        public abstract void Convert(Volumetric3 data);

        public abstract void Display();

    }
}
