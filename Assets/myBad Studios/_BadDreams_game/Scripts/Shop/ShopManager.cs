using UnityEngine;
using UnityEngine.UI;
using MBS;

namespace Template_Beta {
    public class ShopManager : MonoBehaviour {

        [SerializeField] Sprite weapon_sprite;
        [SerializeField] GameObject blocker;
        [SerializeField] GameObject buy1_obj;
        [SerializeField] GameObject buyall_obj;
        [SerializeField] Text stock;
        [SerializeField] Text one;
        [SerializeField] Text max;
        [SerializeField] Text buy1;
        [SerializeField] Text buymax;
        [SerializeField] Image weapon_image;
        [SerializeField] EWeaponType weapon_type;

        WeaponInstance weapon;

        int buyable;

        void OnEnable() => SetStockTexts();
        void Start() => Events.onStatUpgraded += SetStockTexts;
        void OnDestroy() => Events.onStatUpgraded -= SetStockTexts;        

        void SetStockTexts()
        {
            weapon = Data.Instance.Ammo [(int)weapon_type];
            weapon_image.sprite = weapon.Weapon.Icon;

            //can't buy ammo if the weapon hasn't been unlocked yet
            blocker.SetActive( !Data.Unlocked.Bool( weapon.Weapon.WeaponName ) );

            //if the player paid for a serial, prevent the store from charging him and selling him ammo           
            if ( WULogin.HasSerial && Data.Unlocked.Bool( weapon.Weapon.WeaponName ) && weapon.MaxAmmo != Data.Stock.Int( weapon.Weapon.WeaponName ) )   
                   Data.Instance.SetAmmoToMax( weapon.Weapon.Type );

            int
                current_stock = Data.Stock.Int( weapon.Weapon.WeaponName );
            int max_stock = Data.Instance.Ammo [weapon.Weapon.Typei].MaxAmmo;
            int cost = weapon.Weapon.AmmoCost [weapon.LevelAmmo];

            buyable = max_stock - current_stock;

            stock.text = current_stock.ToString();
            max.text = buyable > 0 ? buyable.ToString() : "";
            buy1.text = cost.ToString();
            buymax.text = ( cost * buyable ).ToString();

            bool show = buyable > 0;
            buy1_obj.SetActive( show );
            buyall_obj.SetActive( show );
            one.gameObject.SetActive( show );
            max.gameObject.SetActive( show );
        }

        public void BuyAmmo(bool max)
        {
            int upgrade_cost = weapon.Weapon.AmmoCost[weapon.LevelAmmo] * (max ? buyable : 1);
            if ( upgrade_cost <= 0 )
                return;
            string meta = $"ammo,{(max ? buyable : 1)},{(int)weapon_type}";
            WUMoney.SpendCurrency( upgrade_cost, "dust", meta );
        }

    }
}