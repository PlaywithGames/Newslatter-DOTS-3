using Unity.Collections;
using UnityEngine;

public class NativeContainerError : MonoBehaviour
{
    void Start()
    {
        NativeArray<float> nativeArray = 
            new NativeArray<float>(1, Allocator.Temp);
        nativeArray[0] = 3.0f;
        Debug.Log("nativeArray result : " + nativeArray[0]);
    }
}