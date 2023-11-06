using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NonParallelArraySummation : MonoBehaviour
{
    [SerializeField] private int count = 5000;

    private void Start()
    {
        var aValue = Enumerable.Repeat(1f, count).ToArray();
        var bValue = Enumerable.Repeat(1f, count).ToArray();

        Stopwatch watch = new Stopwatch();

        watch.Start();
        for (int i = 0; i < count; i++)
        {
            CalculateArraySum(aValue, bValue, count);
        }
        watch.Stop();

        Debug.Log($"ElapsedMilliseconds: {watch.ElapsedMilliseconds}ms");
    }

    private static void CalculateArraySum(float[] aValue, float[] bValue, int length)
    {
        float[] result = new float[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = aValue[i] + bValue[i];
        }

        // for (int i = 0; i < length; i++)
        // {
        //     Debug.Log($"result[{i}] : {result[i]}");
        // }
    }
}