using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.IO;

public class ReadMIDI : MonoBehaviour
{
    public string midiFilePath; // Midi파일 이름
    public PlayerController playerController; // PlayerController 스크립트
    public NoteData noteData; // NoteData 클래스
    public Vector2 notePositionAxis;

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
            
            // midi 정보를 바탕으로 note 객체 생성 및 정보 입력
            Note noteSet = ScriptableObject.CreateInstance<Note>();
            noteSet.inputTime = metricTimeSpanInSecond;
            noteSet.inputKey = SetNoteKey(note);
            noteSet.position = SetNotePosition(noteSet.inputTime, noteSet.inputKey);
            
            noteData.notes.Add(noteSet); // 생성된 노트 객체를 noteData.notes에 넣기
        }
    }

    KeyCode SetNoteKey(Melanchall.DryWetMidi.Interaction.Note note) // 노트 입력키를 결정하는 메서드 (노트를 매개변수로 받음)
    {
        KeyCode result; // 인게임에서의 노트 입력키

        switch(note.NoteName) // 노트 음역에 맞는 노트 입력키 정하기
        {
            case NoteName.G:
                result = KeyCode.A;
                break;
            default:
                result = KeyCode.None;
                break;
        }

        return result;
    }

    Vector2 SetNotePosition(double inputTime, KeyCode inputKey) // 노트 위치 결정 메서드 (노트 등장 시간, 입력 키를 매개변수로 받음)
    {
        Vector2 result;

        result.x = notePositionAxis.x + ((float)inputTime * playerController.moveSpeed);

        switch(inputKey)
        {
            case KeyCode.A:
                result.y = notePositionAxis.y + 1f;
                break;
            default:
                result.y = notePositionAxis.y;
                break;
        }

        return result;
    }
}
