using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public GeneratorUIManager UIManager;

        private GeneratorSystem _generator;
        private GeneratorConfigManager _configManager;
        private Task<ResultData> _generateTask;

        private bool _isRunning = false;

        public void Start()
        {
            _generator = new GeneratorSystem(helper, Length, Width, Height);
            _configManager = new GeneratorConfigManager();
            _generator.MethodList.Add(new PerlinNoiseGeneration());
            _generator.ConversionMethod = new MarchingCubesConversion();
        }

        public void AddMethod(Method method)
        {
            if(method != null)
            {
                if (method is ConversionMethod) _generator.ConversionMethod = (ConversionMethod)method;
                else if (method is GenerationMethod) _generator.MethodList.Add((GenerationMethod)method);
                else ResultText.text = "Method invalid.";
            }
        }

        public void Update()
        {
            StatusText.text = _generator.Status;
            if (_isRunning && _generateTask.IsCompleted)
            {
                ResultData data = _generateTask.Result;
                ResultText.text = data.Message;
                _isRunning = false;
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (_isRunning) return;

            //Get the entered data for each method and attempt to apply the values via reflection
            for (int i = 0; i < _generator.MethodList.Count; i++)
            {
                UIManager.GetDataFromPairs(i)
            }

            Func<ResultData> generateFunc= () => { return _generator.Generate(); };
            _generateTask = Task.Run(generateFunc);
            _isRunning = true;
        }

        public List<string> GetConfigList(Type type)
        {
            return _configManager.GetConfigList(type);
        }
    }
}
