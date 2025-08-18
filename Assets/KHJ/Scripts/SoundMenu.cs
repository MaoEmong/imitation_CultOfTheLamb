using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 세팅 메뉴
// 현재 사운드 외 구현 없음
// 필요시 추가
public class SoundMenu : MonoBehaviour
{
    // 사운드 설정용 슬라이더
    public Slider BgmSlider;
    public Slider EffectSlider;

    // 설정한 볼륨 표시 텍스트
    public Text BgmText;
    public Text EffectText;

    // 이니셜라이징
    public void Start()
    {
        // 볼륨 슬라이더에 현재 볼륨값 적용
        BgmSlider.value = SoundManager.Sound.GetBGMVol();
        EffectSlider.value = SoundManager.Sound.GetEffectVol();

        // 볼륨 슬라이더의 수치변화 목록에 사운드매니저 등록
		BgmSlider.onValueChanged.AddListener(SoundManager.Sound.SetBGMVol);
		EffectSlider.onValueChanged.AddListener(SoundManager.Sound.SetEffectVol);
	}


    void Update()
    {
        // 슬라이더값의 변화에 맞춰 텍스트 변경
        BgmText.text = $"{Mathf.Ceil(BgmSlider.value*100)}%";
        EffectText.text = $"{Mathf.Ceil(EffectSlider.value*100)}%";

    }
}
