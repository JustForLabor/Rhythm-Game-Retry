using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private new Rigidbody2D rigidbody2D;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //rigidbody2D.MovePosition(rigidbody2D.position + Vector2.right * moveSpeed * Time.deltaTime);
        rigidbody2D.velocity = Vector2.right * moveSpeed;
    }
}


// 플레이어 움직임과 노트 싱크가 안 맞는 이유 > 오디오 상에서 흐른 시간과 DeltaTime값에 괴리가 발생하기 때문이다.
// 이를 해결하기 위해서는 오디오를 기준으로 한 새로운 DeltaTime을 정의할 필요성이 있다.