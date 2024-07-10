using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private new Rigidbody2D rigidbody2D;
    private float audioDeltaTime; // 오디오 기준 델타타임
    private float accumulatedAudioDeltaTime; // 누적된 오디오 델타타임

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        audioDeltaTime = GameManager.instance.musicManager.audioDeltaTime; // 오디오 기준 델타타임 업데이트
        accumulatedAudioDeltaTime += audioDeltaTime; // 오디오 기준 델타타임 누적
        if (accumulatedAudioDeltaTime > Time.fixedDeltaTime) // fixedDeltaTime보다 누적된 오디오 델타타임 값이 크면
        {
            transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime); // fixedDeltaTime에 맞추어 플레이어 이동
            accumulatedAudioDeltaTime -= Time.fixedDeltaTime; // 누적된 오디오 기준 델타타임에서 fixedDeltaTime값 차감
        }
    }
}