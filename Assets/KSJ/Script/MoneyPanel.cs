using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject believerText;
    [SerializeField]
    private GameObject moneyText;

    public void RefreshMoney()
    {
        var pm = ksjPlayerManager.Instance;
        if(pm.FindItem(ItemCode.Gold)==null)
        {
            moneyText.GetComponent<TextMeshProUGUI>().text = "0";
        }
        else
        {
            moneyText.GetComponent<TextMeshProUGUI>().text = pm.FindItem(ItemCode.Gold).Count.ToString();
        }
        
    }

}
