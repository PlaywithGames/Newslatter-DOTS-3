using System.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace Player_Monster
{
    public class MonsterSpawnerJob : MonoBehaviour
    {
        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private int numMonsters = 100; // 몬스터 개수
        [SerializeField] private float spawnRadius = 30f;

        private NativeArray<Vector3> monsterPositions;
        private NativeArray<float> monsterAngles;
        private NativeArray<float> monsterDistances;
        
        // PlayerSpawnerJob에서 플레이어 위치를 가져올 변수
        private PlayerSpawnerJob playerSpawner; 

        void Start()
        {
            // 플레이어 위치를 가져오기 위해 PlayerSpawnerJob를 찾아냄
            playerSpawner = FindObjectOfType<PlayerSpawnerJob>();

            monsterPositions = new NativeArray<Vector3>
                (numMonsters, Allocator.Persistent);
            monsterAngles = new NativeArray<float>
                (numMonsters, Allocator.Persistent);
            monsterDistances = new NativeArray<float>
                (numMonsters, Allocator.Persistent);

            StartCoroutine(CreateMonster());
        }

        private IEnumerator CreateMonster()
        {
            // 플레이어가 생성된 후 몬스터 생성
            while (!playerSpawner.isSpawnComplete)
            {
                yield return null;
            }
            // 플레이어 위치를 가져옴
            NativeArray<Vector3> playerPositions = 
                playerSpawner.GetPlayerPositions();
            
            // 몬스터 플레이어와 중복되지 않은 위치에 생성
            for (int i = 0; i < numMonsters; i++)
            {
                float angle, distance;
                bool isOverlapping;
            
                do
                {
                    // 랜덤한 위치 계산
                    angle = UnityEngine.Random.Range(0f, 2 * math.PI);
                    distance = UnityEngine.Random.Range(0f, spawnRadius);
            
                    // 플레이어와 중복 확인
                    isOverlapping = false;
                    for (int j = 0; j < playerPositions.Length; j++)
                    {
                        if (math.distance(playerPositions[j], 
                                new Vector3(distance * math.cos(angle),
                                    0, distance * math.sin(angle))) < 2.0f)
                        {
                            isOverlapping = true;
                            break;
                        }
                    }
                } while (isOverlapping);
            
                monsterAngles[i] = angle;
                monsterDistances[i] = distance;
            }
            
            // Schedule Job
            MonsterSpawnJob job = new MonsterSpawnJob
            {
                monsterPositions = monsterPositions,
                monsterAngles = monsterAngles,
                monsterDistances = monsterDistances
            };
            
            JobHandle jobHandle = job.Schedule(numMonsters, 64);
            jobHandle.Complete();
            
            for (int i = 0; i < numMonsters; i++)
            {
                Instantiate(monsterPrefab, monsterPositions[i], Quaternion.identity);
            }
            
            monsterPositions.Dispose();
            monsterAngles.Dispose();
            monsterDistances.Dispose();
        }
    }

    public struct MonsterSpawnJob : IJobParallelFor
    {
        public NativeArray<Vector3> monsterPositions;
        [ReadOnly] public NativeArray<float> monsterAngles;
        [ReadOnly] public NativeArray<float> monsterDistances;

        public void Execute(int index)
        {
            float angle = monsterAngles[index];
            float distance = monsterDistances[index];

            float x = distance * math.cos(angle);
            float z = distance * math.sin(angle);

            monsterPositions[index] = new Vector3(x, 0, z);
        }
    }
}
