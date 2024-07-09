using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource 컴포넌트
    public float audioDelayTime; // 첫 음악 재생 지연 시간
    public float audioDeltaTime = 0f; // 오디오 기준 DeltaTime
    private float previousAudioTime;

    void Start()
    {
        previousAudioTime = audioSource.time;
        audioSource.Stop();
        Invoke(nameof(StartMusic), audioDelayTime);
    }

    void FixedUpdate()
    {
        AudioDeltaTimeUpdate(); // 오디오 기준 DeltaTime 업데이트
        Debug.Log(audioDeltaTime);
    }

    private void StartMusic()
    {
        audioSource.Play();
    }

    public void AudioDeltaTimeUpdate() // 오디오 기준 DeltaTime 업데이트 메서드
    {
        if (audioSource.isPlaying)
        {
            float currentAudioTime = audioSource.time;
            audioDeltaTime = currentAudioTime - previousAudioTime;
            previousAudioTime = currentAudioTime;
            //Debug.Log(audioDeltaTime);
        }  
    }
}
