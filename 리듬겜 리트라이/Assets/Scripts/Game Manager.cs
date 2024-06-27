using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager 인스턴스

    // -------------------------------------------------------------------------- 스크립트의 모든 변수와 메서드는 GameManager를 통해서 참조 가능하도록 설계 
    public ReadMIDI readMIDI;
    public PlayerController playerController;
    public NoteInstantiate noteInstantiate;
    public MusicManager musicManager;
    // --------------------------------------------------------------------------

    private void Awake() // 싱글턴 인스턴스 설정
    {
        if (instance == null)
        {
            instance = this;
        }    
        else
        {
            Destroy(this);
        }
    
    }
}
