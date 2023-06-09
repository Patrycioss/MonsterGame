using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class Jukebox : MonoBehaviour
{
	[SerializeField] private List<MusicClip> _musicClips = new();
	[SerializeField] private float _stopTransitionDuration = 1f;
	[SerializeField] private float _playTransitionDuration = 1f;
	[SerializeField] private bool _finish;
	private int index;
	
	[Serializable]
	private class MusicClip
	{
		public AudioClip clip;
		public float volumeModifier = 1;
	}
	
	private AudioSource _audioSource;

	public static Jukebox instance { get; private set; }

	private bool _shouldBePlaying = false;
	private void Awake()
	{
		if (instance != null) Destroy(instance.gameObject);
		instance = this;
		EventBus<MusicVolumeChangeEvent>.Subscribe(ChangeMusicVolume);
		_audioSource = GetComponent<AudioSource>();
		if (_finish)
		{
			float volume = PlayerPrefs.GetFloat("music", 1f);
			_audioSource.clip = _musicClips[index].clip;
			_audioSource.volume = 0;
			_audioSource.Play();
			_audioSource.DOFade(volume * _musicClips[index].volumeModifier, _playTransitionDuration);
			StartCoroutine(WaitUntillPlayed());
		}
	}

	private IEnumerator WaitUntillPlayed()
	{
		yield return new WaitUntil(() => _audioSource.isPlaying == false);
		index++;
		if (index <= _musicClips.Count)
			yield return null;
		float volume = PlayerPrefs.GetFloat("music", 1f);
		_audioSource.loop = true;
		_audioSource.clip = _musicClips[index].clip;
		_audioSource.volume = 0;
		_audioSource.Play();
		_audioSource.DOFade(volume * _musicClips[index].volumeModifier, _playTransitionDuration);
	}

	void ChangeMusicVolume(MusicVolumeChangeEvent pEvent)
	{
		_audioSource.volume = pEvent.Volume * _musicClips[index].volumeModifier;
	}

	public void PlayRandom()
	{
		if (_audioSource.isPlaying)
		{
			Stop(PlayRandomInternal);
		}
		else PlayRandomInternal();
		
		void PlayRandomInternal()
		{
			float volume = PlayerPrefs.GetFloat("music", 1f);
			int index = Random.Range(0, _musicClips.Count);
			_audioSource.clip = _musicClips[index].clip;
			_audioSource.volume = 0;
			_audioSource.Play();
			_audioSource.DOFade(volume * _musicClips[index].volumeModifier, _playTransitionDuration);
			_shouldBePlaying = true;
		}
	}

	public void Update()
	{
		if (_shouldBePlaying && !_audioSource.isPlaying && !_finish) PlayRandom();
	}

	public void Pause()
	{
		_audioSource.Pause();
	}

	private void OnDisable()
	{
		EventBus<MusicVolumeChangeEvent>.UnSubscribe(ChangeMusicVolume);
	}

	public void Stop()
	{
		_shouldBePlaying = false;
		_audioSource.DOFade(0,_stopTransitionDuration).OnComplete(
			() => _audioSource.Stop());
	}
	
	public void Stop(Action onComplete)
	{
		_shouldBePlaying = false;
		_audioSource.DOFade(0, _stopTransitionDuration).OnComplete(
			() =>
			{
				_audioSource.Stop();
				onComplete?.Invoke();
			});
	}
	
}
