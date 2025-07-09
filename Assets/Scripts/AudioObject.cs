using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
	public AudioSource source;

	private Dictionary<string, List<AudioObject>> audioDic = new Dictionary<string, List<AudioObject>>();

	public bool IsPlaying
	{
		get
		{
			return source.isPlaying;
		}
	}

	public AudioObject Play()
	{
		if ((!source.isPlaying && source.loop) || !source.loop)
		{
			source.Play();
		}
		return this;
	}

	public AudioObject Play(AudioClip clip, bool loop, float volume, float pitch, bool OnAwake)
	{
		return Play(clip, loop, volume, pitch, OnAwake, null);
	}

	public AudioObject Play(AudioClip clip, bool loop, float volume, float pitch, bool OnAwake, Dictionary<string, List<AudioObject>> audioDic)
	{
		this.audioDic = audioDic;
		Set(clip, loop, volume, pitch, OnAwake);
		source.priority = 0;
		Play();
		return this;
	}

	public AudioObject Set(AudioClip clip, bool loop, float volume, float pitch, bool OnAwake)
	{
		source.clip = clip;
		source.priority = Mathf.Clamp(0, 0, 255);
		source.loop = loop;
		source.volume = volume;
		source.pitch = pitch;
		source.playOnAwake = OnAwake;
		if (audioDic == null)
		{
			if (!loop)
			{
				StartCoroutine("playTime", source.clip.length);
			}
		}
		else if (!loop)
		{
			StartCoroutine("playTimeDic", source.clip.length);
		}
		return this;
	}

	private IEnumerator playTime(float time)
	{
		yield return new WaitForSeconds(time);
		source.Stop();
	}

	private IEnumerator playTimeDic(float time)
	{
		yield return new WaitForSeconds(time);
		if (audioDic.ContainsKey(base.name) && audioDic[base.name].Count > 0)
		{
			source.Stop();
			audioDic[base.name].RemoveAt(0);
		}
	}
}
