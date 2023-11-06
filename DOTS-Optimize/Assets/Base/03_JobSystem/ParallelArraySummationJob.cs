using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ParallelArraySummationJob : MonoBehaviour
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
            CalculateJob(aValue, bValue, count);
        }
        watch.Stop();
        
        Debug.Log($"ElapsedMilisecond : {watch.ElapsedMilliseconds}ms");
    }

    private void CalculateJob
        (float[] aValue, float[] bValue, int length)
    {
        NativeArray<float> a = 
            new NativeArray<float>(length, Allocator.TempJob);
        NativeArray<float> b = 
            new NativeArray<float>(length, Allocator.TempJob);
        NativeArray<float> result = 
            new NativeArray<float>(length, Allocator.TempJob);
        
        // 복제
        for (int i = 0; i < length; i++)
        {
            a[i] = aValue[i];
            b[i] = bValue[i];
        }

        AddArrayParalleJob addArrayParalleJob = new AddArrayParalleJob();
        addArrayParalleJob.a = a;
        addArrayParalleJob.b = b;
        addArrayParalleJob.result = result;

        JobHandle handle = addArrayParalleJob.Schedule(result.Length, 1);
        handle.Complete();

        a.Dispose();
        b.Dispose();
        result.Dispose();
    }
}

[BurstCompile]
public struct AddArrayParalleJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float> a;
    [ReadOnly]
    public NativeArray<float> b;
    public NativeArray<float> result;
    
    public void Execute(int index)
    {
        result[index] = a[index] + b[index];
    }
}

