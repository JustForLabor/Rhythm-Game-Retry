using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms; // 오브젝트 풀링 네임스페이스

public class NoteInstantiate : MonoBehaviour
{
    public GameObject notePrefab; // 노트 Prefab
    public List<GameObject> notePool; // 노트 오브젝트 풀
    public Transform noteFolder; // 노트 담아놓을 폴더

    public float prepositionTIme; // 사전에 노트를 미리 생성할 시간
    private int noteIndex = 0; // 노트 순서 인덱스

    private void Awake()
    {
        notePool = new List<GameObject>(); // 노트 오브젝트 풀 초기화
    }

    private void Update()
    {
        ArrangeNotes();
    }

    public void ArrangeNotes() // 노트 어레인지 메서드 (ReadMIDI의 noteData 변수값을 바탕으로 노트 생성)
    {
        if (ReadMIDI.timeStamps.Count >= noteIndex) // 타임스템프 개수가 noteIndex보다 클 때 (타임스템프 개수 이상으로 노트를 생성하는 오류 방지) ************* 타임스템프 개수에 뭔가 문제가 있는거 같으니까 나중에 Debug.Log로 Count값 살펴보자
        {
            if (GameManager.instance.musicManager.audioSource.time >= ReadMIDI.timeStamps[noteIndex] - prepositionTIme) // 현재 오디오 재생 시간이 해당 순서 노트의 (타임스템프 - 미리 등장할 시간)일 때
            {
                GameObject newNote = getNoteObject(); // 오브젝트 풀에서 노트 가져오기
                newNote.transform.position = GameManager.instance.readMIDI.noteData.notes[noteIndex].position; // 노트 위치 설정
                newNote.SetActive(true); // 노트 활성화
                
                noteIndex++;
            }
        }
    }

    public GameObject getNoteObject() // 풀에서 가져올 오브젝트 선택하기
    {
        GameObject result = null; // 봔환값의 기본값 설정

        foreach (GameObject note in notePool) // 노트 오브젝트 풀의 각각의 오브젝트를 조사
        {
            if (note.activeSelf == false) // 오브젝트가 비활성화 상태이면 (사용하고 있지 않을 때)
            {
                result = note; // 반환값에 해당 오브젝트 할당
                break; // 반복문 종료
            }
        }

        if (result == null) // 오브젝트 풀의 모든 오브젝트가 사용 중이라서 가져올 게 없을 때
        {
            result = Instantiate(notePrefab, noteFolder); // 반환값에 오브젝트를 새로 만들어서 할당하기
            notePool.Add(result); // 노트를 오브젝트 풀에 넣어서 오브젝트 풀의 오브젝트 개수 확장
        }

        return result;
    }
}