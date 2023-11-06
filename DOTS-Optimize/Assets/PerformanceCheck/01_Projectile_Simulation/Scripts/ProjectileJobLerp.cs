using System;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Jobs;

namespace Projectile_Simulation_Lerp
{
    public class ProjectileJobLerp : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public int numProjectiles = 100;
        public Transform startPoint;
        public Transform endPoint;

        private TransformAccessArray transformAccessArray;

        private void OnEnable()
        {
            transformAccessArray = new TransformAccessArray(numProjectiles);

            for (int i = 0; i < numProjectiles; i++)
            {
                GameObject projectileObject = 
                    Instantiate(projectilePrefab, 
                        startPoint.position, Quaternion.identity);
                transformAccessArray.Add(projectileObject.transform);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < transformAccessArray.length; i++)
            {
                if (transformAccessArray[i] != null)
                {
                    Destroy(transformAccessArray[i].gameObject);
                }
            }

            if (transformAccessArray.isCreated)
            {
                transformAccessArray.Dispose();
            }
        }

        private void OnDestroy()
        {
            if (transformAccessArray.isCreated)
            {
                transformAccessArray.Dispose();
            }
        }

        private void LateUpdate()
        {
            float currentTime = Time.time;

            // Create Job
            ApplyVelocityJob job = new ApplyVelocityJob()
            {
                time = currentTime,
                startPoint = startPoint.position,
                endPoint = endPoint.position,
            };

            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
        }
    }

    [BurstCompile]
    public struct ApplyVelocityJob : IJobParallelForTransform
    {
        [ReadOnly] public float time;
        [ReadOnly] public float3 startPoint;
        [ReadOnly] public float3 endPoint;
        
        static readonly IntPtr markerHandle = ProfilerUnsafeUtility.CreateMarker("ProjectileJob_Testing.ApplyVelocityJob", ProfilerUnsafeUtility.CategoryScripts, MarkerFlags.Default, 0);

        public void Execute(int index, TransformAccess transform)
        {
            ProfilerUnsafeUtility.BeginSample(markerHandle);
            
            float someValue = math.sin(time);
            // 예를 들어, -1에서 1의 범위를 가진 사인 함수 결과
            float originalValue = math.sin(someValue); 
            float t = (originalValue + 1) / 2; // 0에서 1로 정규화된 값

            float3 newPos = 
                math.lerp(startPoint, endPoint, t);
            transform.position = 
                new float3(newPos.x, newPos.y, newPos.z);
            
            ProfilerUnsafeUtility.EndSample(markerHandle);
        }
    }
}
