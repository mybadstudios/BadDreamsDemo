using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MBS {
	public class BDDisplay : MonoBehaviour {

        [SerializeField] RectTransform content_area;
        [SerializeField] WUAView	view_prefab;
        [SerializeField] bool destroy_contents_on_load = true;

		void Start()
		{
            //wait until login was successful then download all keys
            //this is great during the demo but if you spawn your prefab(s) mid game
            //you might have to call it manually. Either way, see the FetchAwards function to see how
            WULogin.onLoggedIn += FetchAwards;

            //do you want to destroy any existing loaded child prefabs when you load this prefab?
            //personal taste. If the prefab never gets destroyed then you can choose to keep all
            //loaded achievements in place and on displaying the prefab just do a quick call to check
            //if any achievements need to be updated. Since I do this during my spawning and this is
            //just a single scene demo I prefer to start fresh every time this is spawned...
            WUAView [] all_views = content_area.GetComponentsInChildren<WUAView>();
			if (null != content_area && destroy_contents_on_load)
				foreach(WUAView view in all_views)
					Destroy (view.gameObject);

            if ( WULogin.logged_in )
                GenerateEntries( );
		}
        void OnDestroy() => WULogin.onLoggedIn -= FetchAwards;

        //fetch all achievements from the server
        //upon receiving the server response spawn a prefab to display each returned result
        void FetchAwards( CML response ) => WUAchieve.FetchEverything( GenerateEntries, ( CMLData error ) => Debug.LogWarning( "--------> Expected data! Make sure there are achievements on your site!" ) );
        async void GenerateEntries( CML response = null )
        {
            float now = Time.time;
            //in case we just logged in now and this function gets called before WUAchievementManager had a chance to run...
            while ( null == WUAchieveManager.Instance.all_awards || WUAchieveManager.Instance.all_awards.Count == 0 || null == WUAchieveManager.Instance.Keys || WUAchieveManager.Instance.Keys.Count == 0)
            {
                await Task.Delay( 50 );
                if ( Time.time > now + 10f )
                {
                    Debug.LogError( "FAILED TO FIND AWARDS" );
                    return;
                }
            }
            //extract the achievements to work with in this function
            List<CMLData> entries = WUAchieveManager.Instance.all_awards.Children( 0 );

            //make sure our scroll region can handle all the entries...
            GridLayoutGroup glg = content_area.GetComponent<GridLayoutGroup>();
            content_area.sizeDelta = new Vector2( content_area.sizeDelta.x, entries.Count * ( view_prefab.GetComponent<RectTransform>().sizeDelta.y + glg.spacing.y ) );
            content_area.sizeDelta = new Vector2( content_area.sizeDelta.x, entries.Count * ( glg.cellSize.y + glg.spacing.y ) );

            //and now spawn them...
            foreach ( CMLData entry in entries )
            {
                WUAView view = Instantiate( view_prefab );
                view.transform.SetParent( content_area, false );
                view.Fields = entry;
                view.Initialize();
            }
        }

        public void _updateAchievements( CML response )
        {
            //get the complete list of awarded achievements 
            string [] unlocked = response [0].String( "unlocked" ).Split( ',' );
            if ( unlocked.Length == 0 ) return;

            WUAView [] views = FindObjectsOfType<WUAView>();

            //scan through all unlocked achievements
            foreach ( string aid in unlocked )
            {
                foreach (WUAView view in views)
                {
                    if ( view.Fields.String( "aid" ) == aid )
                    {
                        //inside the gui object we linked the object to this achievement data block
                        //so work backwards and use the data block to determine which gui object to work on
                        view.Unlocked = true;
                        view.DisplayRelevantVersion();
                    }
                }
            }
        }
    }
}