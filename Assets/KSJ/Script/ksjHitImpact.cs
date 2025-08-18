using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

// 모든 오브젝트에서 히트 임팩트를 제어하기 위한 스크립트

public class ksjHitImpact : MonoBehaviour
{

    [SerializeField]
    private enum hitObjectType
    {
        BrkObject=7,
        Enemy
    }

    [SerializeField]
    private SkeletonAnimation hitSkelAnim;

    Spine.Animation curAnimation;

    // 애니메이션 재생제어용 필드
    bool isHit = false;

    private void Start()
    {
        hitSkelAnim = GetComponent<SkeletonAnimation>();

        // 오브젝트가 들어있는 부모의 레이어를 검사해서
        // 부모의 타입에 맞는 히트임팩트를 트랙에 넣어주기 위한 스위치
        switch(transform.parent.gameObject.layer)
        {
            case (int)hitObjectType.BrkObject:
                Debug.Log("HitSkelAnim's Parent is found : " + transform.parent.gameObject.layer);
                curAnimation = hitSkelAnim.skeletonDataAsset.GetAnimationStateData().SkeletonData.FindAnimation("HitFX_Weak");
                break;
            case (int)hitObjectType.Enemy:
                Debug.Log("HitSkelAnim's Parent is found : " + transform.parent.gameObject.layer);
                curAnimation = hitSkelAnim.skeletonDataAsset.GetAnimationStateData().SkeletonData.FindAnimation("HitFX_Blocked");
                break;
            default:
                Debug.Log("HitSkelAnim's Parent is found : " + transform.parent.gameObject.layer);
                break;
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isHit)
            return;

        // 0번 트랙 애니메이션이 최대재생시간을 넘었을때 히트임팩트 종료
        if(hitSkelAnim.state.GetCurrent(0).AnimationTime >= hitSkelAnim.state.GetCurrent(0).AnimationEnd)
        {
            SleepHitImpact();
        }
    }

    /// <summary>
    /// 플레이어 어택레이에서 사용될 히트임팩트 작동 메서드
    /// </summary>
    public void AwakeHitImpact()
    {
        isHit = true;
        hitSkelAnim.state.SetAnimation(0, curAnimation, false);        
    }

    /// <summary>
    /// 히트임팩트 스파인 애니메이션의 종료제어메서드
    /// </summary>
    private void SleepHitImpact()
    {
        hitSkelAnim.state.ClearTrack(0);
        isHit = false;

        gameObject.SetActive(false);
    }

  
}
