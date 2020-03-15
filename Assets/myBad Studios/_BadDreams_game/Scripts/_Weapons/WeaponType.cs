using UnityEngine;

namespace Template_Beta
{
    public enum EWeaponType { Gun, Grenade, Bomb }
    [CreateAssetMenu (fileName ="WeaponData", menuName ="Weapon Data")]
    public class WeaponType : ScriptableObject
    {

        [SerializeField] EWeaponType weapon_type;
        [SerializeField] int[] max_ammo = new int[]{ 1,2,3,4,5 };
        [SerializeField] int[] damage = new int[]{ 20, 25, 30, 35, 40 }; // The damage inflicted by each bullet.
        [SerializeField] float[] range = new float[]{ 10f, 25f, 40f, 75f, 100f }; // The distance the gun can fire.
        [SerializeField] float time_between_shots = 0.15f; // The time between each shot.
        [SerializeField] Transform bullet_prefab = null;
        [SerializeField] Sprite icon;

        [Header("Shop values")]
        [SerializeField]
        int unlock_cost = 25;

        [SerializeField] int[] ammo_cost = new int[] { 100, 200, 350, 500};
        [SerializeField] int[] upgrade_ammo_cost = new int[] { 100, 500, 1000, 2000};
        [SerializeField] int[] upgrade_damage_cost = new int[] { 100, 500, 1000, 2000};
        [SerializeField] int[] upgrade_range_cost = new int[] { 100, 500, 1000, 2000};

        public EWeaponType Type => weapon_type;
        public int Typei => (int)weapon_type;
        public string WeaponName => weapon_type.ToString();
        public int [] MaxAmmo => max_ammo;
        public int [] Damage => damage;
        public float [] Range => range;
        public float ShotDelay => time_between_shots;
        public Transform BulletPrefab => bullet_prefab;
        public Sprite Icon => icon;

        public int UnlockCost => unlock_cost;
        public int [] AmmoCost => ammo_cost;
        public int [] UpgradeAmmoCost => upgrade_ammo_cost;
        public int [] UpgradeDamageCost => upgrade_damage_cost;
        public int [] UpgradeRangeCost => upgrade_range_cost;
    }
}