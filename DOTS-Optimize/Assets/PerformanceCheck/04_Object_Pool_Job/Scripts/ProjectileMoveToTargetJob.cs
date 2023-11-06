using System.Collections;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Object_Pool_Job
{
    public class ProjectileMoveToTargetJob : MonoBehaviour
    {
        [SerializeField] private int numProjectiles = 100;
        private TransformAccessArray transformAccessArray;
        private NativeArray<Vector3> initialPositions;
        private NativeArray<Vector3> startPoint;
        private NativeArray<Vector3> targetPoint;
        private NativeArray<bool> hasArrivedIndexArray;

        // PlayerSpawnerJob에서 플레이어 위치를 가져올 변수
        private PlayerSpawnerJob playerSpawner;
        // monsterSpawnerJob에서 몬스터 위치를 가져올 변수
        private MonsterSpawnerJob monsterSpawner;
        // ObjectPoolmanager
        private PoolInfoSetsManager poolInfoSetsManager;  
        
        private JobHandle monsterSpawnerJobHandle;

        private bool isInit = false;

        private JobHandle applyVelocityJobHandle;
        private JobHandle setStartPointJobHandle;
        private JobHandle projectileJobHandle;
        
        private void Start()
        {
            // 플레이어 위치를 가져오기 위해 PlayerSpawnerJob를 찾아냄
            playerSpawner = FindObjectOfType<PlayerSpawnerJob>();
            // 몬스터 위치를 가져오기 위해 MonsterSpawnerJob를 찾아냄
            monsterSpawner = FindObjectOfType<MonsterSpawnerJob>();
            // 오브젝트 Pool
            poolInfoSetsManager = FindObjectOfType<PoolInfoSetsManager>();

            
            transformAccessArray = new TransformAccessArray(numProjectiles);
            hasArrivedIndexArray = new NativeArray<bool>(numProjectiles, Allocator.Persistent);

            StartCoroutine(CreateProjectile());
        }

        private IEnumerator CreateProjectile()
        {
            while (!playerSpawner.isSpawnComplete || !monsterSpawner.isSpawnComplete)
            {
                yield return null;
            }

            while (!poolInfoSetsManager.isSpawnComplete)
            {
                yield return null;
            }

            initialPositions = playerSpawner.GetPlayerPositions();
            startPoint = playerSpawner.GetPlayerPositions();
            targetPoint = monsterSpawner.GetMonsterPositions();

            for (int i = 0; i < numProjectiles; i++)
            {
                GameObject projectile = poolInfoSetsManager.Pop("Projectile_Job(Clone)");
                GLUtil.SetActiveObject(projectile, true);
                transformAccessArray.Add(projectile.transform);
            }
            
            InitTransformJob initTransformJob = new InitTransformJob()
            {
                initialPositions = startPoint,
            };
            JobHandle initProjectileHandle = initTransformJob.Schedule(transformAccessArray);
            initProjectileHandle.Complete();
            
            isInit = true;
        }

        private void OnDestroy()
        {
            if (transformAccessArray.isCreated)
            {
                transformAccessArray.Dispose();
            }

            if (hasArrivedIndexArray.IsCreated)
            {
                hasArrivedIndexArray.Dispose();
            }
        }
        
        private void Update()
        {
            if (!playerSpawner.isSpawnComplete || !monsterSpawner.isSpawnComplete)
            {
                return;
            }
            if (!isInit)
            {
                return;
            }
            
            float deltaTime = Time.deltaTime;
            
            ApplyVelocityJob applyVelocityJob = new ApplyVelocityJob()
            {
                deltaTime = deltaTime,
                startPoint = startPoint,
                targetPoint = targetPoint,
                hasArrivedIndexArray = hasArrivedIndexArray,
            };
            SetStartPointJob setStartPointJob = new SetStartPointJob()
            {
                initialPositions = startPoint,
                hasArrivedIndexArray = hasArrivedIndexArray,
            };
            // "SetStartPointJob"을 "ApplyVelocityJob"의 결과가 나온 뒤에 스케줄링
            applyVelocityJobHandle = applyVelocityJob.Schedule(transformAccessArray);
            setStartPointJobHandle = setStartPointJob.Schedule(transformAccessArray, applyVelocityJobHandle);
        }

        private void LateUpdate()
        {
            setStartPointJobHandle.Complete();
        }
    }

    [BurstCompile]
    public struct InitTransformJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> initialPositions;
        public void Execute(int index, TransformAccess transform)
        {
            transform.position = initialPositions[index];
        }
    }
    
    
    [BurstCompile]
    public struct ApplyVelocityJob : IJobParallelForTransform
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public NativeArray<Vector3> startPoint;
        [ReadOnly] public NativeArray<Vector3> targetPoint;
        public NativeArray<bool> hasArrivedIndexArray;

        public void Execute(int index, TransformAccess transform)
        {
            // 현재 위치에서 타겟 위치를 향하는 방향 벡터를 계산합니다.
            float3 directionToTarget = math.normalize(targetPoint[index] - startPoint[index]);
            float3 m_StartPos = new float3(transform.position);
            float3 vNextPos = (m_StartPos + directionToTarget * 10f * deltaTime);

            if (math.distance(startPoint[index], vNextPos)
                > math.distance(startPoint[index], targetPoint[index]))
            {
                hasArrivedIndexArray[index] = true;
            }
            else
            {
                hasArrivedIndexArray[index] = false;
                transform.position = vNextPos;
            }
        }
    }
    
    [BurstCompile]
    public struct SetStartPointJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> initialPositions;
        [ReadOnly] public NativeArray<bool> hasArrivedIndexArray;
        
        public void Execute(int index, TransformAccess transform)
        {
            if (hasArrivedIndexArray[index])
            {
                transform.position = initialPositions[index];
            }
        }
    }
}