using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Projectile_Simulation_Slerp
{
    public class ProjectileJobSlerp : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public int numProjectiles = 100;
        public Transform startPoint;
        public Transform endPoint;

        private TransformAccessArray transformAccessArray;
        private NativeArray<Vector3> initialPositions;

        private void OnEnable()
        {
            transformAccessArray = new TransformAccessArray(numProjectiles);
            initialPositions = new NativeArray<Vector3>(numProjectiles, Allocator.Persistent);

            for (int i = 0; i < numProjectiles; i++)
            {
                GameObject projectileObject = Instantiate(projectilePrefab, startPoint.position, Quaternion.identity);
                transformAccessArray.Add(projectileObject.transform);
                initialPositions[i] = startPoint.position;
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

            if (initialPositions.IsCreated)
            {
                initialPositions.Dispose();
            }
        }
        
        private void OnDestroy()
        {
            if (transformAccessArray.isCreated)
            {
                transformAccessArray.Dispose();
            }

            if (initialPositions.IsCreated)
            {
                initialPositions.Dispose();
            }
        }

        private void Update()
        {
            float currentTime = Time.time;

            // Create Job
            ApplyVelocityJob job = new ApplyVelocityJob()
            {
                time = currentTime,
                startPoint = startPoint.position,
                endPoint = endPoint.position,
                initialPositions = initialPositions,
            };

            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
        }
    }

    [BurstCompile]
    public struct ApplyVelocityJob : IJobParallelForTransform
    {
        public float time;
        public float3 startPoint;
        public float3 endPoint;
        [ReadOnly] public NativeArray<Vector3> initialPositions;

        public void Execute(int index, TransformAccess transform)
        {
            float someValue = math.sin(time);
            float originalValue = Mathf.Sin(someValue); // 예를 들어, -1에서 1의 범위를 가진 사인 함수 결과
            float t = (originalValue + 1) / 2; // 0에서 1로 정규화된 값
            
            Vector3 newPosition = Vector3.Slerp(startPoint, endPoint, t);
            transform.position = new Vector3(newPosition.x, initialPositions[index].y, transform.position.z);
        }
    }
}
