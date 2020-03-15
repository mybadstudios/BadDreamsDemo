using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Template_Beta
{
    public class MixLevels : MonoBehaviour
    {

        [SerializeField] AudioMixer masterMixer;
        [SerializeField] Slider music;
        [SerializeField] Slider sfx;

        public void SetSfxLvl( float sfxLvl )
        {
            Data.Settings?.Setf( "sfx", sfxLvl );
            masterMixer.SetFloat( "sfxVol", sfxLvl );
        }

        public void SetMusicLvl( float musicLvl )
        {
            Data.Settings?.Setf( "bgm", musicLvl );
            masterMixer.SetFloat( "musicVol", musicLvl );
        }

        public void OnConfirm() => Data.SavePlayerData();

        void Start()
        {
            sfx.value = Data.Settings.Float( "sfx" ) ;
            music.value = Data.Settings.Float( "bgm" ) ;
        }
    }
}