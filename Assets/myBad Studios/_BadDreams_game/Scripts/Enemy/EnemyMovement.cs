using UnityEngine;
using UnityEngine.AI;

namespace Template_Beta
{
    public class EnemyMovement : MonoBehaviour
    {
        //Transform player;         
        EnemyHealth enemyHealth;       
        NavMeshAgent nav;               
        
        void Awake ()
        {
            //player = GameObject.FindGameObjectWithTag ("Player").transform;
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <NavMeshAgent> ();
        }

        void Update ()
        {
            if(enemyHealth.currentHealth > 0 && HealthManager.Instance.CurrentHealth > 0)
                nav.SetDestination ( Data.player.position);
            else
                nav.enabled = false;
        }
    }
}