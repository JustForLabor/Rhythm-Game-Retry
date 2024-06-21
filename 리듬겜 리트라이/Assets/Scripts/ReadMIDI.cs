using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 미디 관련 라이브러리
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

public class ReadMIDI : MonoBehaviour
{
    public static ReadMIDI instance; // ReadMIDI 싱글턴 인스턴스

    public string midiFilePath; // Midi파일 이름
    public PlayerController playerController; // PlayerController 스크립트
    public NoteData noteData; // NoteData 클래스
    public Vector2 notePositionOffset; // 노트 위치 오프셋

    private void Awake() 
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

    private void Start() {
        midiFilePath = $"Assets/MidiFiles/{midiFilePath}.mid"; // MidiFiles 폴더의 위치로 바로 설정하기
        noteData = ScriptableObject.CreateInstance<NoteData>(); // NoteData 객체 생성
        playerController = FindObjectOfType<PlayerController>(); // PlayerController 스크립트 할당

        if (File.Exists(midiFilePath)) // Midi 파일이 경로에 존재하면
        {
            Debug.Log($"{midiFilePath} 파일 로딩");

            byte[] midiBytes = File.ReadAllBytes(midiFilePath);
            ExtractFromMidi(midiBytes, noteData);
        }
        else
        {
            Debug.LogError("Midi 파일이 존재하지 않음");
        }
    }

    void ExtractFromMidi(byte[] midiBytes, NoteData noteData) // Midi 데이터 추출 메서드
    {
        MidiFile midiFile = MidiFile.Read(new MemoryStream(midiBytes)); // 미디 파일
        var noteList = midiFile.GetNotes(); // 미디 파일의 노트 리스트
        TempoMap tempoMap = midiFile.GetTempoMap(); // 미디 파일의 TempoMap

        noteData.notes = new List<Note>(); // noteData의 notes 필드 설정

        foreach (var note in noteList)
        {
            double metricTimeSpanInSecond = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds; // 노트의 시작 시간 가져와서 초 단위로 변환

            Debug.Log(note.NoteName);
            
            // midi 정보를 바탕으로 note 객체 생성 및 정보 입력
            Note noteSet = ScriptableObject.CreateInstance<Note>();
            noteSet.inputTime = metricTimeSpanInSecond;
            noteSet.inputKey = SetNoteKey(note);
            noteSet.position = SetNotePosition(noteSet.inputTime, note);
            noteSet.noteName = note.NoteName;
            
            noteData.notes.Add(noteSet); // 생성된 노트 객체를 noteData.notes에 넣기
        }
    }

    KeyCode SetNoteKey(Melanchall.DryWetMidi.Interaction.Note note) // 노트 입력키를 결정하는 메서드 (노트를 매개변수로 받음)
    {
        KeyCode result; // 인게임에서의 노트 입력키

        switch(note.NoteName) // 노트 음역에 맞는 노트 입력키 정하기
        {
            case NoteName.A:
                result = KeyCode.A;
                break;

            case NoteName.G:
                result = KeyCode.D;
                break;    
            
            default:
                result = KeyCode.None;
                break;
        }

        return result;
    }

    Vector2 SetNotePosition(double inputTime, Melanchall.DryWetMidi.Interaction.Note note) // 노트 위치 결정 메서드 (노트 등장 시간, 입력 키를 매개변수로 받음)
    {
        Vector2 result; // 노트의 최종 위치

        result.x = notePositionOffset.x + ((float)inputTime * playerController.moveSpeed); // 노트 시간과 플레이어 이동속도를 고려하여 x위치 설정

        switch(note.NoteName) // 노트의 입력키를 고려하여 y위치 설정
        {
            case NoteName.A:
                result.y = notePositionOffset.y + 3f;
                break;
            case NoteName.G:
                result.y = notePositionOffset.y + 0.5f;
                break;
            default:
                result.y = notePositionOffset.y;
                break;
        }

        return result;
    }
}
