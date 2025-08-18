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
/// ü��Ʈ�� ��ȯ�ϰ� �����ڸ��� emptyobject
/// </summary>
public class ksjChest : MonoBehaviour
{
	[SerializeField]
	private SkeletonAnimation skelAnim;
	public Transform parentTransform;

	// ���޵� ����������� ����
	[SerializeField]
	private int count;
	// ���޵� ����������� �ڵ�
	private ItemCode code;
	// 
	[SerializeField]
	private GameObject item;
	[SerializeField]
	private GameObject heart;

	// ���ڰ� ���Ǵ��� üũ
	bool isOpened = false;
	bool isRevealed = false;

	// ��Ʈ ��������
	[SerializeField]
    bool isHeartDrop = false;
	[SerializeField]
	float heartRand;


    #region ü��Ʈ �ִϸ��̼� �����ʵ�
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
	/// ���ʿ� ü��Ʈ�� ���õ� �� ȣ��Ǵ� �޼���.
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
	/// ü��Ʈ�� Ÿ�Կ� ���� ������̺� �������ִ� �޼���
	/// </summary>
	/// <param name="skin"></param>
	public void SetReward(ChestSkin skin)
	{
		int itemCount;
		
		var asd =  Random.Range(0f, 100f);
		heartRand = asd;

        int itemCode;

		// ��� ���ڿ� ���� ������̺� Ȯ�� ����
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
	/// ü��Ʈ�� ���µ� �� ȣ��� �޼���.
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

			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}

		// ���� ������� �ִϸ��̼��� �ٽ� ����� ��
		if (Clip.name.Equals(currentAnimation))
			// �ִϸ��̼� ��ȯ X
			return;

		// �ش� �ִϸ��̼� ���
		skelAnim.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// ���� �ִϸ��̼��� �̸��� ����
		currentAnimation = Clip.name;
	}

	void _setCurrentAnimation(ChestAnim state)
	{
		// ���� ����
		switch (state)
		{
			// IDLE
			case ChestAnim.Hidden:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[(int)ChestAnim.Hidden], true, 1);
				break;
			case ChestAnim.Reveal:
				// REVEAL �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[(int)ChestAnim.Reveal], false, 1);
				if (skelAnim.state.GetCurrent(0).AnimationTime !=
					skelAnim.state.GetCurrent(0).AnimationEnd)
					nowAnim = true;
				break;
			case ChestAnim.Open:
				// OPEN �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[(int)ChestAnim.Open], false, 1);
				nowAnim = true;
				break;
			case ChestAnim.Closed:
				// ROCK �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
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
