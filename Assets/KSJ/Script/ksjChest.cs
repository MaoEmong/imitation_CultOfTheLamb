using Spine.Unity;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum Reward
{
	Gold,
	Heart
}

public enum ChestSkin
{
	Wooden,
	Silver,
	Gold
}

/// <summary>
/// 체스트를 소환하고 싶은자리에 emptyobject
/// </summary>
public class ksjChest : MonoBehaviour
{
	[SerializeField]
	private SkeletonAnimation skelAnim;
	public Transform parentTransform;

	// 지급될 보상아이템의 수량
	[SerializeField]
	private int count;
	// 지급될 보상아이템의 코드
	private ItemCode code;
	// 
	[SerializeField]
	private GameObject item;
	[SerializeField]
	private GameObject heart;

	// 상자가 열렷는지 체크
	bool isOpened = false;
	bool isRevealed = false;

	// 하트 떨굴건지
	[SerializeField]
    bool isHeartDrop = false;
	[SerializeField]
	float heartRand;


    #region 체스트 애니메이션 제어필드
    [SerializeField]
	private AnimationReferenceAsset[] animCilp;
	public bool nowAnim;
	string currentAnimation;
	public ChestAnim chestAnim;
	#endregion

	Field field;

	public enum ChestAnim
	{
		Closed,
		Hidden,
		Open,
		Opened,
		Reveal
	}
	private void Start()
	{
		skelAnim = transform.GetComponent<SkeletonAnimation>();

		field = GetComponentInParent<Field>();
	}

	private void Update()
	{
		if (isOpened)
			return;

		if (skelAnim.state.GetCurrent(0).AnimationTime ==
			   skelAnim.state.GetCurrent(0).AnimationEnd)
		{
			if (chestAnim == ChestAnim.Open)
			{
				OpenChest();
			}
		}

		_setCurrentAnimation(chestAnim);
	}

	/// <summary>
	/// 최초에 체스트가 세팅될 때 호출되는 메서드.
	/// </summary>
	/// <param name="skin"></param>
	public void SetChest(ChestSkin skin, bool isGradedRandom)
	{

		if (isGradedRandom)
		{
			var rand = Random.Range(0, 100f);
			Debug.Log("RandomSetChestCompleted");
			if (rand < 70)
			{
				skelAnim.initialSkinName = "Wooden";
				chestAnim = ChestAnim.Hidden;
				SetReward(ChestSkin.Wooden);
			}
			else
			{
				skelAnim.initialSkinName = "Silver";
				chestAnim = ChestAnim.Hidden;
				SetReward(ChestSkin.Silver);
			}

		}
		else
		{
			switch (skin)
			{
				case ChestSkin.Gold:
					skelAnim.initialSkinName = "Gold";
					chestAnim = ChestAnim.Closed;
					SetReward(skin);
					break;
				case ChestSkin.Silver:
					skelAnim.initialSkinName = "Silver";
					chestAnim = ChestAnim.Hidden;
					SetReward(skin);
					break;
				case ChestSkin.Wooden:
					skelAnim.initialSkinName = "Wooden";
					chestAnim = ChestAnim.Hidden;
					SetReward(skin);
					break;
			}
		}

		GameObject obj;
		obj = Instantiate(gameObject);
		obj.transform.parent = parentTransform;
		obj.transform.localPosition = parentTransform.localPosition;
	}

