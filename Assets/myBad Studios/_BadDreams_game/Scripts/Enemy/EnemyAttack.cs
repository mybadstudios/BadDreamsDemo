using UnityEngine;

namespace Template_Beta
{
    public class EnemyAttack : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;    
        public int attackDamage = 10;

        GameObject player;

        [SerializeField] Animator anim;                              
        [SerializeField] EnemyHealth enemyHealth;

        void Awake() => player = GameObject.FindGameObjectWithTag( "Player" );
        void Start() => Events.onGameOver += OnGameOver;
        void OnDestroy() => Events.onGameOver -= OnGameOver;
        void OnGameOver() => anim.SetTrigger( "PlayerDead" );

        void OnTriggerEnter( Collider other ) 
        {
            if ( other.gameObject == player )
                InvokeRepeating( "AttackEnemy", 0, timeBetweenAttacks );
        }
        
        void OnTriggerExit (Collider other)
        {
            if ( other.gameObject == player)
                CancelInvoke();
        }

        void AttackEnemy()
        {
            if ( enemyHealth.currentHealth > 0 && !HealthManager.Instance.IsDead) 
                Events.Trigger( Events.onDamagePlayer, attackDamage );
            else
                CancelInvoke();
        }
    }
}