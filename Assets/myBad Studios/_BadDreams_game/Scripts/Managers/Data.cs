using UnityEngine;
using MBS;

namespace Template_Beta
{
    /// <summary>
    /// functions updating values shared across classes.
    /// I.e. this modifies Data.cs
    /// </summary>
    public class Data : MonoBehaviour
    {
        #region inventory category names
        const string category_name_settings = "Settings";
        const string category_name_unlocked = "Unlocked";
        const string category_name_inventory = "Inventory";
        const string category_name_max_ammo = "MaxAmmo";
        const string category_name_max_damage = "MaxDamage";
        const string category_name_max_range = "MaxRange";
        #endregion

        #region cross-class shared data
#if UNITY_EDITOR
        public bool dev_mode = false;
#endif
        static public CMLData Settings = null;
        static public CMLData Unlocked = null;
        static public CMLData Stock = null;
        static public CMLData AmmoLevels = null;
        static public CMLData DamageLevels = null;
        static public CMLData RangeLevels = null;
        static public int menu_tests_passed = 0;
        static public int score = 0;
        static public int selected_bullet_type = 0;

        static public Transform player;

        public WeaponInstance [] Ammo;
        #endregion

        #region Instance
        static Data _instance;
        static public Data Instance => _instance ?? FindOrCreateInstance();

        static Data FindOrCreateInstance()
        {
            _instance = FindObjectOfType<Data>();
            if ( null == _instance )
            {
                GameObject go = new GameObject( "Data" );
                _instance = go.AddComponent<Data>();
            }
            return _instance;
        }
        #endregion 

        Vector2Int temp_storage;

        void Start()
        {
            DontDestroyOnLoad( gameObject );

            WULogin.onLoggedIn += FetchUserData;
            WULogin.onLoggedOut += OnLoggedOut;

            WUMoney.OnSpendCurrencyResponse += OnSpentCurrency;
        }

        #region purchasing related
        /// <summary>
        /// Weapon's stats can only be upgraded to a certain level. This function updates the stat level
        /// </summary>
        /// <param name="stat">Which stat is being updated?</param>
        /// <param name="to_value">What will the new value be?</param>
        /// <param name="weapon_type">What weapon are we upgrading the stat for?</param>
        void UpgradeStat( string stat, int to_value , int weapon_type)
        {
            WeaponInstance weapon = Ammo [weapon_type];
            CMLData update = new CMLData();
            update.Seti( weapon.Weapon.WeaponName, to_value );
            string message = "";

            //update the value locally so we can play then also update the stat online for future play sessions
            switch ( stat.ToLower().Trim() )
            {
                case "ammo":
                    if ( weapon.LevelAmmo >= to_value )
                        return;
                    AmmoLevels.Seti( weapon.Weapon.WeaponName, to_value );
                    WUData.UpdateCategory( category_name_max_ammo, update );
                    message = $"Ammo slots updated to hold {weapon.MaxAmmo} {(weapon.Weapon.Type == EWeaponType.Gun ? "bullets" : $"{weapon.Weapon.Type.ToString()}'s")}";
                    break;

                case "range":
                    if ( weapon.LevelRange >= to_value )
                        return;
                    RangeLevels.Seti( weapon.Weapon.WeaponName, to_value );
                    WUData.UpdateCategory( category_name_max_range, update );
                    message = $"{weapon.Weapon.Type} now has an attack range of {weapon.Range}";
                    break;

                case "damage":
                    if ( weapon.LevelDamage >= to_value )
                        return;
                    DamageLevels.Seti( weapon.Weapon.WeaponName, to_value );
                    WUData.UpdateCategory( category_name_max_damage, update );
                    message = $"{weapon.Weapon.Type} now deals {weapon.Damage} points of damage";
                    break;

            }
            //display an on screen notification of the transaction being completed
            MBSNotification.SpawnInstance( FindObjectOfType<Canvas>(), new Vector2( 200f, -50f ), new Vector2( 0f, -50f ), "Purchase Successful", message, weapon.Weapon.Icon);

            //and now tell the game it's done also so the GUI can update itself and whatever else you want to happen at this point
            Events.Trigger( Events.onStatUpgraded );
        }

