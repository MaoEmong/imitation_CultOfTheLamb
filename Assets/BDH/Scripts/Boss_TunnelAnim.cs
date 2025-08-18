using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_TunnelAnim : MonoBehaviour
{
	// 애니메이션 제어
	public SkeletonAnimation skel;

	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset animCilp;

	void Start()
    {
		skel = GetComponent<SkeletonAnimation>();
	}

	// 활성화 할 때 마다 애니매이션 재생
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
