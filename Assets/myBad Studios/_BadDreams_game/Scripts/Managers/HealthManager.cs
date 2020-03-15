using UnityEngine;
using UnityEngine.UI;

namespace Template_Beta
{
    public class HealthManager : MonoBehaviour
    {
        static public HealthManager Instance;

        [SerializeField] Slider healthSlider;
        [SerializeField] int startingHealth = 100;

        public float CurrentHealth => healthSlider.value;
        public bool IsDead => healthSlider.value <= 0f;

        void Start()
        {
            Instance = this;
            healthSlider.value = startingHealth;
            Events.onDamagePlayer += OnDamagePlayer;
        }

        void OnDamagePlayer( int amount )
        {
            if ( healthSlider.value <= 0 )
                return;

            healthSlider.value -= amount;
            if ( healthSlider.value <= 0f )
                Events.Trigger( Events.onGameOver );
        }
    }
}