using System.Collections;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Player_Monster_Projectile_Simulation
{
    public class ProjectileMoveToTargetJob : MonoBehaviour
    {
        [SerializeField]private GameObject projectilePrefab;
        [SerializeField]private int numProjectiles = 100;
        private TransformAccessArray transformAccessArray;
        private NativeArray<Vector3> initialPositions;
        private NativeArray<Vector3> startPoint;
        private NativeArray<Vector3> targetPoint;
        
        // PlayerSpawnerJob에서 플레이어 위치를 가져올 변수
        private PlayerSpawnerJob playerSpawner; 
        // monsterSpawnerJob에서 몬스터 위치를 가져올 변수
        private MonsterSpawnerJob monsterSpawner; 
        
        private JobHandle playerSpawnerJobHandle;
        private JobHandle monsterSpawnerJobHandle;
        private bool isInit = false;
        
        private void Start()
        {
            // 플레이어 위치를 가져오기 위해 PlayerSpawnerJob를 찾아냄
            playerSpawner = FindObjectOfType<PlayerSpawnerJob>();
            // 몬스터 위치를 가져오기 위해 MonsterSpawnerJob를 찾아냄
            monsterSpawner = FindObjectOfType<MonsterSpawnerJob>();

            transformAccessArray = 
                new TransformAccessArray(numProjectiles);
            initialPositions = 
                new NativeArray<Vector3>(numProjectiles, Allocator.Persistent);
            
            
            StartCoroutine(CreateProjectile());
        }

        private IEnumerator CreateProjectile()
        {
            while (!playerSpawner.isSpawnComplete 
                   || !monsterSpawner.isSpawnComplete)
            {
                yield return null;
            }
            
            startPoint = playerSpawner.GetPlayerPositions();
            targetPoint = monsterSpawner.GetMonsterPositions();
            
            for (int i = 0; i < numProjectiles; i++)
            {
                GameObject projectileObject = 
                    Instantiate(projectilePrefab, 
                        startPoint[i], Quaternion.identity);
                transformAccessArray.Add(projectileObject.transform);
            }

            isInit = true;
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
            if (!isInit)
            {
                return;
            }
            if (!playerSpawner.isSpawnComplete 
                || !monsterSpawner.isSpawnComplete)
            {
                return;
            }
            
            float deltaTime = Time.deltaTime;
            ApplyVelocityJob job = new ApplyVelocityJob()
            {
                deltaTime = deltaTime,
                startPoint = startPoint,
                targetPoint = targetPoint,
            };
            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
        }
    }
    
    [BurstCompile]
    public struct ApplyVelocityJob : IJobParallelForTransform
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public NativeArray<Vector3> startPoint;
        [ReadOnly] public NativeArray<Vector3> targetPoint;

        public void Execute(int index, TransformAccess transform)
        {
            // 현재 위치에서 타겟 위치를 향하는 방향 벡터를 계산합니다.
            float3 directionToTarget = math.normalize
                (targetPoint[index] - startPoint[index]);
            float3 m_StartPos = new float3(transform.position);
            float3 vNextPos = (m_StartPos + directionToTarget * 1.0f * deltaTime);

            if (math.distance(startPoint[index], vNextPos)
                > math.distance(startPoint[index], targetPoint[index]))
            {
                transform.position = targetPoint[index];
            }
            else
            {
                transform.position = vNextPos;
            }
        }
    }
}