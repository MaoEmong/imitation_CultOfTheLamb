using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 사운드 매니저
/*
 사용법
1. Asset/Resources/Sounds/ 경로 만들기
2. Sounds폴더 안에 사용할 음원 넣기
3. SoundManager.Sound.Play(string "{Sound폴더 안에 또 다른 폴더를 만들었다면 폴더이름/}음원이름", SoundType 음원타입);
4. 음원 타입 = Bgm - 무한반복 / Effect - 한번만 재생됨
5. 씬을 넘어가거나 필요시 SoundManager.Sound.Clear(); 코드 실행 시 사운드 전체 삭제
 */

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public static SoundManager Sound { get { return instance; } }

    public enum SoundType
    {
        Bgm = 0,
        Effect,
        Enemy,


        MaxCount
    }
	AudioSource[] _audioSource = new AudioSource[(int)SoundType.MaxCount];
	Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip> ();

    float EffectVolum = 0.7f;
    float BgmVolum = 0.7f;

	private void Awake()
	{
		if (instance == null)
		{
			GameObject go = GameObject.Find("SoundManager");
			if (go == null)
			{
				go = new GameObject { name = "SoundManager" };
				go.AddComponent<SoundManager>();
			}

			DontDestroyOnLoad(go);
			instance = go.GetComponent<SoundManager>();
            
            instance.Init();

		}
	}

	void Init()
	{
		GameObject root = GameObject.Find("Sound");
		if(root == null)
		{
            root = new GameObject { name = "Sound" };
			Object.DontDestroyOnLoad(root);

			string[] soundNames = System.Enum.GetNames(typeof(SoundType));
			for(int i = 0; i < soundNames.Length - 1; i++)
			{
				GameObject sound = new GameObject { name = soundNames[i] };
				_audioSource[i] = sound.AddComponent<AudioSource>();
				sound.transform.parent = root.transform;
			}
		}

		_audioSource[(int)SoundType.Bgm].loop = true;

        foreach (var n in _audioSource)
            n.volume = EffectVolum;
	}

	public void Clear()
	{
		foreach(var audio in _audioSource)
		{
			audio.clip = null;
			audio.Stop();
		}
		_audioClips.Clear();
	}

    // Resources폴더의 경로안에 있는 파일 재생
    public void Play(string path, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        // 선택한 음원이 Bgm이라면 
        if (type == SoundType.Bgm)
        {
            // 현재 재생중인 음원 멈추고
            AudioSource audioSource = _audioSource[(int)SoundType.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            // 새로 재생
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        // 선택한 음원이 Effect라면
        else if (type == SoundType.Effect)
		{
			float value = Random.Range(pitch * 0.9f, pitch);

			// 바로 재생(1회)
			AudioSource audioSource = _audioSource[(int)SoundType.Effect];
            audioSource.pitch = value;
            audioSource.PlayOneShot(audioClip);
        }
        else if(type == SoundType.Enemy)
        {
            float value = Random.Range(pitch * 0.9f, pitch);

			AudioSource audioSource = _audioSource[(int)SoundType.Enemy];
			audioSource.pitch = value;

            // ===========================================에너미 사운드 너무 크다 싶으시면 해당 수치 조금 내리면 될겁니다
            audioSource.volume = EffectVolum * 0.8f;
			audioSource.PlayOneShot(audioClip);
		}
    }

    public void StopBGM()
    {
        if(_audioSource[(int)SoundType.Bgm].isPlaying)
			_audioSource[(int)SoundType.Bgm].Stop();
	}

    // 경로로 입력한 위치에 있는 오디오 클립 찾아
    // type으로 설정한 오디오소스에 넣어둠
    AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == SoundType.Bgm)
        {
            audioClip = Resources.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
			}
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

    public void SetBGMVol(float vol)
    {
        BgmVolum = vol;
		_audioSource[(int)SoundType.Bgm].volume = BgmVolum;
    }
    public void SetBgmPerVol(float per)
    {
        float val = BgmVolum * per;
        _audioSource[(int)SoundType.Bgm].volume = val;

	}
	public float GetBGMVol()
    {
        return _audioSource[(int)SoundType.Bgm].volume;
    }
    public void SetEffectVol(float vol) 
    {
        EffectVolum = vol;
		_audioSource[(int)SoundType.Effect].volume = EffectVolum;
		_audioSource[(int)SoundType.Enemy].volume = EffectVolum;
	}
    public float GetEffectVol()
    {
        return EffectVolum;
    }

}
