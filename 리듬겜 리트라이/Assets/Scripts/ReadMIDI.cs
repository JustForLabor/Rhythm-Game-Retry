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
    //======================================== 기본 설정 변수 ============================================

    public string midiFilePath; // Midi파일 이름
    public Vector2 notePositionOffset; // 노트 위치 오프셋
    public KeyCode[] inputKeys = new KeyCode[2]; // 플레이어 노트 입력키
    
    //======================================== MIDI 추출물 저장 변수 ============================================

    public NoteData noteData; // NoteData 클래스
    public static List<double> timeStamps; // 노트 타임스템프
    public static List<List<Note>> notesFromInputKey; // 입력키에 대해 분류한 노트 정보 (노트 히트 판정에 사용)

    private void Start()
    {
        midiFilePath = $"Assets/MidiFiles/{midiFilePath}.mid"; // MidiFiles 폴더의 위치로 바로 설정하기
        noteData = ScriptableObject.CreateInstance<NoteData>(); // NoteData 객체 생성

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

    private void ExtractFromMidi(byte[] midiBytes, NoteData noteData) // Midi 데이터 추출 메서드
    {
        MidiFile midiFile = MidiFile.Read(new MemoryStream(midiBytes)); // 미디 파일
        var noteList = midiFile.GetNotes(); // 미디 파일의 노트 리스트
        TempoMap tempoMap = midiFile.GetTempoMap(); // 미디 파일의 TempoMap

        noteData.notes = new List<Note>(); // noteData의 notes 필드 설정

        foreach (var note in noteList)
        {
            double metricTimeSpanInSecond = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds; // 노트의 시작 시간 가져와서 초 단위로 변환

            //Debug.Log(note.NoteName); // 노트 이름 확인용
            
            // midi 정보를 바탕으로 note 객체 생성 및 정보 입력
            Note noteSet = ScriptableObject.CreateInstance<Note>();
            noteSet.inputTime = metricTimeSpanInSecond;
            noteSet.inputKey = SetNoteKey(note);
            noteSet.position = SetNotePosition(noteSet.inputTime, noteSet);
            noteSet.noteName = note.NoteName;
            
            noteData.notes.Add(noteSet); // 생성된 노트 객체를 noteData.notes에 넣기
        }

        timeStamps = GetTimeStamps();// timeStamps 설정
        SetNotesFromInputKey(); // notesFromInputKey 설정

        /*/ 타임스템프 값 확인용
        foreach (double timeStamp in timeStamps)
        {
            Debug.Log(timeStamp);
        }
        /*/
    }

    private KeyCode SetNoteKey(Melanchall.DryWetMidi.Interaction.Note note) // 노트 입력키를 결정하는 메서드 (노트를 매개변수로 받음)
    { 
        KeyCode result; // 인게임에서의 노트 입력키

        switch(note.NoteName) // 노트 음역에 맞는 노트 입력키 정하기
        {
            case NoteName.A:
                result = inputKeys[0];
                break;

            case NoteName.G:
                result = inputKeys[1];
                break;    
            
            default:
                result = KeyCode.None;
                break;
        }

        return result;
    }

    private Vector2 SetNotePosition(double inputTime, Note note) // 노트 위치 결정 메서드 (노트 등장 시간, 입력 키를 매개변수로 받음)
    {
        Vector2 result; // 노트의 최종 위치

        result.x = 
        notePositionOffset.x +
        ((float)inputTime * GameManager.instance.playerController.moveSpeed) +
        (GameManager.instance.musicManager.audioDelayTime * GameManager.instance.playerController.moveSpeed); // 노트 시간과 플레이어 이동속도, 첫 음악 재생 지연 시간을 고려하여 x위치 설정

        if (note.inputKey == inputKeys[0]) // 노트의 입력키를 고려하여 y위치 설정
        {
            result.y = notePositionOffset.y + 0.5f;
        }

        else if (note.inputKey == inputKeys[1])
        {
            result.y = notePositionOffset.y + 3f;
        }

        else
        {
            result.y = 0;
        }

        return result;
    }

    private List<double> GetTimeStamps() // noteData 에서 타임스템프 관련 부분만 추출하는 메서드
    {
        List<double> timeStamps = new List<double>();

        foreach (Note note in noteData.notes)
        {
            timeStamps.Add(note.inputTime);
        }

        return timeStamps;
    }

    private void SetNotesFromInputKey() // noteData 노트를 입력키 별로 분류하는 메서드
    {
        notesFromInputKey = new List<List<Note>>();
        notesFromInputKey.Add(new List<Note>()); // inputKeys의 첫 번째 키로 입력하는 노트
        notesFromInputKey.Add(new List<Note>()); // inputKeys의 두 번째 키로 입력하는 노트  

        foreach (Note note in noteData.notes) // NoteData.notes에서 모든 노트에 대해 입력키를 비교하여 분류
        {
            if (note.inputKey == inputKeys[0])
            {
                notesFromInputKey[0].Add(note);
            }
            else if (note.inputKey == inputKeys[1])
            {
                notesFromInputKey[1].Add(note);
            }
        }
    }
}