using UnityEngine;
using UnityEngine.SceneManagement;

namespace Template_Beta
{
    abstract public class WeaponInstance : MonoBehaviour
    {
        const int max_upgrade_level = 4;

        [SerializeField] WeaponType weapon;
        public WeaponType Weapon => weapon;

        [SerializeField] Explosion explosion_prefab;
        public Explosion ExplosionPrefab => explosion_prefab;

        public int Ammo { get { return Data.Stock.Int(weapon.WeaponName); } set { Data.Stock.Seti( weapon.WeaponName, value ); } }
        public int LevelAmmo => Data.AmmoLevels.Int( weapon.Type.ToString() );
        public int LevelRange => Data.RangeLevels.Int( weapon.Type.ToString() );
        public int LevelDamage => Data.DamageLevels.Int( weapon.Type.ToString() );

        public int UpgradeDamageCost => LevelDamage >= max_upgrade_level ? -1 : weapon.UpgradeDamageCost [LevelDamage];
        public int UpgradeRangeCost => LevelRange >= max_upgrade_level ? -1 : weapon.UpgradeRangeCost [LevelRange];
        public int UpgradeAmmoCost => LevelAmmo >= max_upgrade_level ? -1 : weapon.UpgradeAmmoCost [LevelAmmo];

        public int MaxAmmo => weapon.MaxAmmo [LevelAmmo];
        public int Damage => weapon.Damage [LevelDamage];
        public float Range => weapon.Range [LevelRange];

        public bool CanFire => Time.time > LastFired + weapon.ShotDelay;
        public float LastFired { get; set; } = -10f;

        protected int shootableMask;

        abstract public void Shoot();
        abstract protected void OnGameStart();
        abstract protected void OnGameOver();

        void Awake()
        {
            shootableMask = LayerMask.GetMask( "Shootable" );
            SceneManager.sceneLoaded += RegisterToEvents;
        }

        void RegisterToEvents( Scene scene, LoadSceneMode mode )
        {
            Events.onGameStart += OnGameStart;
            Events.onGameOver += OnGameOver;
        }

    }
}