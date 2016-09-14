using System.Collections.Generic;

namespace LoboLabs
{
namespace Utilities
{

public static class VectorUtils
{
    /// <summary>
    /// Performs the scalar product on the two vectors
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public static double DotProduct(List<double> inputs, List<double> weights)
    {
	    double output = 0;

	    for (int i = 0; i < inputs.Count; ++i)
	    {
		    output += inputs[i] * weights[i];
	    }

	    return output;
    }

    public static bool GetIndex<T>(List<T> list, T item, out int index)
    {
        index = -1;
        bool isFound = false;

        for(int i = 0; i < list.Count && !isFound; ++i)
        {
            if(item.Equals(list[i]))
            {
                isFound = true;
                index = i;
            }
        }

        return isFound;
    }

    /// <summary>
    /// Returns the index of the most positive value. 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>The index of the most positive value - 0 If the vector is empty</returns>
    public static int MaxIndex(List<double> vector)
    {
	    // index of largest value
	    int maxIndex = 0;
	    for (int i = 0; i < vector.Count; ++i)
	    {
		    if (vector[i] > vector[maxIndex])
		    {
			    maxIndex = i;
		    }
	    }
	    return maxIndex;
    }

    /// <summary>
    /// Shuffles the order of the given vector
    /// </summary>
    /// <param name="vector"></param>
    public static void Shuffle(int[] vector)
    {
	    for (int i = 0; i < vector.Length; ++i)
	    {
		    int r = (int)(MathUtils.NextRand() * vector.Length);
		    int tmp = vector[r];
		    vector[r] = vector[i];
		    vector[i] = tmp;
	    }
    }
}

}
}
