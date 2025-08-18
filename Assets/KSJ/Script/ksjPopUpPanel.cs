using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// �˾��� �����̰����� ��Ƽ��
// �˻����� �ش� ������Ʈ�� ���¸� �˻�
// �˻� ��� ��Ƽ�� fasle �϶� true�� ��ȯ
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
    /// �÷��̾� �Ŵ������� AddItemToInven�� ���� �ڵ����� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="itemcode">�κ��丮�� �߰��Ǿ��� �˾�������Ʈ�� ������ �������� �ڵ�</param>
    /// <param name="count">�κ��丮�� �߰��� �˾�������Ʈ�� ������ �������� ����</param>
    public void InitPopUp(ItemCode itemcode, int count)
    {
        // �÷��̾� �Ŵ������� AddItemToInven���� ȣ��Ǿ�����,
        // �ش� �������� �ڵ尡 �̹� �˾����� ����ǰ� ���� ��쿣,
        // �ش� �˾��� ����ð��� �ʱ�ȭ���ְ� �����͸� �����Ͽ� �ν��ͽ��� ����մϴ�.
        for(int i=0;i<popupUI.Count;i++)
        {
            if(popupUI[i].GetComponent<ksjPopUpObject>().ObjectCode == itemcode)
            {

                var pOjbect = popupUI[i].GetComponent<ksjPopUpObject>();
                pOjbect.RefreshPopUp(count);
                return;
            }
        }

        // �׷��� ������� ����ϰ� �ν��Ͻ���, �����͸� �־��ݴϴ�.
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

		// �������� �̵�
		SceneManager.LoadScene("LoadWaitRoom");
	}

}
