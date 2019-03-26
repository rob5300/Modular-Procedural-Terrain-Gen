using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public abstract class ConversionMethod : Method
    {
        public bool Converted = false;

        public abstract ResultData Convert(Volumetric3 data);

        public abstract void Display(HashSet<GameObject> createdObjects, Transform target);

    }
}
