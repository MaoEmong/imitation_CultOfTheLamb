using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// �������� ���� �� ������ �ʵ� ��ũ��Ʈ
// �ش� ��ũ��Ʈ���� OpenDoor�� ��� / CloseDoor�� GroundOnlyTriggerCheck ��ũ��Ʈ���� �����
public class Field : MonoBehaviour
{
	// �������� �ѹ���
	public int StageNumber;

	//====================== �ʵ� ���� ����
	// 1. �̸� �����Ǵ� ���Ͱ� ������
	// 2. �ش� ���͵��� ���� ��ԵǸ� ���� 2�� ��ȯ ����
	// 3. 2�� ��ȯ ���ͱ��� ���� óġ �� �� Ŭ���� ����


	// �̸� ��ȯ�� �� ����
	public Enemy[] alreadyEnemys;
	// 2���� ��ȯ�� ����
	public Enemy[] SpawnEnemys;
	// ��ȯ�� ����
	public Boss_Logic Boss;
	// ���� ��ȯ ��ġ
	public Transform[] SpawnPos;

	// ���� ����Ʈ
	public GameObject SpawnObj;

	// �̸� ��ȯ�� �� ������ ��
	public int alreadyEnemyCount;
	// 2�� �����ϴ� ���� ��
	public int SpawnEnemyCount;
	// �� ���� ��
	public int totalEnemyCount;
	// ���� óġ���� ���� ������ ��
	public int CurEnemyCount;
	// 2�� ��ȯ�� �Ǿ����� Ȯ���ϱ� ���� bool
	public bool isSpawnEnemy = false;
	// ������ ����
	public bool isBossBattle = false;

	// �ʵ� ���� �����ִ� ��������Ʈ
	public GameObject[] Doors;
	// �ٴ� �ɺ�
	public GameObject[] DoorSymbols;
	// �ʵ忡 �����ϴ� ��Ż
	public Portal[] Portals;
	// ���� �ʵ带 Ŭ���� �ߴ��� Ȯ��(���� ����)
	public bool isCleared = false;
	// �ʵ忡 ���� �� ���� ��ȯ�� �ϱ� ���� bool��
	public bool isStartBattle = false;

	// �ʵ� 0 ����
	public Item_WeaponDrop weapon;

	// ����Ʈ�ѷ�
	MapController MC;

	private void Start()
	{
		ksjPlayerManager pm = GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>();

		if (pm.CurStage > 1)
		{
			if (StageNumber == 0 && weapon != null)
			{
				weapon.setLevel(Random.RandomRange(2, 5));
				weapon.setWeapon((WeaponType)Random.RandomRange(0, 3));
			}
		}
		else if (pm.CurStage == 0)
		{
			if (StageNumber == 0 && weapon != null)
			{
				weapon.setLevel(1);
				weapon.setWeapon((WeaponType)Random.RandomRange(0, 3));
			}
		}
	}

	private void Update()
	{
		CheckEnemys();
	}

	// �ʵ� �̴ϼȶ�����
	public void Init(MapController map)	
	{
		// �� ��Ʈ�ѷ� ����
		MC = map;

		// �̸� ��ȯ�Ǿ� �ִ� ������ ��
		alreadyEnemyCount = alreadyEnemys.Length;
		// 2���� ��ȯ�� ������ ��
		SpawnEnemyCount = SpawnEnemys.Length;
		// ������ �� ���� ��
		totalEnemyCount = alreadyEnemyCount + SpawnEnemyCount;
		// ���� óġ���� ���� ������ �� == ��ü ������ ��
		CurEnemyCount = totalEnemyCount;

		foreach (var n in alreadyEnemys)
			n.Init(this);

		// ���� Ŭ���� ������ False
		//isCleared = false;

		// ���� ������ �� ��������
		StartDoor();
	}

	// ���� ��ȯ �ڷ�ƾ
	// GroundOnlyTriggerCheck��ũ��Ʈ���� ���� ��ȯ ��� ��
	public IEnumerator SummonEnemy()
	{
		int i = 0;

		// Enemys �ݺ�
		foreach (var enemy in SpawnEnemys) 
		{
			// 0.3�� ������ �Ѹ����� ��ȯ
			yield return new WaitForSeconds(0.3f);

			var v = Instantiate(enemy);
			var s = Instantiate(SpawnObj);

			v.transform.position = SpawnPos[i].position;
			s.transform.position = SpawnPos[i].position;
			v.transform.rotation = Quaternion.identity;
			s.transform.rotation = Quaternion.identity;
			v.Init(this);

			v.transform.localScale = Vector3.one;
			s.transform.localScale = Vector3.one;

			StartCoroutine(s.GetComponent<Summon_AnimScript>().startEff());

			v.transform.parent = transform.GetChild(0).transform;

			v.state = Enemy.EnemyState.IDLE;

			i++;
		}
	}

