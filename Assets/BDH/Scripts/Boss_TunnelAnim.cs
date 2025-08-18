using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_TunnelAnim : MonoBehaviour
{
	// �ִϸ��̼� ����
	public SkeletonAnimation skel;

	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset animCilp;

	void Start()
    {
		skel = GetComponent<SkeletonAnimation>();
	}

	// Ȱ��ȭ �� �� ���� �ִϸ��̼� ���
	private void OnEnable()
	{
		skel.state.SetAnimation(0, animCilp, false);
	}

	private void Update()
	{
		if (skel.state.GetCurrent(0).AnimationTime == skel.state.GetCurrent(0).AnimationEnd)
			this.enabled = false;
	}
}
