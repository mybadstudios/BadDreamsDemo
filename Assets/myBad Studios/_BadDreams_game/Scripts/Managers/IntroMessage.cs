using UnityEngine;

public class IntroMessage : MonoBehaviour {

    RectTransform r;
    [SerializeField] float speed = 50f;
    [SerializeField] ESceneNames next_scene = ESceneNames.MainMenu;

    void Start () => r = transform as RectTransform;
	
	void Update () {
        r.anchoredPosition = r.anchoredPosition.SetY( r.anchoredPosition.y - ( Time.deltaTime * speed ) );
        if ( r.anchoredPosition.y < -1080f )
        {
            SceneLoader.Instance.LoadScene( next_scene );
            gameObject.SetActive( false );
        }
	}
}
