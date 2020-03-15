using UnityEngine;
using MBS;

public class Currency : MonoBehaviour {

    static Currency _instance;
    static public Currency Instance
    {
        get
        {
            if ( null == _instance )
            {
                GameObject go = new GameObject( "Currency" );
                _instance = go.AddComponent<Currency>();
            }
            return _instance;
        }
    }

    void Start () {
        DontDestroyOnLoad( gameObject );

        WUMoney.OnAwardCurrencyResponse += UpdateCurrency;
        WUMoney.OnSpendCurrencyResponse += UpdateCurrency;
        WUMoney.OnEarnedPoints += OnPointsEarned;

        WUMoney.OnGetCurrencyBalanceResponse += UpdateCurrency;
        WUMoney.OnAwardCurrencyResponseFailure += CurrencyError;
        WUMoney.OnSpendCurrencyResponseFailure += CurrencyError;
        WUMoney.OnGetCurrencyBalanceResponseFailure += CurrencyError;
    }

    void OnPointsEarned( MBSEvent response ) => WULogin.fetched_info?.Seti(WULogin.CurrencyString("dust"), response.details [0].Int( "total" ) ); 
    void CurrencyError( MBSEvent response )  => StatusMessage.Message = response.details [0].String( "message" );

    void UpdateCurrency( MBSEvent response ) 
    {
        string currency_name = response.details [0].String( "currency" );
        WULogin.fetched_info?.Seti( WULogin.CurrencyString( currency_name ), response.details [0].Int() );
    }   
}
