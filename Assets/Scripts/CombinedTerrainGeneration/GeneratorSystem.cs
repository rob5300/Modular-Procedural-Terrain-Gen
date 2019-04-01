using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public class GeneratorSystem
    {
        public string Status = "Idle";

        public List<GenerationMethod> MethodList;
        public ConversionMethod ConversionMethod;
        public HashSet<GameObject> TerrainObjects;

        public event Action Finished;

        public int Length { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private UnityHelper _helper;
        private Volumetric3 _data;
        private Transform _target;

        public GeneratorSystem(UnityHelper helper, int length, int width, int height)
        {
            _helper = helper;
            Length = length;
            Width = width;
            Height = height;
            MethodList = new List<GenerationMethod>();
            TerrainObjects = new HashSet<GameObject>();
        }

        public ResultData Generate(Transform target)
        {
            ResultData result;
            Status = "Generating (1/3)";
            _target = target;

            //Generate a new set of data.
            _data = new Volumetric3(Length, Width, Height);
            if(MethodList.Count > 0)
            {
                foreach(GenerationMethod method in MethodList)
                {
                    try
                    {
                        _data = method.Generate(_data);
                    }
                    catch (Exception e)
                    {
                        Status = "Idle from error.";
                        return new ResultData(false, e.Message);
                    }
                }
                result = Convert();
                if (result.Successful)
                {
                    result = Display();
                }
                else
                {
                    Status = "Idle from Error";
                }
            }
            else
            {
                result = new ResultData(false, "There were no methods to use.");
            }
            return result;
        }

        private ResultData Convert()
        {
            ResultData result;
            Status = "Converting (2/3)";
            if(ConversionMethod != null)
            {
                result = ConversionMethod.Convert(_data);
            }
            else
            {
                result = new ResultData(false, "There is no conversion method.");
            }
            return result;
        }

        private ResultData Display()
        {
            Status = "Displaying (3/3)";
            _helper.UnityTasks.Enqueue(() => { _displayLogic(); });
            return new ResultData(true);
        }

        private void _displayLogic()
        {
            if (ConversionMethod.Converted)
            {
                ConversionMethod.Display(TerrainObjects, _target);
            }
            Status = "Done (Idle)";
            Finished?.Invoke();
        }
    }
}
