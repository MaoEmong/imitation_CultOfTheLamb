using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 스테이지 진입 후 생성될 필드 스크립트
// 해당 스크립트에서 OpenDoor만 담당 / CloseDoor는 GroundOnlyTriggerCheck 스크립트에서 사용중
public class Field : MonoBehaviour
{
	// 스테이지 넘버링
	public int StageNumber;

	//====================== 필드 수정 사항
	// 1. 미리 스폰되는 몬스터가 존재함
	// 2. 해당 몬스터들을 전부 잡게되면 이후 2차 소환 시작
	// 3. 2차 소환 몬스터까지 전부 처치 시 맵 클리어 판정


	// 미리 소환해 둘 몬스터
	public Enemy[] alreadyEnemys;
	// 2차로 소환할 몬스터
	public Enemy[] SpawnEnemys;
	// 소환할 보스
	public Boss_Logic Boss;
	// 랜덤 소환 위치
	public Transform[] SpawnPos;

	// 스폰 이펙트
	public GameObject SpawnObj;

	// 미리 소환해 둔 몬스터의 수
	public int alreadyEnemyCount;
	// 2차 스폰하는 적의 수
	public int SpawnEnemyCount;
	// 총 적의 수
	public int totalEnemyCount;
	// 아직 처치되지 않은 몬스터의 수
	public int CurEnemyCount;
	// 2차 소환이 되었는지 확인하기 위한 bool
	public bool isSpawnEnemy = false;
	// 보스전 유무
	public bool isBossBattle = false;

	// 필드 길을 막고있는 문오브젝트
	public GameObject[] Doors;
	// 바닥 심볼
	public GameObject[] DoorSymbols;
	// 필드에 존재하는 포탈
	public Portal[] Portals;
	// 현재 필드를 클리어 했는지 확인(몬스터 정리)
	public bool isCleared = false;
	// 필드에 들어갔을 시 몬스터 소환을 하기 위한 bool값
	public bool isStartBattle = false;

	// 필드 0 무기
	public Item_WeaponDrop weapon;

	// 맵컨트롤러
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

	// 필드 이니셜라이즈
	public void Init(MapController map)	
	{
		// 맵 컨트롤러 정보
		MC = map;

		// 미리 소환되어 있는 몬스터의 수
		alreadyEnemyCount = alreadyEnemys.Length;
		// 2차로 소환할 몬스터의 수
		SpawnEnemyCount = SpawnEnemys.Length;
		// 몬스터의 총 유닛 수
		totalEnemyCount = alreadyEnemyCount + SpawnEnemyCount;
		// 아직 처치되지 않은 몬스터의 수 == 전체 몬스터의 수
		CurEnemyCount = totalEnemyCount;

		foreach (var n in alreadyEnemys)
			n.Init(this);

		// 맵의 클리어 판정은 False
		//isCleared = false;

		// 최초 시작은 문 열려있음
		StartDoor();
	}

	// 몬스터 소환 코루틴
	// GroundOnlyTriggerCheck스크립트에서 몬스터 소환 사용 중
	public IEnumerator SummonEnemy()
	{
		int i = 0;

		// Enemys 반복
		foreach (var enemy in SpawnEnemys) 
		{
			// 0.3초 단위로 한마리씩 소환
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

	// 필드에 적몬스터가 남아있는지 확인
	void CheckEnemys()
	{
		if (!isStartBattle)
			return;

		// 아직 클리어가 아닐때
		if(!isCleared)
		{	
			// 보스전 일때 
			if(isBossBattle)
			{
				// 보스전 처리 하고 return
				Boss.bossBattle = true;

				// 보스 오브젝트가 비활성화 되었을 때
				if (Boss.Dead)
				{
					// 맵 클리어 처리
					isCleared = true;
					//===========================================김해준 내용추가=============================
					// 보스전 브금을 멈추고
					SoundManager.Sound.StopBGM();

					// 그와 별개로 10.0초(보면서 변경) 이후 기본 브금 재생
					IEnumerator startBgm() 
					{
						yield return new WaitForSeconds(10.0f);
						SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Standard", SoundManager.SoundType.Bgm);
						// 브금 볼륨을 현재 볼륨의 1배로 업
						SoundManager.Sound.SetBgmPerVol(1f);

					};
					StartCoroutine(startBgm());

					OpenDoor();
				}
				return;
			}

			// 아직 2차소환이 되지 않은 상태에서
			if (!isSpawnEnemy)
			{ // 아직 처치되지 않은 몬스터의 수가 소환될 몬스터의 수보다 작거나 같을때(미리 소환된 몬스터가 전부 처치 되었을 때)
				if (CurEnemyCount <= SpawnEnemyCount)
				{
					// 몬스터 2차 소환
					StartCoroutine(SummonEnemy());
					// 2차 소환 bool값은 true
					isSpawnEnemy = true;
				}
			}
			// 2차 소환이 된 상태일 떄
			else if(isSpawnEnemy)
			{
				// 남아있는 몬스터의 수가 0보다 작거나 같을때(필드에 존재하는 몬스터가 전부 처치되었을 때)
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

					// 필드 클리어
					isCleared = true;
				}
			}
		}
	}

	// 문 열림(문 오브젝트의 콜라이더 비활성화)
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

	// 문 열림(문 오브젝트의 콜라이더 비활성화)
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

	// 문 닫힘(문 오브젝트의 콜라이더 활성화)
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

				//=================================김해준 내용추가=============================================
				// 현재 방이 보스방이고 보스가 살아있는 상태(보스의 게임오브젝트가 활성화)에서 문닫는 함수 호출 시
				if (isBossBattle && Boss.gameObject.activeSelf)
				{
					// 현재 브금 보스전으로 바꿈
					SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Boss", SoundManager.SoundType.Bgm);
					// 브금 볼륨을 현재 볼륨의 0.7배로 감소
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

	// 소환되어있는 적의 수 --
	public void MinusCurEnemyCount()
	{
		CurEnemyCount--;
	}
}
