using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ksjPlayerAttackScan : MonoBehaviour
{
    private float maxRayDistance = 2f;

    private Vector3 rayVector;

    // 화면 사이즈가 여러가지인 환경일 땐 구분을 요구해서 그냥 만들은 필드.
    private Vector3 fullHDSize = new Vector3(1920, 1080, 0);

    /// <summary>
    /// 캐릭터 기준으로 마우스의 위치를 플레이어의 공격방향으로 변환해주는 메서드
    /// </summary>
    private void SetRayDirection()
    {
        Vector3 mousePos = Input.mousePosition - new Vector3(fullHDSize.x / 2, fullHDSize.y / 2, 0);        
        rayVector.x = mousePos.x;
        rayVector.z = mousePos.y;
        rayVector.y = 0;
    }

    /// <summary>
    /// 플레이어의 어택모션에서 발생할 레이캐스트.
    /// 브레이커블 오브젝트 또는 에너미와 상호작용할 예정.
    /// </summary>
    public void AttackRay()
    {
        SetRayDirection();

        RaycastHit hitInfo = new RaycastHit();

        // 스피어레이(구형태)를 쏩니다.
        if (Physics.SphereCast(transform.position, 0.5f, rayVector, out hitInfo, maxRayDistance))
        {
            if (hitInfo.transform.GetChild(0).gameObject.name == "HitImpact")
            {
				hitInfo.transform.GetChild(0).gameObject.SetActive(true);
				hitInfo.transform.GetComponentInChildren<ksjHitImpact>().AwakeHitImpact();
			}

            if (hitInfo.transform.GetComponentInChildren<ksjHitImpact>() == null)
                Debug.Log("AttackTarget don't have HitImpact, AttackTarget : " + hitInfo.transform.gameObject.name);
            // 레이로 반환한 오브젝트가 히트임팩트를 가지고 있을 때 히트임팩트 재생
            else
            {
				
			}
                
            // 레이로 반환한 오브젝트가 브레이커블일때 해당하는 오브젝트의 히트모션or파티클을 재생합니다.
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
