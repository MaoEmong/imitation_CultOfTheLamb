using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 팝업을 일저이간동안 액티브
// 검색을해 해당 오브젝트의 상태를 검사
// 검사 결과 액티브 fasle 일때 true로 전환
public class ksjPopUpPanel : MonoBehaviour
{
    public List<GameObject>popupUI;
    public List<GameObject> PopupUI
    {
        get { return popupUI; }      
        set { popupUI = value; }
    }

    [SerializeField]
    private GameObject prefab;

    /// <summary>
    /// 플레이어 매니저에서 AddItemToInven에 의해 자동으로 호출됩니다.
    /// </summary>
    /// <param name="itemcode">인벤토리에 추가되었던 팝업오브젝트에 보여줄 아이템의 코드</param>
    /// <param name="count">인벤토리에 추가된 팝업오브젝트에 보여줄 아이템의 개수</param>
    public void InitPopUp(ItemCode itemcode, int count)
    {
        // 플레이어 매니저에서 AddItemToInven으로 호출되었으나,
        // 해당 아이템의 코드가 이미 팝업으로 재생되고 있을 경우엔,
        // 해당 팝업의 재생시간을 초기화해주고 데이터를 갱신하여 인스터스를 대신합니다.
        for(int i=0;i<popupUI.Count;i++)
        {
            if(popupUI[i].GetComponent<ksjPopUpObject>().ObjectCode == itemcode)
            {

                var pOjbect = popupUI[i].GetComponent<ksjPopUpObject>();
                pOjbect.RefreshPopUp(count);
                return;
            }
        }

        // 그렇지 않을경우 평범하게 인스턴스후, 데이터를 넣어줍니다.
        GameObject obj;
        obj = Instantiate(prefab);
        obj.transform.parent = transform;
        

        popupUI.Add(obj);
        obj.GetComponent<ksjPopUpObject>().SetPopUp(itemcode,count);

    }

    public void reStart()
    {
        var pm = ksjPlayerManager.Instance;

		pm.CurStage = 0;

		pm.ChangePlayerWeapon();

		// 대기방으로 이동
		SceneManager.LoadScene("LoadWaitRoom");
	}

}
