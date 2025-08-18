using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_SpikeScript : MonoBehaviour
{
    // 함정 발동 콜라이더
    public Collider OnRange;

    // 업데이트 트리거
    bool on;
    bool spikeUp;

    // 검사 시간
    public float TwinkleTime;
	public float curTime;
	public float upTime;
	public float activeTime;

    bool Clear;

    // 가시 히트 콜라이더
    public Collider AttackRange;

    // 발동 오브젝트&비활성화 오브젝트
    public GameObject ReadyTrap;
    public GameObject OffTrap;

    // 가시 오브젝트
    public GameObject Spike;

    // 필드
	public Field field;

	private void Start()
	{
        field = GetComponentInParent<Field>();

        activeTime = 0.014f;
		curTime = 0;
		upTime = 0.014f;
		TwinkleTime = 0.25f;
	}

	void Update()
    {
        if (Clear)
            return;

		if (field.isCleared && field.isStartBattle)
			clearTrap();

		// 가시가 작동
		if (on)
        {
            // 발동 준비시간 동안 발동 순간을 암시하는 반짝이는 시간 적용
            if (curTime > TwinkleTime)
            {
                // 가시가 튀어나오기 전이라면 반짝이는 스프라이트 활성화/비활성화
                if (!ReadyTrap.activeSelf)
					ReadyTrap.SetActive(true);
				else
				    ReadyTrap.SetActive(false);

                // 반짝이는 시간 점점 감소
				TwinkleTime -= 1f * Time.deltaTime;
                curTime = 0;
			}

            // 반짝이는 시간 증가
            curTime += Time.deltaTime;

            // 가시 상승 준비시간
			if (upTime > 2)
            {
                // 반짝이 스프라이트 비활성화
				ReadyTrap.SetActive(false);
                // 스파이크 상승
				spikeUp = true;
                upTime *= 0;
                on = false;
                // 반짝임 시간 초기화
                TwinkleTime = 0.25f;
                curTime = 0;
			}    

            // 발동 준비시간 증가
			upTime += Time.deltaTime;
        }

        // 가시 상승
        if (spikeUp)
        {
            // 가시 발동 범위 비활성화
			OnRange.enabled = false;
            // 가시 히트 레인지 발동
			AttackRange.enabled = true;

            // 발동시간 동안
            if (activeTime < 1.5f)
            {

                // 가시 상승
				Spike.transform.localPosition = Vector3.Lerp(Spike.transform.localPosition,
														 new Vector3(0.1f, 0.8f, 0), 20 * Time.deltaTime);
			}
            // 발동시간 후 잠시간 가시 유지 후 가시 하강
			else if (activeTime > 1.5f)
			{
				Spike.transform.localPosition = Vector3.Lerp(Spike.transform.localPosition,
														 new Vector3(0.1f, -1f, 0), 20 * Time.deltaTime);
				// 가시 히트 레인지 비활성화
				AttackRange.enabled = false;
			}

            // 가시 하강 종료와 동시에 가시 함정 재장전
			if (activeTime > 2.5f)
			{
                // 발동 레인지 재활성화
				OnRange.enabled = true;
                spikeUp = false;

				activeTime *= 0;
			}

			activeTime += Time.deltaTime;
		}
    }

    void clearTrap()
    {
        Clear = true;
        OffTrap.SetActive(true);
        Spike.transform.localPosition = new Vector3(0.1f, -1f, 0);
        spikeUp = false;
		OnRange.enabled = false;
		AttackRange.enabled = false;
        on = false;
	}

	private void OnTriggerEnter(Collider other)
	{
        // 플레이어가 발동 범위에 들어오면 함정발동
		if (other.gameObject.name.Contains("Player") && (activeTime < 2.0f && !on && !spikeUp))
        {
            on = true;
        }

        // 플레이어가 히트 판정에 들어오면 히트
		if (other.gameObject.name.Contains("Player") && spikeUp)
		{
			// 가시 히트 레인지 비활성화
			AttackRange.enabled = false;
		}
	}
}
