using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TapNum
{
    Tap0,
    Tap1,
    Tap2,
    Tap3
}
public class MenuSelectionTap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private GameObject selectedTap;

    [SerializeField]
    private TapNum curTap;

    bool isEntered = false;

    private void Update()
    {
        if (isEntered == true)
            return;

        var parent = GetComponentInParent<MenuSelectionPanel>();
        if(parent.GetClickedState(curTap)==true)
        {
            selectedTap.SetActive(true);            
        }        
        else if(parent.GetClickedState(curTap) == false)
        {
            selectedTap.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
        selectedTap.SetActive(true);

        SoundManager.Sound.Play("KSJ/UI/PointerOnUI");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var parent = GetComponentInParent<MenuSelectionPanel>();
        parent.RefreshTap(curTap);
        parent.ChangePanel(curTap);
        switch (curTap)
        {
            case TapNum.Tap0:
                parent.InfoCard.SetActive(true);
                break;

            case TapNum.Tap1:
                parent.InfoCard.SetActive(true);
                break;

            case TapNum.Tap2:
                parent.InfoCard.SetActive(false);
                break;

            case TapNum.Tap3:
                parent.InfoCard.SetActive(false);
                break;
        }

        var rand = Random.RandomRange(0,2);
        switch(rand)
        {
            case 0:
            SoundManager.Sound.Play("KSJ/UI/ChangeSelection3");
                break;
            case 1:
                SoundManager.Sound.Play("KSJ/UI/ChangeSelection4");
                break;
        }


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var parent = GetComponentInParent<MenuSelectionPanel>();

        if (parent.GetClickedState(curTap) == false)
            selectedTap.SetActive(false);

        isEntered = false;
    }

}
