using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
	// 포탈의 타입
	public enum PortalType
	{
		Next,
		Back
	}
	// 위치 설정
	public PortalType type;
	// 맵 컨트롤러
	MapController MC;
	// 순간이동할 trnasform
	public Transform TargetPos = null;

	// 포탈이 존재하는 필드 넘버링
	public int FieldNumber;

	// 포탈의 부모(Field)
	public Field Parant;

	private void Awake()
	{
		// Field 정보 가져오기
		Parant = transform.GetComponentInParent<Field>();
		FieldNumber = Parant.StageNumber;

		Debug.Log($"{FieldNumber}Portal Start");
	}
	// 최초 생성 시
	public void Init(MapController map)
	{
		MC = map;
		Field[] myfield = MC.TotalField[MC.StageNumber];
		// 포탈의 타입에 따른 타겟 설정
		switch(type)
		{
			// 현재 포탈의 타입이 Next일 때
			case PortalType.Next:
				// 현재 포탈이 존재하는 필드의 넘버링이 전체 필드숫자와 같을때
				if (FieldNumber == myfield.Length - 1)
				{
					// 이동할 포지션은 null
					TargetPos = null;
				}
				// 현재 포탈이 존재하는 필드가 마지막 필드가 아니라면
				else
				{
					// 이동할 포지션은 다음 필드의 포탈중 0번째(Back)의 자식객체
					TargetPos = myfield[FieldNumber + 1].Portals[0].transform.GetChild(0).transform;
				}
				break;

			// 현재 포탈의 타입이 Back일때
			case PortalType.Back:
				// 현재 포탈이 존재하는 필드가 제일 첫번째 일 때
				if(FieldNumber <= 0)
				{
					// 이동할 위치는 null
					TargetPos = null;
				}
				// 제일 첫번쨰가 아니라면
				else
				{
					// 이동할 포지션은 이전 필드의 포탈중 1번째(Next)의 자식객체
					TargetPos = myfield[FieldNumber - 1].Portals[1].transform.GetChild(0).transform;
				}
				break;
		}

		Debug.Log($"{FieldNumber} Portal Init");
	}

	// 트리거 설정
	private void OnTriggerEnter(Collider other)
	{
		// 충돌한 물체의 태그가 플레이어일 경우
		if(other.CompareTag("Player"))
		{
			// 화면 가리는 효과
			MC.FadeInOutImage();
			// 플레이어 포지션값 변경
			StartCoroutine(GoToTarget(other));
		}
	}

	// 플레이어 이동 코루틴
	IEnumerator GoToTarget(Collider other)
	{
		// 화면이 완전히 가려지는 0.3초
		yield return new WaitForSeconds(0.3f);

		// 타겟 포지션이 비어있다면(마지막 필드 정리하고 넘어갈 경우)
		if (TargetPos == null)
		{
			// 현재 스테이지 클리어로 판정하고 넘버링을 다음 스테이지로 넘김
			// GameManager.CurStage++;

			// 이동하는 포탈의 타입이 Next일때
			if (type == PortalType.Next)
			{
				GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().CurStage++;
								
				// 스테이지 넘버링이 3보다 크다면(마지막스테이지를 클리어했다면 / 마지막 스테이지 넘버링이 3임)
				if(GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().CurStage > 3)
				{
					var pm = ksjPlayerManager.Instance;
					pm.CurStage = 0;

					pm.ChangePlayerWeapon();

					// 대기방으로 이동
					SceneManager.LoadScene("LoadWaitRoom");
				}
				// 아니면 셀렉트씬으로 이동
				else
				// 스테이지 선택 씬으로 전환
					SceneManager.LoadScene("Select");
			}
		}
		// 타겟 포지션이 설정되 있다면
		else
		{
			// 플레이어의 위치값 변경
			other.gameObject.transform.position = TargetPos.position;
			// 이동한 필드의 넘버링 디버그로 출력
			Debug.Log($"{TargetPos.parent.GetComponent<Portal>().FieldNumber}");
		}
	}

}
