using UnityEngine;
using MBS;

namespace Template_Beta
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] AudioClip deathClip;                       
        [SerializeField] Animator anim;                             
        [SerializeField] AudioSource playerAudio;                   
        [SerializeField] PlayerMovement playerMovement;             
        [SerializeField] PlayerShooting playerShooting;

        void Awake ()
        {
            Events.onGameOver += Die;
            Events.onDamagePlayer += TakeDamage;
        }

        void TakeDamage (int amount)
        {
            if ( HealthManager.Instance.IsDead )
                return;

            // Play the hurt sound effect.
            playerAudio.Play ();
        }

        void Die ()
        {
            anim.SetTrigger ("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;

            //give the player cash to buy upgrades/stock with
            int points = Data.score / 10;
            if ( points > 0 )
                WUMoney.AwardCurrency( points, "dust" );
        }

    }
}