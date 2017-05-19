using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[HeaderAttribute("Audio Sources")]
	public AudioSource music;
	public AudioSource SFX;
	public AudioSource keyboardSFX;

	[HeaderAttribute("Audio clips")]
	public List<AudioClip> keyboardTypingSoundList = new List<AudioClip>();
	public AudioClip backspaceSound;

	public void PlayKeyboardClick() {

		int randomClickSound = Random.Range(0, keyboardTypingSoundList.Count);

		keyboardSFX.PlayOneShot(keyboardTypingSoundList[randomClickSound]);

	}

	public void PlayBackspace() {

		keyboardSFX.PlayOneShot(backspaceSound);

	}

}