using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead_AnimScript : MonoBehaviour
{
    public SkeletonAnimation skel;
    public AnimationReferenceAsset Clip;

	//public GameObject blood;

	public IEnumerator startEff()
    {
		bool start = true;

        skel.state.SetAnimation(0, Clip, false).TimeScale = 1;

        while(start)
        {
			if (skel.state.GetCurrent(0).AnimationTime ==
			skel.state.GetCurrent(0).AnimationEnd)
			{
				start = false;

				//blood.SetActive(true);
			}

			yield return null;
		}
	}
}
