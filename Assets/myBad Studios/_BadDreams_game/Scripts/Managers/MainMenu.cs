using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using MBS;

namespace Template_Beta {
    public class MainMenu : MonoBehaviour {

        const int max_lives = 5;

        [SerializeField] WUUGLoginGUI gui;
        [SerializeField] WUScoringUGUI scoring;
        [SerializeField] WUTimer lives;
        [SerializeField] Transform scores_parent;
        [SerializeField] Image gravatar;
        [SerializeField] Image heart;
        [SerializeField] Image coins;
        [SerializeField] Text high_score_text;
        [SerializeField] Text username_text;
        [SerializeField] Text lives_text;
        [SerializeField] Text coins_text;

        void Start() {
            WULogin.onLoggedIn += ShowLoggedInMenu;
            WULogin.onLoggedOut += HideAvatar;
            WULogin.onGravatarSpriteFetched += SetGravatar;

            WUMoney.OnAwardCurrencyResponse += UpdateGUI;
            WUMoney.OnSpendCurrencyResponse += UpdateGUI;
            WUMoney.OnGetCurrencyBalanceResponse += UpdateGUI;
            WUMoney.OnAwardCurrencyResponseFailure += OnWUMErrorReceived;
            WUMoney.OnGetCurrencyBalanceResponseFailure += OnWUMErrorReceived;
            WUMoney.OnSpendCurrencyResponseFailure += OnWUMErrorReceived;

            if ( WULogin.logged_in )
            {
                //WUMoney.GetCurrencyBalance("dust");
                ShowLoggedInMenu();
            }
            else
                HideAvatar();
        }

        private void OnDestroy()
        {
            WULogin.onLoggedIn -= ShowLoggedInMenu;
            WULogin.onLoggedOut -= HideAvatar;
            WULogin.onGravatarSpriteFetched -= SetGravatar;

            WUMoney.OnAwardCurrencyResponse -= UpdateGUI;
            WUMoney.OnSpendCurrencyResponse -= UpdateGUI;
            WUMoney.OnGetCurrencyBalanceResponse -= UpdateGUI;
            WUMoney.OnAwardCurrencyResponseFailure -= OnWUMErrorReceived;
            WUMoney.OnGetCurrencyBalanceResponseFailure -= OnWUMErrorReceived;
            WUMoney.OnSpendCurrencyResponseFailure -= OnWUMErrorReceived;
        }

        void OnWUMErrorReceived( MBSEvent response )
        {
            print(response.details[0].String("Message"));
            WUMoney.GetCurrencyBalance("dust");
        }

        void UpdateGUI( MBSEvent response )
        {
            WULogin.fetched_info.Set( WULogin.CurrencyString( "dust" ), response.details [0].String() );
            ShowCoins();
        }

        void ShowCoins() => coins_text.text = $"{ WULogin.Cash( "dust" ) } Gold";

        void SetLivesTimer()
        {
            lives_text.text = ( lives.Value == max_lives ) ?
                $"{max_lives}/{max_lives}" :
                $"{lives.Value}/{max_lives}\n{lives.FormattedTimer}";
        }

        void SetGravatar( Sprite tex = null )
        {
            if ( null == tex )
            {
                if ( null != gravatar )
                {
                    gravatar.gameObject.SetActive( false );
                    gravatar.color = gravatar.color.SetAlpha( 0f );
                }
                return;
            }

            gravatar.gameObject.SetActive( true );
            gravatar.color = gravatar.color.SetAlpha( 1f );
            gravatar.sprite = WULogin.user_gravatar_sprite;
        }

        void HideAvatar( CML response = null )
        {
            SetGravatar();
            high_score_text.gameObject.SetActive( false );
            username_text.gameObject.SetActive( false );
            heart.gameObject.SetActive( false );
            coins.gameObject.SetActive( false );
            coins_text.gameObject.SetActive( false );
            lives_text.gameObject.SetActive( false );
        }

        void OnContactedLivesServer( CMLData data )
        {
            lives.onContactedServer -= OnContactedLivesServer;
            Data.menu_tests_passed++;
        }

        //after login I disable the prefab by default. So wait a frame or so and then show it again
        void ShowLoggedInMenu( CML response = null ) => StartCoroutine( __ShowPostLoginMenu() );

        IEnumerator __ShowPostLoginMenu()
        {
            //WUData.RemoveGameData( );
            //yield return new WaitForSeconds( 1f );

            gui.HideActiveScreen();
            Data.menu_tests_passed = 0;

            lives.onContactedServer += OnContactedLivesServer;
            lives.FetchStat();
            if ( !IsInvoking( "SetLivesTimer" ) )
                InvokeRepeating( "SetLivesTimer", 1f, 1f );
            lives_text.text = string.Empty;

            high_score_text.gameObject.SetActive( true );
            username_text.gameObject.SetActive( true );
            heart.gameObject.SetActive( true );
            coins.gameObject.SetActive( true );
            coins_text.gameObject.SetActive( true );
            lives_text.gameObject.SetActive( true );
            high_score_text.text = WULogin.highscore.ToString();
            username_text.text = WULogin.display_name;
            gravatar.sprite = WULogin.user_gravatar_sprite;
            ShowCoins();

            do
            {
                yield return new WaitForSeconds( 0.05f );
            }
            while ( Data.menu_tests_passed < 2 || gui.active_state == WUUGLoginGUI.eWULUGUIState.Active);
            gui.ShowPostLoginMenu();
        }

        public void ShowHighScores()
        {
            gui.ShowHighScoresScreen();
            WUScoring.onFetched += ShowScores;
            WUScoring.FetchScores();
        }

        void ShowScores( CML response )
        {
            WUScoring.onFetched -= ShowScores;
            scoring.OnFetched( response );
        }

        public void HideHighScores()
        {
            for ( int i = 0; i < scores_parent.childCount; i++ )
                Destroy( scores_parent.GetChild( i ).gameObject );

            scoring.onWindowClosed?.Invoke();
            scoring.gameObject.SetActive( false );

            gui.ShowPostLoginMenu();
        }

        async public void StartGame()
        {
            if ( lives.Value > 0
#if UNITY_EDITOR
                || Data.Instance.dev_mode
#endif
                )
            {
#if UNITY_EDITOR
                if ( !Data.Instance.dev_mode)
#endif
                    lives.SpendPoints( 1 );
                WUAchieveManager.Instance.UpdateKeys("Played");
                await Task.Delay( 100 );
                gui.DoResumeGame();
                Events.ResetAll();

                //if the player bought a serial then make sure they start with max ammo for their ammo level
                if ( WULogin.HasSerial )
                {
                    if ( Data.Unlocked.Bool( Data.Instance.Ammo[1].Weapon.WeaponName ) && Data.Instance.Ammo [1].Ammo < Data.Instance.Ammo [1].MaxAmmo )
                        Data.Instance.SetAmmoToMax( Data.Instance.Ammo [1].Weapon.Type );
                    if ( Data.Unlocked.Bool( Data.Instance.Ammo[2].Weapon.WeaponName ) && Data.Instance.Ammo [2].Ammo < Data.Instance.Ammo [2].MaxAmmo )
                        Data.Instance.SetAmmoToMax( Data.Instance.Ammo [2].Weapon.Type );
                }

                SceneLoader.Instance.LoadScene( ESceneNames.TheGame );
            }
        }
    }
}