using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Boss_Tunnel : MonoBehaviour
{
    public Boss_TunnelAnim[] tunnelList;

	bool nowEnd;

	void Update()
    {
		if (nowEnd)
		{
			nowEnd = false;
			gameObject.SetActive(false);

			/*
			for (int i = 0; i < tunnelList.Length; i++)
			{
				if (tunnelList[i].isActiveAndEnabled)
				{
					if (tunnelList[i].skel.state.GetCurrent(0).AnimationTime == tunnelList[i].skel.state.GetCurrent(0).AnimationEnd)
					{
						
					}
				}
			}
			*/
		}
		else
		{
			for (int i = 0; i < tunnelList.Length; i++)
			{
				if (tunnelList[i].isActiveAndEnabled)
				{
					if (tunnelList[i].skel.state.GetCurrent(0).AnimationTime > 0.4)
					{
						if (i + 1 != 4)
						{
							tunnelList[i + 1].enabled = true;
						}
						else if (i + 1 > 3)
						{
							tunnelList[0].enabled = true;
						}
					}
				}
				else
					tunnelList[0].enabled = true;
			}
		}
    }

    public void NowEnd()
    {
		nowEnd = true;
	}
}
