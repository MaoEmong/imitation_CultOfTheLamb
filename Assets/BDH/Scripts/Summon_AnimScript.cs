using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_AnimScript : MonoBehaviour
{
    public SkeletonAnimation SummonAnim;
    //public SkeletonAnimation GoopAnim;

	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] animCilp;

	public IEnumerator startEff()
    {
        float goopTime = 0;

        SummonAnim.state.SetAnimation(0, animCilp[0], false).TimeScale = 1;

		//GoopAnim.state.SetAnimation(0, animCilp[1], true).TimeScale = 1;

		while (goopTime > SummonAnim.state.GetCurrent(0).AnimationEnd)
        {

            goopTime += Time.deltaTime;

			yield return null;
		}

        Destroy(gameObject);
    }
}
