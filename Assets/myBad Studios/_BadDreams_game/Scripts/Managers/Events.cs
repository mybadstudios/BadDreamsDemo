using System;

namespace Template_Beta
{
    static public class Events
    {

        static public Action
            onGameStart,
            onGameOver,
            onStatUpgraded
            ;

        static public Action<int>
            onShoot, //called when the "fire" button gets pressed        
            onUpdateScore,
            onDamagePlayer
            ;

        static public Action<int, int>
            onCurrencyUpdated, //called after a currency's value changed
            onBulletsUpdated
            ;

        static public void Trigger( Action action ) => action?.Invoke();
        static public void Trigger( Action<int> action, int val1 ) => action?.Invoke( val1 );
        static public void Trigger( Action<int, int> action, int val1, int val2 ) => action?.Invoke( val1, val2 );

        static public void ResetAll()
        {
            onGameStart = null;
            onGameOver = null;
            onStatUpgraded = null;

            onShoot = null;
            onUpdateScore = null;
            onDamagePlayer = null;

            onCurrencyUpdated = null;
            onBulletsUpdated = null;
        }
    }
}