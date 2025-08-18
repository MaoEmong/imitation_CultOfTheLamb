using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_SpikeScript : MonoBehaviour
{
    // ���� �ߵ� �ݶ��̴�
    public Collider OnRange;

    // ������Ʈ Ʈ����
    bool on;
    bool spikeUp;

    // �˻� �ð�
    public float TwinkleTime;
	public float curTime;
	public float upTime;
	public float activeTime;

    bool Clear;

    // ���� ��Ʈ �ݶ��̴�
    public Collider AttackRange;

    // �ߵ� ������Ʈ&��Ȱ��ȭ ������Ʈ
    public GameObject ReadyTrap;
    public GameObject OffTrap;

    // ���� ������Ʈ
    public GameObject Spike;

    // �ʵ�
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

		// ���ð� �۵�
		if (on)
        {
            // �ߵ� �غ�ð� ���� �ߵ� ������ �Ͻ��ϴ� ��¦�̴� �ð� ����
            if (curTime > TwinkleTime)
            {
                // ���ð� Ƣ����� ���̶�� ��¦�̴� ��������Ʈ Ȱ��ȭ/��Ȱ��ȭ
                if (!ReadyTrap.activeSelf)
					ReadyTrap.SetActive(true);
				else
				    ReadyTrap.SetActive(false);

                // ��¦�̴� �ð� ���� ����
				TwinkleTime -= 1f * Time.deltaTime;
                curTime = 0;
			}

            // ��¦�̴� �ð� ����
            curTime += Time.deltaTime;

            // ���� ��� �غ�ð�
			if (upTime > 2)
            {
                // ��¦�� ��������Ʈ ��Ȱ��ȭ
				ReadyTrap.SetActive(false);
                // ������ũ ���
				spikeUp = true;
                upTime *= 0;
                on = false;
                // ��¦�� �ð� �ʱ�ȭ
                TwinkleTime = 0.25f;
                curTime = 0;
			}    

            // �ߵ� �غ�ð� ����
			upTime += Time.deltaTime;
        }

        // ���� ���
        if (spikeUp)
        {
            // ���� �ߵ� ���� ��Ȱ��ȭ
			OnRange.enabled = false;
            // ���� ��Ʈ ������ �ߵ�
			AttackRange.enabled = true;

            // �ߵ��ð� ����
            if (activeTime < 1.5f)
            {

                // ���� ���
				Spike.transform.localPosition = Vector3.Lerp(Spike.transform.localPosition,
														 new Vector3(0.1f, 0.8f, 0), 20 * Time.deltaTime);
			}
            // �ߵ��ð� �� ��ð� ���� ���� �� ���� �ϰ�
			else if (activeTime > 1.5f)
			{
				Spike.transform.localPosition = Vector3.Lerp(Spike.transform.localPosition,
														 new Vector3(0.1f, -1f, 0), 20 * Time.deltaTime);
				// ���� ��Ʈ ������ ��Ȱ��ȭ
				AttackRange.enabled = false;
			}

            // ���� �ϰ� ����� ���ÿ� ���� ���� ������
			if (activeTime > 2.5f)
			{
                // �ߵ� ������ ��Ȱ��ȭ
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
        // �÷��̾ �ߵ� ������ ������ �����ߵ�
		if (other.gameObject.name.Contains("Player") && (activeTime < 2.0f && !on && !spikeUp))
        {
            on = true;
        }

        // �÷��̾ ��Ʈ ������ ������ ��Ʈ
		if (other.gameObject.name.Contains("Player") && spikeUp)
		{
			// ���� ��Ʈ ������ ��Ȱ��ȭ
			AttackRange.enabled = false;
		}
	}
}
