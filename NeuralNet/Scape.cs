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

        public delegate void ScapeDataHandler(ScapeData scapeData);
        public event ScapeDataHandler DataReceived;

        protected void NotifyDataReceived(ScapeData scapeData)
        {
            DataReceived(scapeData);
        }
    }
}