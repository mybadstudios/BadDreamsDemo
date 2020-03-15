using UnityEngine;

namespace Template_Beta
{
    public class PlayerShooting : MonoBehaviour
    {
        void Start() => Events.onShoot += OnShoot;
        void OnShoot( int weapon_type ) => Data.Instance.Ammo [weapon_type].Shoot();

        void Update()
        {
            if ( Time.timeScale == 0f )
                return;

#if !MOBILE_INPUT

            if ( Input.GetMouseButton( 0 ) && !Input.GetKey( KeyCode.LeftShift ) )
                Events.Trigger( Events.onShoot, 0 );

            //if you have a 2 button mouse, hold down shift and right click to throw a bomb
            if ( Input.GetMouseButtonDown( 1 ) )
                Events.Trigger( Events.onShoot, Input.GetKey( KeyCode.LeftShift ) ? 2 : 1 );

            //if you have a 3 button mouse, middle click to throw a bomb
            if ( Input.GetMouseButtonDown( 2 ) )
                Events.Trigger( Events.onShoot, 2 );

#else            
           // If there is input on the shoot direction stick and it's time to fire...
           if ( CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0 )
               Events.Trigger( Events.onShoot, selected_bullet_type );            
#endif
        }
    }
}