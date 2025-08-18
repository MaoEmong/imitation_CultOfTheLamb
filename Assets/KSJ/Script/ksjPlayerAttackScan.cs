using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ksjPlayerAttackScan : MonoBehaviour
{
    private float maxRayDistance = 2f;

    private Vector3 rayVector;

    // ȭ�� ����� ���������� ȯ���� �� ������ �䱸�ؼ� �׳� ������ �ʵ�.
    private Vector3 fullHDSize = new Vector3(1920, 1080, 0);

    /// <summary>
    /// ĳ���� �������� ���콺�� ��ġ�� �÷��̾��� ���ݹ������� ��ȯ���ִ� �޼���
    /// </summary>
    private void SetRayDirection()
    {
        Vector3 mousePos = Input.mousePosition - new Vector3(fullHDSize.x / 2, fullHDSize.y / 2, 0);        
        rayVector.x = mousePos.x;
        rayVector.z = mousePos.y;
        rayVector.y = 0;
    }

    /// <summary>
    /// �÷��̾��� ���ø�ǿ��� �߻��� ����ĳ��Ʈ.
    /// �극��Ŀ�� ������Ʈ �Ǵ� ���ʹ̿� ��ȣ�ۿ��� ����.
    /// </summary>
    public void AttackRay()
    {
        SetRayDirection();

        RaycastHit hitInfo = new RaycastHit();

        // ���Ǿ��(������)�� ���ϴ�.
        if (Physics.SphereCast(transform.position, 0.5f, rayVector, out hitInfo, maxRayDistance))
        {
            if (hitInfo.transform.GetChild(0).gameObject.name == "HitImpact")
            {
				hitInfo.transform.GetChild(0).gameObject.SetActive(true);
				hitInfo.transform.GetComponentInChildren<ksjHitImpact>().AwakeHitImpact();
			}

            if (hitInfo.transform.GetComponentInChildren<ksjHitImpact>() == null)
                Debug.Log("AttackTarget don't have HitImpact, AttackTarget : " + hitInfo.transform.gameObject.name);
            // ���̷� ��ȯ�� ������Ʈ�� ��Ʈ����Ʈ�� ������ ���� �� ��Ʈ����Ʈ ���
            else
            {
				
			}
                
            // ���̷� ��ȯ�� ������Ʈ�� �극��Ŀ���϶� �ش��ϴ� ������Ʈ�� ��Ʈ���or��ƼŬ�� ����մϴ�.
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObject"))
            {
                switch (hitInfo.transform.gameObject.tag)
                {
                    case "Grass":
                        hitInfo.transform.GetComponent<ksjGrassBreak>().HitObject();
                        break;
                    case "Tent":
                        hitInfo.transform.GetComponent<ksjTentBreak>().HitObject();
                        break;
                    case "WoodenRedStick":
                        hitInfo.transform.GetComponent<ksjWoodenRedStickBreak>().HitObject();
                        break;
                    case "Stone":
                        hitInfo.transform.GetComponent<ksjStoneBreak>().HitObject();
                        print("Stone");
                        break;
                    case "Stump":
                        hitInfo.transform.GetComponent<ksjTreeStumpBreak>().HitObject();
                        break;
                    default:
                        print("default");
                        break;
                }
            }
        }

    }
}
