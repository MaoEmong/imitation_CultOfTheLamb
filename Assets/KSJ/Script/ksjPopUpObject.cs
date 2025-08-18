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

    // �˾��� ���⿡ ���� Ÿ���ʵ�
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
    /// �÷��̾� �Ŵ������� �˾��� �̴ϼȶ������� �� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="itemcode"> �˾�������Ʈ�� �־��� �������� �ڵ�</param>
    /// <param name="count"> �˾�������Ʈ�� �־��� �������� ����</param>
    public void SetPopUp(ItemCode itemcode, int count)
    {
        objectCode = itemcode;

        popUpImage.GetComponent<UnityEngine.UI.Image>().sprite = Image[(int)itemcode];
        switch (itemcode)
        {
            case ItemCode.Gold:
                popUpNameText.GetComponent<TextMeshProUGUI>().text = "���";
                break;
            case ItemCode.Grass:
                popUpNameText.GetComponent<TextMeshProUGUI>().text = "Ǯ";                
                break;
        }
        itemCount = count;

        popUpCountText.GetComponent<TextMeshProUGUI>().text = "+" + itemCount.ToString();

        var pm = ksjPlayerManager.Instance;
        popUpCurCountText.GetComponent<TextMeshProUGUI>().text = pm.FindItem(itemcode).Count.ToString();

        // ������ ������ �Ϸ��� �˾��� ���
        RaisePopUp();

    }   
    

    private WaitForSeconds delay = new WaitForSeconds(0.005f);
    private WaitForSeconds shakeDelay = new WaitForSeconds(0.01f);

    /// <summary>
    /// �˾� ��� �޼���. ����ϱ��� �˾��� ���ʻ��¸� �������ݴϴ�.
    /// ���ʿ��� ���������� �����մϴ�.
    /// </summary>
    public void RaisePopUp()
    {
        allImage.transform.position = new Vector3(-600, 0, 0);
        curTime = 0;
        StartCoroutine(PopUp());
    }

    /// <summary>
    ///  �˾� ��� �޼���. �˾��̵� �� �����ð��� ����� ����� �� �ڵ����� ȣ��˴ϴ�.
    ///  �����ʿ��� �������� ������ϴ�.
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
    /// �˾��� �ö�� ������ ���� ������Ʈ�� ȹ������ ��� �ڵ����� ȣ��Ǵ� �޼����Դϴ�.
    /// �˾��� �����͸� �������ݴϴ�.
    /// �˾��� �����ݴϴ�.
    /// </summary>
    public void RefreshPopUp(int count)
    {
        // �̹� ���������� �˾����϶��� �ʱ�ȭ.
        StopAllCoroutines();
        allImage.transform.position = new Vector3(0, allImage.transform.position.y, allImage.transform.position.z);

        var pm = ksjPlayerManager.Instance;

        itemCount += count;
        popUpCountText.GetComponent<TextMeshProUGUI>().text = "+" + itemCount.ToString();
        popUpCurCountText.GetComponent<TextMeshProUGUI>().text = pm.FindItem(objectCode).Count.ToString();

        curTime = 0;
        StartCoroutine(PopShake());
    }

    // �˾�â�� ���������� �����Ű�� �ڷ�ƾ
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
    /// �˾�â�� �������� ��������ϴ� �ڷ�ƾ
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

    // �˾��� �����ִ� �ڷ�ƾ
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
