using System.Collections.Generic;

namespace LoboLabs.NeuralNet
{
    /// <summary>
    /// Represents that Data that will be piped into a Neural Net
    /// </summary>
    public abstract class Scape
    {
        public Scape()
        {
        }

        public delegate void ScapeDataHandler(object Sender, List<double> data);
        public event ScapeDataHandler DataReceived;

        protected void NotifyDataReceived(List<double> data)
        {
            DataReceived(this, data);
        }
    }
}