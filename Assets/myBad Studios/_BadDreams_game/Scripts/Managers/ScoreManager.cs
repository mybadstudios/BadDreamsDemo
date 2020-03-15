using UnityEngine;
using UnityEngine.UI;
using MBS;

namespace Template_Beta
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] Text text;

        void Awake() => Data.score = 0;
        void Start()
        {
            Events.onUpdateScore = OnUpdateScore;
            Events.onGameOver += SubmitHighScore;
        }

        void OnUpdateScore( int amt )
        {
            Data.score += amt;
            text.text = $"Score: {Data.score}";
        }

        void SubmitHighScore()
        {
            if ( Data.score > WULogin.highscore )
            {
                WULogin.highscore = Data.score;
                WUScoring.SubmitScore( Data.score );
            }
        }
    }
}