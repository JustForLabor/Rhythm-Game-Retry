using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteDeactive : MonoBehaviour
{
    private float noteLastingTime = 7f; // 활성화되고 난 후 노트의 지속 시간 

    private void OnEnable() // 노트가 활성화되면 발동하는 이벤트
    {
        Invoke(nameof(NotePoolBack), noteLastingTime); // 노트 지속 시간 이후 노트 비활성화
    }

    private void NotePoolBack() // 노트 비활성화하는 메서드
    {
        gameObject.SetActive(false);
    }
}
