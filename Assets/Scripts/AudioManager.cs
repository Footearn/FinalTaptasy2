using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	public enum EffectType
	{
		Skill,
		Touch,
		Resource,
		Speak,
		Colleague,
		TranscendSkill,
		Princess
	}

	public AudioMixer mainMixer;

	public AudioMixerGroup sfxMixerGroup;

	public AudioMixerGroup skillMixerGroup;

	public AudioSource bgmAudioSource;

	public AudioSource touchAudioSource;

	public AudioSource resourceAudioSource;

	public AudioSource speakAudioSource;

	public AudioSource princessAudioSource;

	public float backGroundVolume;

	public float effectVolume;

	public Dictionary<string, AudioClip> bgmAudioClipDictionary;

	public Dictionary<string, AudioClip> effectAudioClipDictionary;

	private Dictionary<string, List<AudioObject>> m_loopEffectAudioClipDictionary = new Dictionary<string, List<AudioObject>>();

	private Dictionary<string, List<AudioObject>> m_skillEffectAudioClipDictionary = new Dictionary<string, List<AudioObject>>();

	public AudioObject[] audioObjectArray;

	private long m_audioObjectIndex;

	private void Awake()
	{
		Singleton<AudioManager>.instance.refreshAudioVolume();
	}

	private AudioObject getAudioObject()
	{
		AudioObject result = audioObjectArray[m_audioObjectIndex % audioObjectArray.Length];
		m_audioObjectIndex++;
		return result;
	}

	public void addBackgroundAudioClip(string name, AudioClip audioClip)
	{
		if (bgmAudioClipDictionary == null)
		{
			bgmAudioClipDictionary = new Dictionary<string, AudioClip>();
		}
		bgmAudioClipDictionary.Add(name, audioClip);
	}

	public void addEffectAudioClip(string name, AudioClip audioClip)
	{
		if (effectAudioClipDictionary == null)
		{
			effectAudioClipDictionary = new Dictionary<string, AudioClip>();
		}
		effectAudioClipDictionary.Add(name, audioClip);
	}

	private void OnApplicationPause(bool pause)
	{
	}

	public void refreshAudioVolume()
	{
		if (NSPlayerPrefs.GetFloat("SFX", 1f) > 0f)
		{
			effectVolume = NSPlayerPrefs.GetFloat("SFX", 1f);
		}
		else
		{
			effectVolume = 0f;
		}
		if (NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
		{
			backGroundVolume = NSPlayerPrefs.GetFloat("BGM", 1f);
		}
		else
		{
			backGroundVolume = 0f;
		}
		bgmAudioSource.volume = backGroundVolume;
		if (bgmAudioSource.volume > 0f && !bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Play();
		}
	}

	public void playBackgroundSound(string name)
	{
		if (!bgmAudioSource.isPlaying || bgmAudioSource.clip.name != name)
		{
			bgmAudioSource.Stop();
			bgmAudioSource.clip = bgmAudioClipDictionary[name];
			bgmAudioSource.pitch = 1f;
			bgmAudioSource.loop = true;
			bgmAudioSource.volume = backGroundVolume;
			bgmAudioSource.Play();
		}
	}

	public void stopBackgroundSound()
	{
		if (bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Stop();
		}
	}

	public void playForceEffectSound(string name, float volume = -1f)
	{
		if (!(NSPlayerPrefs.GetFloat("SFX", 1f) <= 0f))
		{
			if (volume == -1f)
			{
				volume = effectVolume;
			}
			if (effectAudioClipDictionary.ContainsKey(name))
			{
				AudioSource.PlayClipAtPoint(effectAudioClipDictionary[name], Vector3.zero, volume);
			}
			else
			{
				DebugManager.LogError(name + " SFX is not registered");
			}
		}
	}

	public void playEffectSound(string name, bool loop = false)
	{
		if (NSPlayerPrefs.GetFloat("SFX", 1f) <= 0f)
		{
			return;
		}
		if (loop)
		{
			AudioObject audioObject = getAudioObject();
			if (!m_loopEffectAudioClipDictionary.ContainsKey(name))
			{
				m_loopEffectAudioClipDictionary.Add(name, new List<AudioObject>());
			}
			m_loopEffectAudioClipDictionary[name].Add(audioObject);
			audioObject.Play(effectAudioClipDictionary[name], true, effectVolume, 1f, true);
		}
		else
		{
			getAudioObject().Play(effectAudioClipDictionary[name], false, effectVolume, 1f, true).source.outputAudioMixerGroup = sfxMixerGroup;
		}
	}

	public void playEffectSound(string name, EffectType type, bool loop = false)
	{
		if (NSPlayerPrefs.GetFloat("SFX", 1f) <= 0f)
		{
			return;
		}
		switch (type)
		{
		case EffectType.Skill:
		{
			AudioObject audioObject = getAudioObject();
			if (!m_skillEffectAudioClipDictionary.ContainsKey(name))
			{
				m_skillEffectAudioClipDictionary.Add(name, new List<AudioObject>());
			}
			m_skillEffectAudioClipDictionary[name].Add(audioObject);
			audioObject.Play(effectAudioClipDictionary[name], loop, effectVolume, 1f, loop);
			audioObject.source.outputAudioMixerGroup = skillMixerGroup;
			break;
		}
		case EffectType.Touch:
			resourceAudioSource.loop = false;
			touchAudioSource.clip = effectAudioClipDictionary[name];
			touchAudioSource.volume = effectVolume;
			touchAudioSource.Play();
			break;
		case EffectType.Resource:
			resourceAudioSource.loop = false;
			resourceAudioSource.clip = effectAudioClipDictionary[name];
			resourceAudioSource.volume = effectVolume;
			resourceAudioSource.Play();
			break;
		case EffectType.Speak:
			speakAudioSource.loop = false;
			speakAudioSource.clip = effectAudioClipDictionary[name];
			speakAudioSource.volume = effectVolume;
			speakAudioSource.Play();
			break;
		case EffectType.Colleague:
			speakAudioSource.loop = false;
			speakAudioSource.clip = effectAudioClipDictionary[name];
			speakAudioSource.volume = effectVolume * 0.1f;
			speakAudioSource.Play();
			break;
		case EffectType.TranscendSkill:
			speakAudioSource.loop = false;
			speakAudioSource.clip = effectAudioClipDictionary[name];
			speakAudioSource.volume = effectVolume * 0.5f;
			speakAudioSource.Play();
			break;
		case EffectType.Princess:
			princessAudioSource.loop = false;
			princessAudioSource.clip = effectAudioClipDictionary[name];
			princessAudioSource.Play();
			break;
		default:
			getAudioObject().Play(effectAudioClipDictionary[name], false, effectVolume, 1f, true).source.outputAudioMixerGroup = sfxMixerGroup;
			break;
		}
	}

	public void stopLoopEffectSound(string name)
	{
		if (m_loopEffectAudioClipDictionary.ContainsKey(name) && m_loopEffectAudioClipDictionary[name].Count > 0)
		{
			m_loopEffectAudioClipDictionary[name][0].source.Stop();
			m_loopEffectAudioClipDictionary[name].RemoveAt(0);
		}
	}
}