	// �ʵ忡 �����Ͱ� �����ִ��� Ȯ��
	void CheckEnemys()
	{
		if (!isStartBattle)
			return;

		// ���� Ŭ��� �ƴҶ�
		if(!isCleared)
		{	
			// ������ �϶� 
			if(isBossBattle)
			{
				// ������ ó�� �ϰ� return
				Boss.bossBattle = true;

				// ���� ������Ʈ�� ��Ȱ��ȭ �Ǿ��� ��
				if (Boss.Dead)
				{
					// �� Ŭ���� ó��
					isCleared = true;
					//===========================================������ �����߰�=============================
					// ������ ����� ���߰�
					SoundManager.Sound.StopBGM();

					// �׿� ������ 10.0��(���鼭 ����) ���� �⺻ ��� ���
					IEnumerator startBgm() 
					{
						yield return new WaitForSeconds(10.0f);
						SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Standard", SoundManager.SoundType.Bgm);
						// ��� ������ ���� ������ 1��� ��
						SoundManager.Sound.SetBgmPerVol(1f);

					};
					StartCoroutine(startBgm());

					OpenDoor();
				}
				return;
			}

			// ���� 2����ȯ�� ���� ���� ���¿���
			if (!isSpawnEnemy)
			{ // ���� óġ���� ���� ������ ���� ��ȯ�� ������ ������ �۰ų� ������(�̸� ��ȯ�� ���Ͱ� ���� óġ �Ǿ��� ��)
				if (CurEnemyCount <= SpawnEnemyCount)
				{
					// ���� 2�� ��ȯ
					StartCoroutine(SummonEnemy());
					// 2�� ��ȯ bool���� true
					isSpawnEnemy = true;
				}
			}
			// 2�� ��ȯ�� �� ������ ��
			else if(isSpawnEnemy)
			{
				// �����ִ� ������ ���� 0���� �۰ų� ������(�ʵ忡 �����ϴ� ���Ͱ� ���� óġ�Ǿ��� ��)
				if (CurEnemyCount <= 0)
				{
					OpenDoor();

					ksjChest chest = gameObject.GetComponentInChildren<ksjChest>();

					if (chest != null)
					{
						if (chest.chestAnim == ksjChest.ChestAnim.Hidden)
							chest.chestAnim = ksjChest.ChestAnim.Reveal;
						else if (chest.chestAnim == ksjChest.ChestAnim.Closed)
						{
							chest.nowAnim = false;
							chest.chestAnim = ksjChest.ChestAnim.Open;
						}
							
					}

					// �ʵ� Ŭ����
					isCleared = true;
				}
			}
		}
	}

	// �� ����(�� ������Ʈ�� �ݶ��̴� ��Ȱ��ȭ)
	public void StartDoor()
	{
		foreach (var n in Doors)
		{
			n.GetComponent<BoxCollider>().enabled = false;
		}
		foreach (var n in DoorSymbols)
		{
			n.GetComponent<SpriteRenderer>().color = Color.white;
			n.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// �� ����(�� ������Ʈ�� �ݶ��̴� ��Ȱ��ȭ)
	public void OpenDoor()
	{
		foreach(var n in Doors)
		{
			n.GetComponent<BoxCollider>().enabled = false;
		}
		foreach(var n in DoorSymbols)
		{
			n.GetComponent<SpriteRenderer>().color = Color.white;
			n.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// �� ����(�� ������Ʈ�� �ݶ��̴� Ȱ��ȭ)
	public void CloseDoor()
	{
		if (CurEnemyCount > 0)
		{
			if (alreadyEnemys.Length > 0)
			{
				for (int i = 0; i < alreadyEnemys.Length; i++)
				{
					alreadyEnemys[i].state = Enemy.EnemyState.IDLE;
				}
			}

			foreach (var n in Doors)
			{
				n.GetComponent<BoxCollider>().enabled = true;
			}
			foreach (var n in DoorSymbols)
			{
				n.GetComponent<SpriteRenderer>().color = Color.red;
				n.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
		else if (CurEnemyCount <= 0)
		{
			if (isBossBattle)
			{
				StartCoroutine(CameraShake.instance.ShakeCoroutine());

				//=================================������ �����߰�=============================================
				// ���� ���� �������̰� ������ ����ִ� ����(������ ���ӿ�����Ʈ�� Ȱ��ȭ)���� ���ݴ� �Լ� ȣ�� ��
				if (isBossBattle && Boss.gameObject.activeSelf)
				{
					// ���� ��� ���������� �ٲ�
					SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Boss", SoundManager.SoundType.Bgm);
					// ��� ������ ���� ������ 0.7��� ����
					SoundManager.Sound.SetBgmPerVol(0.7f);
				}

				foreach (var n in Doors)
				{
					n.GetComponent<BoxCollider>().enabled = true;
				}
				foreach (var n in DoorSymbols)
				{
					n.GetComponent<SpriteRenderer>().color = Color.red;
					n.transform.GetChild(0).gameObject.SetActive(true);
				}
			}
		}
	}

	// ��ȯ�Ǿ��ִ� ���� �� --
	public void MinusCurEnemyCount()
	{
		CurEnemyCount--;
	}
}
