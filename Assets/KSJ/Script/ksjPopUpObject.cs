using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ksjPopUpObject : MonoBehaviour
{
    [SerializeField]
    private GameObject back;
    [SerializeField]
    private GameObject popUpImage;
    [SerializeField]
    private GameObject popUpCountText;
    [SerializeField]
    private GameObject popUpNameText;
    [SerializeField]
    private GameObject popUpCurCountText;
    [SerializeField]
    private GameObject allImage;

    private ItemCode objectCode;
    public ItemCode ObjectCode
    {
        get { return objectCode; }
        set { objectCode = value; }
    }

    private int itemCount;
    public int ItemCount
    {
        get { return itemCount; }
        set { itemCount = value; }
    }


    [SerializeField]
    private Sprite[] Image;

    // 팝업의 연출에 사용될 타임필드
    private float curTime = 0;

    public GameObject PopUpImage 
    {
        get { return popUpImage; } 
        set { popUpImage = value; }
    }

    public GameObject PopUpCountText
    {
        get { return popUpCountText; }
        set { popUpCountText = value; }
    }

    public GameObject PopUpNameText
    {
        get { return popUpNameText; }
        set { popUpNameText = value; }
    }

    /// <summary>
    /// 플레이어 매니저에서 팝업을 이니셜라지즈한 후 자동으로 호출됩니다.
    /// </summary>
    /// <param name="itemcode"> 팝업오브젝트에 넣어줄 아이템의 코드</param>
    /// <param name="count"> 팝업오브젝트에 넣어줄 아이템의 개수</param>
    public void SetPopUp(ItemCode itemcode, int count)
    {
        objectCode = itemcode;

        popUpImage.GetComponent<UnityEngine.UI.Image>().sprite = Image[(int)itemcode];
        switch (itemcode)
        {
            case ItemCode.Gold:
                popUpNameText.GetComponent<TextMeshProUGUI>().text = "골드";
                break;
            case ItemCode.Grass:
                popUpNameText.GetComponent<TextMeshProUGUI>().text = "풀";                
                break;
        }
        itemCount = count;

        popUpCountText.GetComponent<TextMeshProUGUI>().text = "+" + itemCount.ToString();

        var pm = ksjPlayerManager.Instance;
        popUpCurCountText.GetComponent<TextMeshProUGUI>().text = pm.FindItem(itemcode).Count.ToString();

        // 데이터 세팅이 완료후 팝업을 재생
        RaisePopUp();

    }   
    

    private WaitForSeconds delay = new WaitForSeconds(0.005f);
    private WaitForSeconds shakeDelay = new WaitForSeconds(0.01f);

    /// <summary>
    /// 팝업 재생 메서드. 재생하기전 팝업의 기초상태를 설정해줍니다.
    /// 왼쪽에서 오른쪽으로 등장합니다.
    /// </summary>
    public void RaisePopUp()
    {
        allImage.transform.position = new Vector3(-600, 0, 0);
        curTime = 0;
        StartCoroutine(PopUp());
    }

    /// <summary>
    ///  팝업 재생 메서드. 팝업이된 후 일정시간이 경과해 사라질 때 자동으로 호출됩니다.
    ///  오른쪽에서 왼쪽으로 사라집니다.
    /// </summary>

    public void LowerPopUp()
    {
        StopCoroutine(PopUp());
        StopCoroutine(PopShake());

        var pup = GetComponentInParent<ksjPopUpPanel>();
        // 
        for (int i = 0; i < pup.popupUI.Count; i++) 
        {
            if(pup.popupUI[i].GetComponent<ksjPopUpObject>().objectCode == objectCode)
            {
                pup.popupUI.RemoveAt(i);
                break;
            }
        }

        StartCoroutine(PopDown());
    }

    /// <summary>
    /// 팝업이 올라와 있을때 같은 오브젝트를 획득했을 경우 자동으로 호출되는 메서드입니다.
    /// 팝업의 데이터를 갱신해줍니다.
    /// 팝업을 흔들어줍니다.
    /// </summary>
    public void RefreshPopUp(int count)
    {
        // 이미 리스프레쉬 팝업중일때도 초기화.
        StopAllCoroutines();
        allImage.transform.position = new Vector3(0, allImage.transform.position.y, allImage.transform.position.z);

        var pm = ksjPlayerManager.Instance;

        itemCount += count;
        popUpCountText.GetComponent<TextMeshProUGUI>().text = "+" + itemCount.ToString();
        popUpCurCountText.GetComponent<TextMeshProUGUI>().text = pm.FindItem(objectCode).Count.ToString();

        curTime = 0;
        StartCoroutine(PopShake());
    }

    // 팝업창을 오른쪽으로 등장시키는 코루틴
    IEnumerator PopUp()
    {
        var isCompleted = false;

        while (true) 
        {
            if (curTime == 0)
            {
                allImage.transform.position += new Vector3(700,0,0) * Time.deltaTime;
            }

            if(allImage.transform.position.x >=0)
            {
                curTime += Time.deltaTime;
                if(curTime>=2.5f)
                {
                    isCompleted = true;
                }
            }

            if(isCompleted)
            {
                LowerPopUp();
                break;
            }
            
             yield return delay;
        }
    }


    /// <summary>
    /// 팝업창을 왼쪽으로 사라지게하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator PopDown()
    {
        var isCompleted = false;
        var isMax = false;
        while (true)
        {
            var loDistance = 700 - allImage.transform.position.x;

            if (!isMax)
            {   
                allImage.transform.position += new Vector3(loDistance+100, 0, 0) * Time.deltaTime;
                if (allImage.transform.position.x >= 100)
                    isMax = true;
            }

            if(isMax)
            {
                allImage.transform.position += new Vector3(-loDistance-100 , 0, 0) * Time.deltaTime;
            }

            if (allImage.transform.position.x <= -500)
            {
                isCompleted= true;
                break;
            }


            yield return delay;
        }

        if(isCompleted)
        {
            Destroy(gameObject);
        }
    }

    // 팝업을 흔들어주는 코루틴
    IEnumerator PopShake()
    {
        var isCompleted = false;

        while (true)
        {
            curTime += Time.deltaTime;

            if (curTime <= 0.3f)
            {
                var rand = Random.Range(-8f, 8f);
                allImage.transform.position = new Vector3(rand, allImage.transform.position.y, allImage.transform.position.z);
            }
            else
                allImage.transform.position = new Vector3(0, allImage.transform.position.y, allImage.transform.position.z);

            if (curTime >=2.0f)
            {
                isCompleted = true;
            }

            if (isCompleted)
            {
                LowerPopUp();
                break;
            }


            yield return shakeDelay;
        }
    }


}
