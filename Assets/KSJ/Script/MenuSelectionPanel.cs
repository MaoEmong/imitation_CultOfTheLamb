using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectionPanel : MonoBehaviour
{
    public bool isClicked0 = false;
    public bool isClicked1 = false;
    public bool isClicked2 = false;
    public bool isClicked3 = false;

    [SerializeField]
    private GameObject[] menuPanel;

    [SerializeField]
    private GameObject inforCard;
    public GameObject InfoCard
    {
        get { return inforCard; }
            
    }


    // Start is called before the first frame update
    void Start()
    {
        RefreshTap(TapNum.Tap0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshTap(TapNum num)
    {
        switch(num)
        {
            case TapNum.Tap0:
                isClicked0 = true;
                isClicked1 = false;
                isClicked2 = false;
                isClicked3 = false;
                break;

            case TapNum.Tap1:
                isClicked0 = false;
                isClicked1 = true;
                isClicked2 = false;
                isClicked3 = false;
                break;

            case TapNum.Tap2:
                isClicked0 = false;
                isClicked1 = false;
                isClicked2 = true;
                isClicked3 = false;
                break;

            case TapNum.Tap3:
                isClicked0 = false;
                isClicked1 = false;
                isClicked2 = false;
                isClicked3 = true;
                break;
        }
    }

    public bool GetClickedState(TapNum num)
    {
        switch(num)
        {
            case TapNum.Tap0:
                return isClicked0;
            case TapNum.Tap1:
                return isClicked1;
            case TapNum.Tap2:
                return isClicked2;
            case TapNum.Tap3:
                return isClicked3;
        }        

        return false;
    }

    public void ChangePanel(TapNum num)
    {
        for(int i =0;i<menuPanel.Length;i++)
        {
            if((int)num != i)
            {
                menuPanel[i].SetActive(false);
                inforCard.GetComponent<ksjInfoCard>().RefreshInfoRect(num);

            }
            else
            {
                menuPanel[i].SetActive(true);
            }
        }
    }

}


