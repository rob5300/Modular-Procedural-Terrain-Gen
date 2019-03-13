using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public class GeneratorSystem
    {
        public string Status = "Idle";

        public List<GenerationMethod> MethodList;
        public ConversionMethod ConversionMethod;

        public int Length { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private UnityHelper _helper;
        private Volumetric3 _data;

        public GeneratorSystem(UnityHelper helper, int length, int width, int height)
        {
            _helper = helper;
            Length = length;
            Width = width;
            Height = height;
            MethodList = new List<GenerationMethod>();
        }

        public ResultData Generate()
        {
            ResultData result;
            Status = "Generating";

            //Generate a new set of data.
            _data = new Volumetric3(Length, Width, Height);
            if(MethodList.Count > 0)
            {
                foreach(GenerationMethod method in MethodList)
                {
                    _data = method.Generate(_data);
                }
                result = Convert();
                if (result.Successful)
                {
                    result = Display();
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
            Status = "Converting";
            if(ConversionMethod != null)
            {
                ConversionMethod.Convert(_data);
                result = new ResultData(true);
            }
            else
            {
                result = new ResultData(false, "There is no conversion method.");
            }
            return result;
        }

        private ResultData Display()
        {
            Status = "Displaying";
            _helper.UnityTasks.Enqueue(() => { _displayLogic(); });
            return new ResultData(true);
        }

        private void _displayLogic()
        {
            if (ConversionMethod.Converted)
            {
                ConversionMethod.Display();
            }
            Status = "Done (Idle)";
        }
    }
}
