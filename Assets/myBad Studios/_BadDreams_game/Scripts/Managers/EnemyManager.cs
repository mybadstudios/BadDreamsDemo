using UnityEngine;

namespace Template_Beta
{
    public class EnemyManager : MonoBehaviour
    {
        public GameObject enemy;                // The enemy prefab to be spawned.
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

        void Start () => InvokeRepeating ("Spawn", spawnTime, spawnTime);

        void Spawn ()
        {
            if( !HealthManager.Instance || HealthManager.Instance.CurrentHealth <= 0f)
                return;

            int spawnPointIndex = Random.Range (0, spawnPoints.Length);
            Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
    }
}