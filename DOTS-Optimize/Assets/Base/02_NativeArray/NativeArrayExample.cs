using Unity.Collections;
using UnityEngine;

public class NativeArrayExample : MonoBehaviour
{
    void Start()
    {
        NativeArrayIntExample();
        NativeArrayUIntExample();
        NativeArrayLongExample();
        NativeArrayFloatExample();
        NativeArrayDoubleExample();
        NativeArrayByteExample();
    }

    private void NativeArrayIntExample()
    {
        NativeArray<int> nativeArray = 
            new NativeArray<int>(1, Allocator.Temp);
        nativeArray[0] = 42; // 정수값으로 변경
        Debug.LogFormat("nativeArray int result : {0}", nativeArray[0]);
    }
    private void NativeArrayUIntExample()
    {
        NativeArray<uint> nativeArray = 
            new NativeArray<uint>(1, Allocator.Temp);
        nativeArray[0] = 100; // 양의 정수값으로 변경
        Debug.LogFormat("nativeArray uint result : {0}", nativeArray[0]);
    }
    private void NativeArrayLongExample()
    {
        NativeArray<long> nativeArray = 
            new NativeArray<long>(1, Allocator.Temp);
        nativeArray[0] = 9999999999L; // 긴 정수값으로 변경
        Debug.LogFormat("nativeArray long result : {0}", nativeArray[0]);
    }
    private void NativeArrayFloatExample()
    {
        NativeArray<float> nativeArray = 
            new NativeArray<float>(1, Allocator.Temp);
        nativeArray[0] = 3.14f; // 부동 소수점 값으로 변경
        Debug.LogFormat("nativeArray float result : {0}", nativeArray[0]);
    }
    private void NativeArrayDoubleExample()
    {
        NativeArray<double> nativeArray = 
            new NativeArray<double>(1, Allocator.Temp);
        nativeArray[0] = 3.14159265359; // 더블값으로 변경
        Debug.LogFormat("nativeArray double result : {0}", nativeArray[0]);
    }
    private void NativeArrayByteExample()
    {
        NativeArray<byte> nativeArray = 
            new NativeArray<byte>(1, Allocator.Temp);
        nativeArray[0] = 255; // 바이트 값으로 변경 (0에서 255 사이)
        Debug.LogFormat("nativeArray byte result : {0}", nativeArray[0]);
    }
}
