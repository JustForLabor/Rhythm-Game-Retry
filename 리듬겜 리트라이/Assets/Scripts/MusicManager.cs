using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource 컴포넌트
    public float audioDelayTime; // 첫 음악 재생 지연 시간
    public double audioDeltaTime = 0f; // 오디오 기준 DeltaTime

    void Start()
    {
        audioSource.Stop();
        Invoke(nameof(StartMusic), audioDelayTime);
    }

    void Update()
    {
        
    }

    private void StartMusic()
    {
        audioSource.Play();
    }

    public double GetAudioSourceTime() // 현재 재생 중인 음악의 시간 위치를 구하는 메서드
    {
        return (double) audioSource.timeSamples / audioSource.clip.frequency; // 현재 재생중인 오디오의 샘플 시간 / 샘플링 주파수 = 시간
    }
}
