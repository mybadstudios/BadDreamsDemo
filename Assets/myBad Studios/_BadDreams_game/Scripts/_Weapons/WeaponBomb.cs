using UnityEngine;

namespace Template_Beta
{
    public class WeaponBomb : WeaponGrenade
    {
        override public void Shoot()
        {
            if ( Ammo <= 0 || !CanFire )
                return;

            LastFired = Time.time;

            Ammo--;
            Events.Trigger( Events.onBulletsUpdated, 2, Ammo );

            Transform t = Instantiate<Transform>( Weapon.BulletPrefab );
            StartCoroutine( ThrowProjectile( t ) );
        }
    }
}