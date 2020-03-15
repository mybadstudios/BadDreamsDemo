using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour {

    [SerializeField] ESceneNames next_scene = ESceneNames.MainMenu;
    [SerializeField] GameObject intro_message;
    [SerializeField] bool show_intro = true;

    void Start ()
    {
        if ( show_intro )
            intro_message.SetActive( true );
        else
            SceneLoader.Instance.LoadScene( next_scene );
	}
}
