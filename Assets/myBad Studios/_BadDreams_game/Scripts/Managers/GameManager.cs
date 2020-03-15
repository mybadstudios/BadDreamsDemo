using System.Collections;
using UnityEngine;

namespace Template_Beta
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] ESceneNames game_over_next_scene = ESceneNames.MainMenu;
        Data Inv => Data.Instance;

        Animator anim;

        void Awake () => Data.player = GameObject.FindGameObjectWithTag( "Player" ).transform;
        public void ReturnToMainMenu() => SceneLoader.Instance.LoadScene( game_over_next_scene );

        IEnumerator Start()
        {
            anim = GetComponent<Animator>();

            Events.onGameOver += () => anim.SetTrigger( "GameOver" );
            Events.onGameOver += Events.ResetAll;

            yield return new WaitForSeconds( 0.05f );
            Inv.SetAmmo( 0, Inv.Ammo [0].MaxAmmo );
            Inv.SetAmmo( 1, Mathf.Min( Data.Stock.Int( Inv.Ammo [1].Weapon.WeaponName ), Inv.Ammo [1].MaxAmmo ) );
            Inv.SetAmmo( 2, Mathf.Min( Data.Stock.Int( Inv.Ammo [2].Weapon.WeaponName ), Inv.Ammo [2].MaxAmmo ) );

            Events.Trigger( Events.onBulletsUpdated, 0, Inv.Ammo [0].Ammo );
            Events.Trigger( Events.onBulletsUpdated, 1, Inv.Ammo [1].Ammo );
            Events.Trigger( Events.onBulletsUpdated, 2, Inv.Ammo [2].Ammo );

            Events.Trigger( Events.onGameStart );
        }
    }
}