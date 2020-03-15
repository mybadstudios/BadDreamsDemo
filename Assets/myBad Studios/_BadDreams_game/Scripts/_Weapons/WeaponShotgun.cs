using UnityEngine;

namespace Template_Beta
{
    public class WeaponShotgun : WeaponInstance
    {
        Light faceLight;
        Light gunLight;

        Ray shootRay = new Ray();       
        RaycastHit shootHit;            
        float effectsDisplayTime = 0.2f;
        float timer;
        
        ParticleSystem gunParticles;
        LineRenderer gunLine;       
        AudioSource gunAudio;      
        Transform gun_barrel;

        override protected void OnGameOver() => DisableEffects();        

        override protected void OnGameStart( )
        {
            gunParticles = Data.player.GetComponentsInChildren<ParticleSystem>(true)[0];
            gunLine = Data.player.GetComponentsInChildren<LineRenderer>(true)[0];
            gun_barrel = gunLine.transform;

            AudioSource [] audio_sources = Data.player.GetComponentsInChildren<AudioSource>( true );
            foreach(AudioSource a in audio_sources)
                if (a.gameObject.name.ToLower().Contains("gun"))
                    gunAudio = a;

            Light [] lights = Data.player.GetComponentsInChildren<Light>(true);
            foreach ( Light l in lights )
            {
                string s = l.name.ToLower();
                if ( s.Contains( "facelight" ) )
                    faceLight = l;
                else
                    if ( s.Contains( "gun" ) )
                    gunLight = l;
            }
        }

        void Update()
        {
            timer += Time.deltaTime;

            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if ( timer >= Weapon.ShotDelay * effectsDisplayTime )
                DisableEffects();
        }

        public void DisableEffects()
        {
            if ( null != gunLine ) gunLine.enabled = false;
            if ( null != gunLight ) gunLight.enabled = false;
            if ( null != faceLight ) faceLight.enabled = false;
        }

        override public void Shoot()
        {
            if ( Ammo <= 0  || !CanFire)
                return;

            LastFired = Time.time;

            timer = 0f;

            //update bullet inventory and send an event for the GUI to catch
            Ammo--;
            Events.Trigger( Events.onBulletsUpdated, 0 , Ammo );

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition( 0, gun_barrel.position );

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = gun_barrel.position;
            shootRay.direction = gun_barrel.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if ( Physics.Raycast( shootRay, out shootHit, Range, shootableMask ) )
            {
                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
                enemyHealth?.TakeDamage( Damage, shootHit.point );

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition( 1, shootHit.point );
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
                gunLine.SetPosition( 1, shootRay.origin + shootRay.direction * Range );
        }
    }
}