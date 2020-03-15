using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Template_Beta
{
    public class ScreenFlash : MonoBehaviour
    {
        [SerializeField] float flashSpeed;
        [SerializeField] Color flashColour;
        Image damageImage;
        Coroutine flashing = null;

        void Start()
        {
            damageImage = GetComponent<Image>();
            Events.onDamagePlayer += FlashScreenOnDamage;
        }

        IEnumerator FlashDamage()
        {
            float progress = 1f;
            while ( !Mathf.Approximately( progress, 0f ) )
            {
                progress -= flashSpeed * Time.deltaTime;
                damageImage.color = Color.Lerp( Color.clear, damageImage.color, progress );
                yield return null;
            }
            flashing = null;
        }

        void FlashScreenOnDamage(int ignore)
        {
            if ( null != flashing )
                StopCoroutine( flashing );
            damageImage.color = flashColour;
            flashing = StartCoroutine( FlashDamage() );
        }

    }
}