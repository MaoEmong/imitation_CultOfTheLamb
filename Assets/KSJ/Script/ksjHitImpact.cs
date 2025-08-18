using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

// ��� ������Ʈ���� ��Ʈ ����Ʈ�� �����ϱ� ���� ��ũ��Ʈ

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

    // �ִϸ��̼� �������� �ʵ�
    bool isHit = false;

    private void Start()
    {
        hitSkelAnim = GetComponent<SkeletonAnimation>();

        // ������Ʈ�� ����ִ� �θ��� ���̾ �˻��ؼ�
        // �θ��� Ÿ�Կ� �´� ��Ʈ����Ʈ�� Ʈ���� �־��ֱ� ���� ����ġ
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

        // 0�� Ʈ�� �ִϸ��̼��� �ִ�����ð��� �Ѿ����� ��Ʈ����Ʈ ����
        if(hitSkelAnim.state.GetCurrent(0).AnimationTime >= hitSkelAnim.state.GetCurrent(0).AnimationEnd)
        {
            SleepHitImpact();
        }
    }

    /// <summary>
    /// �÷��̾� ���÷��̿��� ���� ��Ʈ����Ʈ �۵� �޼���
    /// </summary>
    public void AwakeHitImpact()
    {
        isHit = true;
        hitSkelAnim.state.SetAnimation(0, curAnimation, false);        
    }

    /// <summary>
    /// ��Ʈ����Ʈ ������ �ִϸ��̼��� ��������޼���
    /// </summary>
    private void SleepHitImpact()
    {
        hitSkelAnim.state.ClearTrack(0);
        isHit = false;

        gameObject.SetActive(false);
    }

  
}