	/// <summary>
	/// 체스트의 타입에 따른 드랍테이블 설정해주는 메서드
	/// </summary>
	/// <param name="skin"></param>
	public void SetReward(ChestSkin skin)
	{
		int itemCount;
		
		var asd =  Random.Range(0f, 100f);
		heartRand = asd;

        int itemCode;

		// 골드 상자에 따라 드랍테이블 확률 설정
		switch (skin)
		{
			case ChestSkin.Gold:
				itemCode = Random.Range(0, 1);
				code = (ItemCode)itemCode;
				itemCount = Random.Range(5, 7);
				count = itemCount;
                Debug.Log("Rand : " + asd);
                if (asd <= 100)
                {
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = true;
                }
				else
				{
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = false;
                }

                break;
			case ChestSkin.Silver:
				itemCode = Random.Range(0, 1);
				code = (ItemCode)itemCode;
				itemCount = Random.Range(3, 5);
				count = itemCount;
                Debug.Log("Rand : " + asd);
                if (asd <= 33)
                {
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = true;
                }
                else
                {
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = false;
                }
                break;
			case ChestSkin.Wooden:
				itemCode = Random.Range(0, 1);
				code = (ItemCode)itemCode;
				itemCount = Random.Range(1, 2);
				count = itemCount;
                Debug.Log("Rand : " + asd);
                if (asd <= 11)
                {
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = true;
                }
                else
                {
                    Debug.Log("Rand : " + asd);
                    isHeartDrop = false;
                }
                break;
		}
	}

	/// <summary>
	/// 체스트가 오픈될 때 호출될 메서드.
	/// </summary>
	public void OpenChest()
	{
        Debug.Log("heartDropped : "+ isHeartDrop+", heartRand : "+ heartRand);

		GameObject obj;
		obj = Instantiate(item);
		obj.transform.parent = gameObject.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.position += new Vector3(-0.5f, 0.5f, -0.7f) ;

		obj.GetComponent<ItemObject>().Count = this.count;
		obj.GetComponent<ItemObject>().ObjectCode = code;
		CallChestSound(ChestAnim.Open);

		if(isHeartDrop)
		{
			
			GameObject hObj;
			hObj = Instantiate(heart);
			hObj.transform.parent = gameObject.transform;
            hObj.transform.localPosition = Vector3.zero;
            hObj.transform.position += new Vector3(0.5f, 0.5f, -0.7f);
		}

        isOpened = true;
	}

	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		if(chestAnim == ChestAnim.Reveal && isRevealed==false)
		{
			CallChestSound(ChestAnim.Reveal);
		}

		if (nowAnim)
		{
			if (skelAnim.state.GetCurrent(0).AnimationTime ==
				skelAnim.state.GetCurrent(0).AnimationEnd)
			{
				if (chestAnim == ChestAnim.Reveal)
				{
					chestAnim = ChestAnim.Open;
					Debug.Log(chestAnim.ToString());
				}
					
				nowAnim = false;
			}

			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}

		// 현재 재생중인 애니메이션이 다시 재생될 때
		if (Clip.name.Equals(currentAnimation))
			// 애니메이션 전환 X
			return;

		// 해당 애니메이션 재생
		skelAnim.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// 현재 애니메이션의 이름을 저장
		currentAnimation = Clip.name;
	}

	void _setCurrentAnimation(ChestAnim state)
	{
		// 현재 상태
		switch (state)
		{
			// IDLE
			case ChestAnim.Hidden:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[(int)ChestAnim.Hidden], true, 1);
				break;
			case ChestAnim.Reveal:
				// REVEAL 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[(int)ChestAnim.Reveal], false, 1);
				if (skelAnim.state.GetCurrent(0).AnimationTime !=
					skelAnim.state.GetCurrent(0).AnimationEnd)
					nowAnim = true;
				break;
			case ChestAnim.Open:
				// OPEN 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[(int)ChestAnim.Open], false, 1);
				nowAnim = true;
				break;
			case ChestAnim.Closed:
				// ROCK 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[(int)ChestAnim.Closed], true, 1);
				break;
		}
	}


	void DestroySelf()
	{
		Destroy(gameObject);
	}


	public void CallChestSound(ChestAnim CA)
	{
		switch(CA)
		{
			case ChestAnim.Reveal:
			SoundManager.Sound.Play("Object/Chest/Master.assets [629]");
				isRevealed= true;
				break;
			case ChestAnim.Open:
			SoundManager.Sound.Play("Object/Chest/Master.assets [713]");				
				break;
		}
	}

}
