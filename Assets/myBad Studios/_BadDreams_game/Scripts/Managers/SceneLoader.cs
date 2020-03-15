using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MBS;

public enum ESceneNames { Bootstrap, MainMenu, TheGame }
public enum ESceneState { FadeOut, FadeIn, Switch, Loaded }

public class SceneLoader : MonoBehaviour {

    static public SceneLoader Instance;

    [SerializeField] Image modal_overlay;
    [SerializeField] float fade_speed = 2f;

    MBSStateMachine<ESceneState> scene_load_state;
    ESceneNames next_scene = ESceneNames.Bootstrap;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad( gameObject );
        scene_load_state = new MBSStateMachine<ESceneState>();
        scene_load_state.AddState( ESceneState.FadeIn, OnFadeInState);
        scene_load_state.AddState( ESceneState.FadeOut, OnFadeOutState);
        scene_load_state.AddState( ESceneState.Loaded, OnLoadedState);
        scene_load_state.AddState( ESceneState.Switch, OnSwitchState);
        scene_load_state.SetState( ESceneState.Loaded );
    }

    void SwitchState( ESceneState new_state )
    {
        if ( scene_load_state.CompareState( new_state ) )
            return;

        scene_load_state.SetState( new_state );
        scene_load_state.PerformAction();
    }

    IEnumerator FadeScene()
    {
        float progress = 0f;
        float value = modal_overlay.color.a;
        while ( !Mathf.Approximately( progress, 1f ) )
        {
            yield return 1;
            progress += Time.deltaTime * fade_speed;
            if ( progress > 1f )
                progress = 1f;
            value = scene_load_state.CompareState( ESceneState.FadeOut ) ? progress : 1f - progress;
            modal_overlay.color = modal_overlay.color.SetAlpha( value );
        }
        switch ( scene_load_state.CurrentState )
        {
            case ESceneState.FadeIn:
                SwitchState( ESceneState.Loaded );
                break;

            case ESceneState.FadeOut:
                SwitchState( ESceneState.Switch );
                break;
        }
    }

    void OnFadeInState() =>StartCoroutine( FadeScene() );
    void OnLoadedState() => modal_overlay.gameObject.SetActive( false );

    void OnFadeOutState()
    {
        modal_overlay.gameObject.SetActive( true );
        StartCoroutine( FadeScene() );
    } 

    void OnSwitchState()
    {
        for ( int i = 0; i < SceneManager.sceneCount; i++ )
        {
            Scene scene = SceneManager.GetSceneAt( i );
            if ( scene.name == next_scene.ToString() )
            {
                SceneManager.SetActiveScene( scene );
                SwitchState( ESceneState.FadeIn );
                return;
            }
        }
        StartCoroutine( WaitforSceneToLoad() );
    }

    IEnumerator WaitforSceneToLoad()
    {
        AsyncOperation als = SceneManager.LoadSceneAsync( (int)next_scene );
        als.allowSceneActivation = true;
        yield return als;
        SwitchState( ESceneState.FadeIn );
    }

    public void LoadScene( ESceneNames scene_name )
    {
        if ( next_scene == scene_name )
            return;
        if ( !gameObject.activeSelf )
            gameObject.SetActive( true );
        next_scene = scene_name;
        SwitchState( ESceneState.FadeOut );
    }

}
