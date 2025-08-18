using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTapIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject basicOutLine;
    [SerializeField]
    private GameObject redOutLine;

    private void Start()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        basicOutLine.SetActive(false);
        redOutLine.SetActive(true);
        SoundManager.Sound.Play("KSJ/UI/PointerOnUI");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        basicOutLine.SetActive(true);
        redOutLine.SetActive(false);
    }
}
