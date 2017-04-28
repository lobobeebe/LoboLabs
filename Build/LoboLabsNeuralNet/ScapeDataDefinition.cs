using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoboLabs.NeuralNet
{
    public class ScapeDataDefinition
    {
        public ScapeDataDefinition(string name, int numInputs)
        {
            Name = name;
            NumInputs = numInputs;

            DataList = new List<ScapeData>();
        }

        public void AddScapeData(ScapeData data)
        {
            DataList.Add(data);
        }
        
        public List<ScapeData> DataList
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            protected set;
        }

        public int NumInputs
        {
            get;
            protected set;
        }
    }
}
