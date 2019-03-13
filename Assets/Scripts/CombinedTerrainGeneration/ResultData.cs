using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public struct ResultData
    {
        public bool Successful;
        public string Message;

        public ResultData(bool successful) : this(successful, "Successful with no errors.")
        {
        }

        public ResultData(bool successful, string errorMessage)
        {
            Successful = successful;
            Message = errorMessage;
        }
    }
}
