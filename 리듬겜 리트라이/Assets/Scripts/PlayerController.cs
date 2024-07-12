using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //======================================== 플레이어 이동 관련 변수 ============================================

    public float moveSpeed; // 플레이어 이동 속도
    private new Rigidbody2D rigidbody2D;    
    private float audioDeltaTime; // 오디오 기준 델타타임
    private float accumulatedAudioDeltaTime; // 누적된 오디오 델타타임

    //======================================== 노트 히트 관련 변수 ============================================

    public AudioSource hitSFX; // 노트를 쳤을 때 효과음
    private Dictionary<string, float> timeRange; // Perfect, Great 등의 판정 시간 범위 저장
    [SerializeField] private float noteMarginTolerance; // 노트 히트가 이 시간 이상으로 늦으면 자동으로 미스 판정
    [SerializeField] private float noteDisappearntTime; // 노트 미스 판정 후 비활성화되기까지의 시간
    private float audioTime; // 현재 오디오 재생 시간
    [SerializeField] private int index_A = 0; // A키에 대응하는 노트 인덱스
    [SerializeField] private int index_D = 0; // D키에 대응하는 노트 인덱스
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timeRange = new Dictionary<string, float>();
        timeRange.Add("Perfect", 0.1f);
        timeRange.Add("Great", 0.25f);
        timeRange.Add("Bad", 0.5f);
    }

    private void Update()
    {
        audioTime = GameManager.instance.musicManager.audioSource.time;

        MissDetector();

        if (Input.GetKeyDown(GameManager.instance.readMIDI.inputKeys[0]))
        {
            NoteHitDetector(ReadMIDI.notesFromInputKey[0], ref index_A);
        }

        if (Input.GetKeyDown(GameManager.instance.readMIDI.inputKeys[1]))
        {
            NoteHitDetector(ReadMIDI.notesFromInputKey[1], ref index_D);
        }
    } 

    void FixedUpdate()
    {
        PlayerMovementProcess();
    }

    private void PlayerMovementProcess()
    {
        audioDeltaTime = GameManager.instance.musicManager.audioDeltaTime; // 오디오 기준 델타타임 업데이트
        accumulatedAudioDeltaTime += audioDeltaTime; // 오디오 기준 델타타임 누적
        if (accumulatedAudioDeltaTime > Time.fixedDeltaTime) // fixedDeltaTime보다 누적된 오디오 델타타임 값이 크면
        {
            transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime); // fixedDeltaTime에 맞추어 플레이어 이동
            accumulatedAudioDeltaTime -= Time.fixedDeltaTime; // 누적된 오디오 기준 델타타임에서 fixedDeltaTime값 차감
        }
    }

    private void NoteHitDetector(List<Note> targetNoteList, ref int index) // 노트 히트 판정 메서드
    // targetNoteList : 노트 입력키에 따라서 조사할 노트셋 (입력키 별로 분류한 리스트에서 가져옴)
    {
        //Debug.Log($"index : {index} / object : {targetNoteList[index]}");

        if (index >= targetNoteList.Count) // 인덱스 범위 검사
        {return;}

        if (Mathf.Abs((float)targetNoteList[index].inputTime - audioTime) <= timeRange["Perfect"])
        {
            Debug.Log("Perfect!");
            hitProcess(targetNoteList, ref index);
        }

        else if (Mathf.Abs((float)targetNoteList[index].inputTime - audioTime) <= timeRange["Great"])
        {
            Debug.Log("Great!");
            hitProcess(targetNoteList, ref index);
        }

        else if (Mathf.Abs((float)targetNoteList[index].inputTime - audioTime) <= timeRange["Bad"])
        {
            Debug.Log("Bad...");
            hitProcess(targetNoteList, ref index);
        } 
    }

    private void hitProcess(List<Note> targetNoteList, ref int index) // 노트 히트 처리
    {
        StartCoroutine(NoteDisappearant(targetNoteList, index, 0f));
        index++;
    }

    private void MissDetector() // 미스 판정 확인 메서드
    {
        // 현재 오디오 시간 - 노트를 눌러야하는 시간 > 딜레이 허용 시간이면 미스 판정 및 노트 인덱스 +1
        //Debug.Log($"{audioTime}, {ReadMIDI.notesFromInputKey[0][index_A].inputTime}, {ReadMIDI.notesFromInputKey[1][index_D].inputTime}");
        
        if (index_A < ReadMIDI.notesFromInputKey[0].Count)
        {
            if (audioTime - ReadMIDI.notesFromInputKey[0][index_A].inputTime >= noteMarginTolerance)
            {
                if (ReadMIDI.notesFromInputKey[0][index_A].isDeactived == false)
                {
                    Debug.Log("Miss...");
                    StartCoroutine(NoteDisappearant(ReadMIDI.notesFromInputKey[0], index_A, noteDisappearntTime));
                    index_A++;
                }
            }
        }

        if (index_D < ReadMIDI.notesFromInputKey[1].Count)
        {
            if (audioTime - ReadMIDI.notesFromInputKey[1][index_D].inputTime >= noteMarginTolerance)
            {
                if (ReadMIDI.notesFromInputKey[1][index_D].isDeactived == false)
                {
                    Debug.Log("Miss...");
                    StartCoroutine(NoteDisappearant(ReadMIDI.notesFromInputKey[1], index_D, noteDisappearntTime));
                    index_D++;
                }
            }
        }
    }

    private IEnumerator NoteDisappearant(List<Note> targetList, int index, float decativeTime) // 노트 비활성화 코루틴
    {
        GameObject noteObject = targetList[index].noteObject;

        if (targetList[index].isDeactived == true) // 이미 노트가 비활성화 되었으면 하던거 취소
        {yield break;}

        yield return new WaitForSeconds(decativeTime);
        noteObject.SetActive(false);
        targetList[index].isDeactived = true;
        index++;

        Debug.Log($"{ReadMIDI.notesFromInputKey.IndexOf(targetList)}th list, {index - 1} note is deactived. current index : {index}");
    }
}