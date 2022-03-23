using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour 
{
	public Sound[] sounds;
	public AudioSource currentSound;
	public float lastSoundTime;

	public int currentRunSound;

	void Start()
	{
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}

		GameManager.instance.tapEvent += PlayTapSound;
		//GameManager.instance.crystalTaken.AddListener(PlayCrystalSound);
		GameManager.instance.dificultyEvent += SetRunSoundSpeed;
		GameManager.instance.levelComplete.AddListener(PlayCompleteSound);
		GameManager.instance.levelLose.AddListener(PlayLoseSound);
		GameManager.instance.levelStart.AddListener(PlayRunSound);
	}

	public void Play(string sound)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (currentSound) currentSound.Stop();
		currentSound = s.source;
		currentSound.Play();
	}

	public IEnumerator PlayWhileRun(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (currentSound) currentSound.Stop();
		currentSound = s.source;
		currentSound.Play();

        while (currentSound.isPlaying)
        {
			yield return null;
		}
		PlayRunSound();
	}

	private void PlayTapSound(Commands command)
    {
		if(command == Commands.Jump) StartCoroutine(PlayWhileRun("Jump"));	
		else StartCoroutine(PlayWhileRun("Turn"));
	}

	private void PlayCompleteSound()
	{
		StartCoroutine(PlayWhileRun("Complete"));
	}
	private void PlayLoseSound()
	{
		StopAllCoroutines();
		Play("Lose");
	}

	private void PlayCrystalSound()
    {
		Play("Crystal");
	}

	private void PlayRunSound()
    {
        switch(currentRunSound)
		{
			case 1:
				Play("Run1");
				break;

			case 2:
				Play("Run2");
				break;

			case 3:
				Play("Run3");
				break;
		}
    }

	private void SetRunSoundSpeed(int speed)
    {
		currentRunSound = speed;
    }
}
