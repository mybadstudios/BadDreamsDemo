using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour {

    Renderer shell_rend;
    [SerializeField] Transform shell;
    [SerializeField] new Light light;
    [SerializeField] float speed = 1.5f, scale = 3f;

	// Use this for initialization
	void Start () {
        shell_rend = shell.GetComponent<Renderer>();
        StartCoroutine( DoExplosion() );
	}

    IEnumerator DoExplosion()
    {
        //yield return new WaitForSeconds( 0.25f );
        float progress = 0f, starting_opacity = shell_rend.material.color.a;
        Vector3
            starting_scale = transform.localScale,
            final_scale = starting_scale * scale;

        while ( progress < 1f )
        {
            progress = Mathf.MoveTowards( progress, 1f, Time.deltaTime * speed );
            if ( Mathf.Approximately( progress, 1f ) )
                progress = 1f;
            shell_rend.material.color = shell_rend.material.color.SetAlpha( starting_opacity * ( 1f - progress) );
            shell.localScale = Vector3.Lerp( starting_scale, final_scale, progress );
            light.color = light.color.SetAlpha( 1f - progress );
            yield return null;
        }
        Destroy( gameObject );
    }
}
