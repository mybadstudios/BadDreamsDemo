using UnityEngine;
using UnityEngine.UI;
using MBS;

namespace Template_Beta
{
    public class UpgradeManager : MonoBehaviour
    {
        const int max_upgrade_level = 4;

        [SerializeField] Image icon;
        [SerializeField] Text attack_text;
        [SerializeField] Text range_text;
        [SerializeField] Text ammo_text;
        [SerializeField] Text attack_cost_text;
        [SerializeField] Text range_cost_text;
        [SerializeField] Text ammo_cost_text;
        [SerializeField] GameObject attack_obj;
        [SerializeField] GameObject range_obj;
        [SerializeField] GameObject ammo_obj;
        [SerializeField] GameObject blocker;
        [SerializeField] EWeaponType weapon_type;
        WeaponInstance weapon = null;

        void Awake() => weapon = Data.Instance.Ammo [(int)weapon_type];
        void OnEnable() => UpdateFields();
        void Start() => Events.onStatUpgraded += UpdateFields;
        void OnDestroy() => Events.onStatUpgraded -= UpdateFields;

        void UpdateFields()
        {
            if ( null == Data.Stock )
                return;

            //can't upgrade it if the weapon hasn't been unlocked yet
            blocker.SetActive( !Data.Unlocked.Bool( weapon.Weapon.WeaponName ) && weapon.Weapon.Type != EWeaponType.Gun);

            icon.sprite = weapon.Weapon.Icon;
            attack_text.text = weapon.LevelDamage < max_upgrade_level ? $"{weapon.Damage} > {weapon.Weapon.Damage [weapon.LevelDamage + 1]}" : $"{weapon.Damage}";
            range_text.text = weapon.LevelRange < max_upgrade_level ? $"{weapon.Range} > {weapon.Weapon.Range [weapon.LevelRange + 1]}" : $"{weapon.Range}";
            ammo_text.text = weapon.LevelAmmo < max_upgrade_level ? $"{weapon.MaxAmmo} > {weapon.Weapon.MaxAmmo [weapon.LevelAmmo + 1]}" : $"{weapon.MaxAmmo}";
            attack_obj.SetActive( weapon.LevelDamage < max_upgrade_level );
            range_obj.SetActive( weapon.LevelRange < max_upgrade_level );
            ammo_obj.SetActive( weapon.LevelAmmo < max_upgrade_level );
            attack_cost_text.text = attack_obj.activeSelf ? weapon.UpgradeDamageCost.ToString() : string.Empty;
            range_cost_text.text = range_obj.activeSelf ? weapon.UpgradeRangeCost.ToString() : string.Empty;
            ammo_cost_text.text = ammo_obj.activeSelf ? weapon.UpgradeAmmoCost.ToString() : string.Empty;
        }

        public void UpgradeAttack()
        {
            int upgrade_cost = weapon.UpgradeDamageCost;
            if ( upgrade_cost <= 0 )
                return;
            string meta = $"upgrade,damage,{weapon.LevelDamage + 1},{(int)weapon_type}";
            WUMoney.SpendCurrency( upgrade_cost, "dust", meta );
        }

        public void UpgradeRange()
        {
            int upgrade_cost = weapon.UpgradeRangeCost;
            if ( upgrade_cost <= 0 )
                return;
            string meta = $"upgrade,range,{weapon.LevelRange + 1},{(int)weapon_type}";
            WUMoney.SpendCurrency( upgrade_cost, "dust", meta );
        }

        public void UpgradeMaxAmmo()
        {
            int upgrade_cost = weapon.UpgradeAmmoCost;
            if ( upgrade_cost <= 0 )
                return;
            string meta = $"upgrade,ammo,{weapon.LevelAmmo + 1},{(int)weapon_type}";
            WUMoney.SpendCurrency( upgrade_cost, "dust", meta );
        }

    }
}