using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobDependent : MonoBehaviour
{
    private NativeArray<float> result;
    private JobHandle secondHandle;
    public struct AddOperationJob : IJob
    {
        public float a;
        public float b;
        public NativeArray<float> result;
        
        public void Execute()
        {
            result[0] = a + b;
        }
    }

    public struct AddOneJob : IJob
    {
        public NativeArray<float> result;
        
        public void Execute()
        {
            result[0] = result[0] + 1;
        }
    }
    
    void Update()
    {
        result = new NativeArray<float>(1, Allocator.TempJob);
        AddOperationJob jobData = new AddOperationJob()
        {
            a = 10,
            b = 12,
            result = result
        };
        
        // 첫번째 Job Schedule
        JobHandle firstHandle = jobData.Schedule();
        AddOneJob incJobData = new AddOneJob
        {
            result = result,
        };
        
        // 두 번째 Job과 의존성 설정
        secondHandle = incJobData.Schedule(firstHandle);
    }

    private void LateUpdate()
    {
        secondHandle.Complete();
        
        float aPlusBPlusOne = result[0];
        Debug.Log("aPlusBPlusOne : " + aPlusBPlusOne);

        result.Dispose();
    }
}
