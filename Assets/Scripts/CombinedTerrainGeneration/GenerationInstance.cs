using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public class GenerationInstance : MonoBehaviour
    {
        public int Length;
        public int Width;
        public int Height;

        public Text StatusText;
        public Text ResultText;

        public UnityHelper helper;

        private GeneratorSystem generator;
        private Task generateTask;

        public void Start()
        {
            generator = new GeneratorSystem(helper, Length, Width, Height);

            generator.MethodList.Add(new PerlinNoiseGeneration());
            generator.ConversionMethod = new MarchingCubesConversion();
        }

        public void Update()
        {
            StatusText.text = generator.Status;
            //if(generateTask)
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            Func<ResultData> generateTask = () => { return generator.Generate(); };
            Task.Run(generateTask);
        }
    }
}
