using UnityEngine;
using UnityEngine.Audio;

namespace Template_Beta
{
    public class PauseManager : MonoBehaviour
    {

        public AudioMixerSnapshot paused;
        public AudioMixerSnapshot unpaused;

        Canvas canvas;

        void Start()
        {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }

        void Update()
        {
            if ( Input.GetKeyDown( KeyCode.Escape ) )
            {
                canvas.enabled = !canvas.enabled;
                Pause();
            }
        }

        public void Pause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            Lowpass();
        }

        void Lowpass()
        {
            if ( Time.timeScale == 0 )
                paused.TransitionTo( .01f );
            else
                unpaused.TransitionTo( .01f );
        }

        public void OnCancel()
        {
            Pause();
            Events.Trigger( Events.onGameOver );
            canvas.enabled = false;
        }
    }
}