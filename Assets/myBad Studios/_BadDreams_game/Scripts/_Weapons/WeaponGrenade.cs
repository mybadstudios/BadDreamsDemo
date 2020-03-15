using System.Collections;
using UnityEngine;
using MBS;

namespace Template_Beta
{
    public class WeaponGrenade : WeaponInstance
    {
        protected override void OnGameOver() { }
        override protected void OnGameStart() { }
        
        override public void Shoot()
        {
            if ( Ammo <= 0 || !CanFire )
                return;

            LastFired = Time.time;

            Ammo--;
            Events.Trigger( Events.onBulletsUpdated, 1, Ammo );

            Transform t = Instantiate<Transform>( Weapon.BulletPrefab );
            StartCoroutine( ThrowProjectile( t ) );
        }

        protected IEnumerator ThrowProjectile( Transform t )
        {
            t.position = Data.player.position + new Vector3( 0, 0.5f, 0 );
            Rigidbody r = t.GetComponentInChildren<Rigidbody>();
            r.AddForceAtPosition( ( Data.player.forward * 300f ) + new Vector3( 0, 1f ), t.position );

            float destroy_time = Time.time + 3f;
            while ( Time.time < destroy_time )
            {
                yield return null;
            }
            if ( null != ExplosionPrefab )
            {
                Explosion e = Instantiate<Explosion>( ExplosionPrefab, t.position, Quaternion.identity );
                e.transform.localScale *= Range;
            }

            DealRadialDamage( t.position );
            Destroy( t.gameObject );
        }

        void DealRadialDamage(Vector3 origin)
        {

            Collider[] all_enemies_in_range = Physics.OverlapSphere( origin, Range, shootableMask );
            foreach ( Collider c in all_enemies_in_range )
            {
                EnemyHealth enemyHealth = c.GetComponent<EnemyHealth>();
                if ( null != enemyHealth )
                {
                    Rigidbody rb = c.GetComponent<Rigidbody>();
                    rb?.AddExplosionForce( Damage * 500f, origin, Range, 1f, ForceMode.Impulse );
                    enemyHealth?.TakeDamage( Damage, c.transform.position + new Vector3(0f,0.25f) );
                }
            }
        }
    }
}