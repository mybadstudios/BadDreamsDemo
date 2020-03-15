using UnityEngine;
using UnityEngine.UI;
using MBS;

namespace Template_Beta
{
    public class UnlockManager : MonoBehaviour
    {

        [SerializeField] Image icon;
        [SerializeField] Text cost_text;
        [SerializeField] GameObject cost_obj, blocker;
        [SerializeField] EWeaponType weapon_type;
        WeaponInstance weapon = null;

        void Awake() => weapon = Data.Instance.Ammo [(int)weapon_type];

        void OnEnable()
        {
            if ( null == Data.Stock )
                return;

            //don't allow it to be unlocked twice
            blocker.SetActive( !Data.Unlocked.Bool( weapon.Weapon.WeaponName ) && (WULogin.Cash(1) < weapon.Weapon.UnlockCost) || Data.Unlocked.Bool(weapon.Weapon.WeaponName));

            cost_obj.SetActive( !Data.Stock.defined.ContainsKey( weapon.Weapon.WeaponName ) );
            cost_text.text = $"Unlock\n{weapon.Weapon.UnlockCost}";
        }

        public void UnlockWeapon()
        {
            if ( WULogin.Cash(1) < weapon.Weapon.UnlockCost || Data.Unlocked.Bool( weapon.Weapon.WeaponName ) )
                return;

            WUMoney.SpendCurrency( weapon.Weapon.UnlockCost, "dust", $"unlock,{(int)weapon_type}" );
        }
    }
}