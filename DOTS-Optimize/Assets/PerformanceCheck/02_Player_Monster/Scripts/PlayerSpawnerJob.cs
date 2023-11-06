using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace Player_Monster
{
    public class PlayerSpawnerJob : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private int numPlayers = 100;
        [SerializeField] private float spawnRadius = 30f;

        private NativeArray<Vector3> playerPositions;
        private NativeArray<float> randomAngles;
        private NativeArray<float> randomDistances;
        
        public bool isSpawnComplete = false;

        void Start()
        {
            playerPositions = 
                new NativeArray<Vector3>(numPlayers, Allocator.Persistent);
            randomAngles = 
                new NativeArray<float>(numPlayers, Allocator.Persistent);
            randomDistances = 
                new NativeArray<float>(numPlayers, Allocator.Persistent);
            
            for (int i = 0; i < numPlayers; i++)
            {
                randomAngles[i] = 
                    UnityEngine.Random.Range(0f, 2 * math.PI);
                randomDistances[i] = 
                    UnityEngine.Random.Range(0f, spawnRadius);
            }
            
            PlayerSpawnJob job = new PlayerSpawnJob
            {
                playerPositions = playerPositions,
                randomAngles = randomAngles,
                randomDistances = randomDistances
            };

            JobHandle jobHandle = job.Schedule(numPlayers, 64);
            jobHandle.Complete();
            
            for (int i = 0; i < numPlayers; i++)
            {
                Instantiate(playerPrefab, playerPositions[i], Quaternion.identity);
            }
            
            randomAngles.Dispose();
            randomDistances.Dispose();

            isSpawnComplete = true;
        }
        
        public NativeArray<Vector3> GetPlayerPositions()
        {
            return playerPositions;
        }
        
        private void OnDestroy()
        {
            if (playerPositions.IsCreated)
            {
                playerPositions.Dispose();
            }
        }
    }

    public struct PlayerSpawnJob : IJobParallelFor
    {
        public NativeArray<Vector3> playerPositions;
        [ReadOnly] public NativeArray<float> randomAngles;
        [ReadOnly] public NativeArray<float> randomDistances;

        public void Execute(int index)
        {
            float angle = randomAngles[index];
            float distance = randomDistances[index];

            float x = distance * math.cos(angle);
            float z = distance * math.sin(angle);

            playerPositions[index] = new Vector3(x, 0, z);
        }
    }
}