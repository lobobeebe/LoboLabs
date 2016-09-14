using System.Collections.Generic;

namespace LoboLabs
{ 
namespace Utilities
{

/// <summary>
/// TODO: Documentation
/// TODO: Unit Tests
/// </summary>
public class KnownData
{
    public KnownData(List<double> inputs, List<double> expectedOutputs)
    {
        InputValues = inputs;
        ExpectedOutputs = expectedOutputs;
    }

    public List<double> InputValues
    {
        get;
        set;
    }

    public List<double> ExpectedOutputs
    {
        get;
        set;
    }
}

}
}