        /// <summary>
        /// Makes this weapon available for use. The "Ammo" array lists the weapons in the same order as the weapon type enum so the int value passed
        /// as a parameter to this function correlates 1:1 with the weapon's name in the enum as well as the array index. We thus use the int value
        /// to fetch the weapon from the Ammo array and then use the weapon definition contained therein to fetch the weapon name.
        /// Since the weapon name just returns the string value of the enum we could also have said ((WeaponType)index).ToString() but casting
        /// an int to an enum only to immediately cast it to a string... I simply felt that fetching a string value from the definitioon directly
        /// would look a bit easier onthe eyes and HELP with the comprehension of what was going on, rather than adding confusion to the procedings.
        /// </summary>
        /// <param name="weapon_type">Which weapon are we unlocking?</param>
        void UnlockWeapon(int weapon_type)
        {
            WeaponInstance weapon = Ammo [weapon_type];
            CMLData update = new CMLData();
            CMLData unlock = new CMLData();

            update.Seti( weapon.Weapon.WeaponName, 0 );
            unlock.Seti( weapon.Weapon.WeaponName, 1 );
            Unlocked.Seti( weapon.Weapon.WeaponName, 1 );
            Stock.Seti( weapon.Weapon.WeaponName, 0 );
            WUData.UpdateCategory( category_name_unlocked, unlock );
            WUData.UpdateCategory( category_name_inventory, update );


            //display a notification of a successful unlock
            MBSNotification.SpawnInstance( FindObjectOfType<Canvas>(), new Vector2( 200f, -50f ), new Vector2( 0f, -50f ), "Unlock Successful", $"You can now use {weapon.Weapon.WeaponName}s", weapon.Weapon.Icon );
        }

        /// <summary>
        /// Gun ammo is always reset to max each time you play so the gun ammo is never purchased. 
        /// Both the bombs and the grenades, however, need to be purchased before it can be used and is used up when you enter a level
        /// </summary>
        /// <param name="qty">Buy one or more?</param>
        /// <param name="weapon_type">Which weapon's ammo are we buying</param>
        void UpdateAmmo( int qty, int weapon_type )
        {
            //just to curb cheating a tad, before we update the online database with new stock, let's first fetch the current stock levels
            //and update what the server sends back. The updated fetched value now becomes the current value and is sent back to the server again
            temp_storage = new Vector2Int( weapon_type, qty );
            WUData.FetchField(Ammo[weapon_type].Weapon.WeaponName, category_name_inventory, response: _updateAmmo, errorResponse: _updateAmmoOnError );
        }

        // Special case! If user paid for the game, make sure he always starts with max ammo for his current weapon level
        public void SetAmmoToMax( EWeaponType weaponType )
        {
            WeaponInstance weapon = Ammo [(int)weaponType];
            CMLData update = new CMLData();

            //update the value locally also
            Stock.Seti( weapon.Weapon.WeaponName, weapon.MaxAmmo );
            update.Seti( weapon.Weapon.WeaponName, weapon.MaxAmmo );
            WUData.UpdateCategory( category_name_inventory, update );            
        }

        //NOTE: Possible exploit! User can play on multiple devices at once and if the price is set to 100 on all devices then a check to see if
        //the user has 100 currency and based on that the level is upgraded by 1 means they can upgrade to max and only pay the lowest price for each
        //upgrade level. The correct course of action would be to first fetch the inventory again, check what the current level is of whatever is being
        //upgraded, make sure the current cost is the correct cost for the item being purchased (updating the GUI if not the case) and only then attempt
        //to buy the item.
        //When dealing with money this would be the safest course of action but since this is just a demo made for a fun passtime I am not being that strict
        void _updateAmmo(CML response)
        { 
            //see what was already stored online and add one. Make sure you don't exceed the max level or you'll get array index out of bounds errors
            WeaponInstance weapon = Ammo [temp_storage.x];
            int new_value = Mathf.Min(weapon.MaxAmmo, weapon.Ammo + temp_storage.y);

            //update the value locally so we can play then update it online for future games also
            CMLData update = new CMLData();
            update.Seti( weapon.Weapon.WeaponName, new_value );
            Stock.Seti( weapon.Weapon.WeaponName, new_value );
            WUData.UpdateCategory( category_name_inventory, update );

            //display a notification of a successful purchase
            MBSNotification.SpawnInstance( FindObjectOfType<Canvas>(), new Vector2( 200f, -50f ), new Vector2( 0f, -50f ), "Purchase Successful", $"You now have {Stock.Int(weapon.Weapon.WeaponName)} {weapon.Weapon.WeaponName}s", weapon.Weapon.Icon );
        }

        void _updateAmmoOnError( CMLData response ) => Debug.LogWarning( response.String( "message" ) );

