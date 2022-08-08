using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource BGMSource;
    [SerializeField]
    private AudioSource SFXSource;
    [SerializeField]
    private AudioSource ambientSource;

    public void PlayBGM(AudioClip clip)
    {
        if(clip != BGMSource.clip)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void SetAmbience(AudioClip clip)
    {

    }

    private IEnumerator FadeToNewBGM(AudioClip clip)
    {
        float duration = .5f;
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            BGMSource.volume = Mathf.Lerp(1, 0, elapsedTime / duration);
            yield return null;
        }
        BGMSource.clip = clip;
        BGMSource.volume = 1f;
        BGMSource.Play();
    }
}
