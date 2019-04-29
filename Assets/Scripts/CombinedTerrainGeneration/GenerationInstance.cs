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
        public Text DurationText;
        public Text totalTimesText;
        public InputField RepeatAmount;

        public UnityHelper helper;
        public GeneratorUIManager UIManager;
        public Transform GenerateTarget;

        private int RepeatCount = 10;
        private int genCount = 0;
        private bool repeatGenFinished = false;

        public GeneratorSystem GeneratorSys;
        private GeneratorConfigManager _configManager;
        private Task<ResultData> _generateTask;

        private bool _isRunning = false;
        private float _startTime;

        public void Awake()
        {
            GeneratorSys = new GeneratorSystem(helper, Length, Width, Height);
            _configManager = new GeneratorConfigManager();
        }

        public void AddMethod(Method method)
        {
            if(method != null)
            {
                if (method is ConversionMethod) GeneratorSys.ConversionMethod = (ConversionMethod)method;
                else if (method is GenerationMethod) GeneratorSys.MethodList.Add((GenerationMethod)method);
                else ResultText.text = "Method object type is invalid.";
            }
        }

        public void RemoveMethod(Method method)
        {
            //Get the index of the method being removed to remove its data pair list aswell.
            int index = GeneratorSys.MethodList.FindIndex(x => x == method);

            GeneratorSys.MethodList.Remove(method as GenerationMethod);
            UIManager.RemoveMethodDataPairs(index);
        }

        public void Update()
        {
            StatusText.text = GeneratorSys.Status;
            if (_isRunning && _generateTask.IsCompleted)
            {
                ResultData data = _generateTask.Result;
                ResultText.text = data.Message;
                _isRunning = false;

                //We are done!
                float duration = Time.time - _startTime;

                totalTimesText.text += genCount + ": " + duration.ToString() + " s, \n";

                DurationText.text = duration.ToString() + " s";
            }

            if (repeatGenFinished)
            {
                repeatGenFinished = false;
                if(genCount < RepeatCount)
                {
                    GenerateRepeat();
                }
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (_isRunning) return;

            CleanOldObjects();

            //Record the start time
            _startTime = Time.time;

            //Get the entered data for each method and attempt to apply the values via reflection
            for (int i = 0; i < GeneratorSys.MethodList.Count; i++)
            {
                _configManager.ApplyConfigurableValuesTo(GeneratorSys.MethodList[i], UIManager.GetDataFromPairs(i));
            }
            //Get the entered data for the conversion method and also apply that data
            _configManager.ApplyConfigurableValuesTo(GeneratorSys.ConversionMethod, UIManager.GetDataFromPairsOnConversionMethod());

            Func<ResultData> generateFunc= () => { return GeneratorSys.Generate(GenerateTarget); };
            _generateTask = Task.Run(generateFunc);
            _isRunning = true;
        }

        public void StartRepeatGen()
        {
            RepeatCount = Convert.ToInt32(RepeatAmount.text);
            genCount = 0;
            GenerateRepeat();
        }

        protected void GenerateRepeat()
        {
            Generate();
            genCount++;
            GeneratorSys.Finished += () => { repeatGenFinished = true; };
        }

        public List<ConfigurableField> GetConfigList(Type type)
        {
            return _configManager.GetConfigList(type);
        }

        private void CleanOldObjects()
        {
            foreach(GameObject ob in GeneratorSys.TerrainObjects)
            {
                Destroy(ob);
            }
            GeneratorSys.TerrainObjects.Clear();
        }
    }
}