        /// <summary>
        /// Whenever you buy something in the store, this function is called upon successfully taking the player's currency
        /// Determine what the player did in the store and then call an appropriate function to complete the action
        /// </summary>
        /// <param name="response">The response from the server contains(in order):
        /// 1. What action the user took in the store: Upgrade a stat, unlock a weapon or buy ammo
        /// 2. For upgrades: Which stat; For unlocks: which weapon
        /// 3. For upgrades: To what level
        /// 4. for upgrades: Which weapon</param>
        void OnSpentCurrency( MBSEvent response )
        {
            string [] args = response.details [0].String( "meta" ).Split( ',' );
            switch ( args [0].ToLower().Trim() )
            {
                case "upgrade":
                    int amt = int.Parse( args [2] );
                    int type = int.Parse( args [3] );
                    UpgradeStat( args [1], amt , type);
                    break;

                case "unlock":
                    UnlockWeapon( int.Parse( args [1]) );
                    break;

                case "ammo":
                    int qty = int.Parse( args [1] );
                    int ammo_type = int.Parse( args [2] );
                    UpdateAmmo( qty, ammo_type );
                    break;
            }
        }
        #endregion

        public void SetAmmo(int index, int amount) => Ammo [index].Ammo = amount;
        void FetchUserData( CML response ) => WUData.FetchGameInfo( ParseUserData, errorResponse: GenerateFirstTimePlayerData);

        void GenerateFirstTimePlayerData( CMLData error )
        {
            WelcomeFirstTimePlayer();
            menu_tests_passed++;
        }

        void ParseUserData( CML response )
        {
            //make sure there is set data. If the WUData demo was run first and the GameID has not yet been changed
            //we  might be getting back a bunch of settings that doesn't incldue this game's settings...
            if ( response.Count < 4 || null == response.NodesWithField( "category", category_name_inventory ) )
            {
                WelcomeFirstTimePlayer();
            }
            else
            {
                //WUMoney.AwardCurrency( 1000, "dust" );
                Settings = response.NodeWithField( "category", category_name_settings ) ?? new CMLData();
                Unlocked = response.NodeWithField( "category", category_name_unlocked ) ?? new CMLData();
                Stock = response.NodeWithField( "category", category_name_inventory ) ?? new CMLData();
                AmmoLevels = response.NodeWithField( "category", category_name_max_ammo ) ?? new CMLData();
                DamageLevels = response.NodeWithField( "category", category_name_max_damage ) ?? new CMLData();
                RangeLevels = response.NodeWithField( "category", category_name_max_range ) ?? new CMLData();
            }

            menu_tests_passed++;
        }

        void OnLoggedOut( CML response )
        {
            SavePlayerData();
            Stock = AmmoLevels = DamageLevels = RangeLevels = null;
        }

        void WelcomeFirstTimePlayer()
        {
            WUMoney.AwardCurrency( 100, WULogin.CurrencyNames [1] );

            Stock = new CMLData();
            AmmoLevels = new CMLData();
            DamageLevels = new CMLData();
            RangeLevels = new CMLData();
            Settings = new CMLData();
            Unlocked = new CMLData();
            Settings.Seti( "bgm", 1 );
            Settings.Seti("sfx", 1);
            Stock.Seti( Ammo[0].Weapon.WeaponName, Ammo [0].MaxAmmo );
            SavePlayerData();
        }

        static public void SavePlayerData()
        {
            CMLData data = Stock.Copy( CMLCopyMode.no_id );
            data.Remove( "category" );
            WUData.UpdateCategory( category_name_inventory, data );

            data = Settings.Copy( CMLCopyMode.no_id );
            data.Remove( "category" );
            WUData.UpdateCategory( category_name_settings, data );

            if ( AmmoLevels.defined.Count > 1 )
            {
                data = AmmoLevels.Copy( CMLCopyMode.no_id );
                data.Remove( "category" );
                WUData.UpdateCategory( category_name_max_ammo, data );
            }

            if ( Unlocked.defined.Count > 1 )
            {
                data = Unlocked.Copy( CMLCopyMode.no_id );
                data.Remove( "category" );
                WUData.UpdateCategory( category_name_unlocked, data );
            }

            if ( RangeLevels.defined.Count > 1 )
            {
                data = RangeLevels.Copy( CMLCopyMode.no_id );
                data.Remove( "category" );
                WUData.UpdateCategory( category_name_max_range, data );
            }

            if ( DamageLevels.defined.Count > 1 )
            {
                data = DamageLevels.Copy( CMLCopyMode.no_id );
                data.Remove( "category" );
                WUData.UpdateCategory( category_name_max_damage, data );
            }

        }

    }
}