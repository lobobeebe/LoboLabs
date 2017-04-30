namespace LoboLabs.NeuralNet.Mock
{
    public class Scape : NeuralNet.Scape
    {
        public Scape()
        {
        }

        public new void NotifyDataReceived(NeuralNet.ScapeData scapeData)
        {
            base.NotifyDataReceived(scapeData);
        }
    }

}