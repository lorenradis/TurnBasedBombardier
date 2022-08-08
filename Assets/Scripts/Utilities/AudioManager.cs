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
    private AudioSource SecondarySFXSource;
    [SerializeField]
    private AudioSource ambientSource;

    public static AudioManager instance = null;

    public void PlayBGM(AudioClip clip)
    {
        if(clip != BGMSource.clip)
        {
            StartCoroutine(FadeToNewBGM(clip));

        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (SFXSource.isPlaying)
        {
            Debug.Log("should be playing " + clip.name + " on the secondary channel now");
            SecondarySFXSource.clip = clip;
            SecondarySFXSource.Play();
        }
        else
        {
            SFXSource.clip = clip;
            SFXSource.Play();
        }
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
