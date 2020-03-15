using UnityEngine;
using System.Collections;

namespace Template_Beta
{
    public class EnemyHealth : MonoBehaviour
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.


        Animator anim;                              // Reference to the animator.
        AudioSource enemyAudio;                     // Reference to the audio source.
        ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
        Coroutine isDead = null;                    // Whether the enemy is dead.


        void Awake ()
        {
            anim = GetComponent <Animator> ();
            enemyAudio = GetComponent <AudioSource> ();
            hitParticles = GetComponentInChildren <ParticleSystem> ();
            capsuleCollider = GetComponent <CapsuleCollider> ();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;
        }

        public void TakeDamage (int amount, Vector3 hitPoint)
        {
            if ( null != isDead )
                return;

            // Play the hurt sound effect.
            enemyAudio.Play ();
            Events.Trigger( Events.onUpdateScore, scoreValue );

            // Set the position of the particle system to where the hit was sustained.
            // And play the particles.
            hitParticles.transform.position = hitPoint;
            hitParticles.Play();

            currentHealth -= amount;
            if ( currentHealth <= 0)
                isDead = StartCoroutine( Die() );
        }

        IEnumerator Die ()
        {
            capsuleCollider.isTrigger = true;
            anim.SetTrigger( "Dead" );

            MBS.WUAchieveManager.Instance.UpdateKeys( "Kills" );
            if (name.Contains( "Hellephant" ) )
                MBS.WUAchieveManager.Instance.UpdateKeys( "Hellephants" );

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            enemyAudio.clip = deathClip;
            enemyAudio.Play();

            GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
            GetComponent <Rigidbody> ().isKinematic = true;

            Events.Trigger( Events.onUpdateScore, scoreValue * 2);

            float time_of_destroy = Time.time + 2f;
            while ( Time.time < time_of_destroy )
            {
                transform.Translate( -Vector3.up * sinkSpeed * Time.deltaTime );
                yield return null;
            }
            Destroy (gameObject, 2f);
        }
    }
}