using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� �Ŵ���
/*
 ����
1. Asset/Resources/Sounds/ ��� �����
2. Sounds���� �ȿ� ����� ���� �ֱ�
3. SoundManager.Sound.Play(string "{Sound���� �ȿ� �� �ٸ� ������ ������ٸ� �����̸�/}�����̸�", SoundType ����Ÿ��);
4. ���� Ÿ�� = Bgm - ���ѹݺ� / Effect - �ѹ��� �����
5. ���� �Ѿ�ų� �ʿ�� SoundManager.Sound.Clear(); �ڵ� ���� �� ���� ��ü ����
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

    // Resources������ ��ξȿ� �ִ� ���� ���
    public void Play(string path, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        // ������ ������ Bgm�̶�� 
        if (type == SoundType.Bgm)
        {
            // ���� ������� ���� ���߰�
            AudioSource audioSource = _audioSource[(int)SoundType.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            // ���� ���
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        // ������ ������ Effect���
        else if (type == SoundType.Effect)
		{
			float value = Random.Range(pitch * 0.9f, pitch);

			// �ٷ� ���(1ȸ)
			AudioSource audioSource = _audioSource[(int)SoundType.Effect];
            audioSource.pitch = value;
            audioSource.PlayOneShot(audioClip);
        }
        else if(type == SoundType.Enemy)
        {
            float value = Random.Range(pitch * 0.9f, pitch);

			AudioSource audioSource = _audioSource[(int)SoundType.Enemy];
			audioSource.pitch = value;

            // ===========================================���ʹ� ���� �ʹ� ũ�� �����ø� �ش� ��ġ ���� ������ �ɰ̴ϴ�
            audioSource.volume = EffectVolum * 0.8f;
			audioSource.PlayOneShot(audioClip);
		}
    }

    public void StopBGM()
    {
        if(_audioSource[(int)SoundType.Bgm].isPlaying)
			_audioSource[(int)SoundType.Bgm].Stop();
	}

    // ��η� �Է��� ��ġ�� �ִ� ����� Ŭ�� ã��
    // type���� ������ ������ҽ��� �־��
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
