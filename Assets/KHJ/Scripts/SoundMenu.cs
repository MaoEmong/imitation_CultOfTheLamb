using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���� �޴�
// ���� ���� �� ���� ����
// �ʿ�� �߰�
public class SoundMenu : MonoBehaviour
{
    // ���� ������ �����̴�
    public Slider BgmSlider;
    public Slider EffectSlider;

    // ������ ���� ǥ�� �ؽ�Ʈ
    public Text BgmText;
    public Text EffectText;

    // �̴ϼȶ���¡
    public void Start()
    {
        // ���� �����̴��� ���� ������ ����
        BgmSlider.value = SoundManager.Sound.GetBGMVol();
        EffectSlider.value = SoundManager.Sound.GetEffectVol();

        // ���� �����̴��� ��ġ��ȭ ��Ͽ� ����Ŵ��� ���
		BgmSlider.onValueChanged.AddListener(SoundManager.Sound.SetBGMVol);
		EffectSlider.onValueChanged.AddListener(SoundManager.Sound.SetEffectVol);
	}


    void Update()
    {
        // �����̴����� ��ȭ�� ���� �ؽ�Ʈ ����
        BgmText.text = $"{Mathf.Ceil(BgmSlider.value*100)}%";
        EffectText.text = $"{Mathf.Ceil(EffectSlider.value*100)}%";

    }
}
