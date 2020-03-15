using UnityEngine;

public class RotateIndefinitely : MonoBehaviour {

    Transform t;
    [SerializeField] float speed = 50f;

    void Start() => t = transform;
    void Update() => t.Rotate( 0f, 0f, -speed * Time.deltaTime );
}
