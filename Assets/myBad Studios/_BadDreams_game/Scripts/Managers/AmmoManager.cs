using UnityEngine;
using UnityEngine.UI;

namespace Template_Beta
{
    public class AmmoManager : MonoBehaviour
    {
        [SerializeField] Button[] ammo_buttons;
        [SerializeField] Text[] ammo_quantities;

        void Start() => Events.onBulletsUpdated += UpdateBullets;

        void UpdateBullets( int index, int value )
        {
            ammo_buttons [index].interactable = value > 0;
            ammo_quantities [index].text = value.ToString();
        }

        public void FireWeapon( int index )
        {
            #if UNITY_IOS || UNITY_ANDROID
            Data.selected_bullet_type = index;            
            #endif
        }
    }
}