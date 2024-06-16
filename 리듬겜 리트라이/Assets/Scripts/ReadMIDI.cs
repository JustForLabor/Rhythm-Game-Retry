using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.IO;

public class ReadMIDI : MonoBehaviour
{
    public string midiFilePath;
    public static NoteData noteData;

    private void Start() {
        midiFilePath = $"Assets/MidiFiles/{midiFilePath}.mid"; // MidiFiles 폴더의 위치로 바로 설정하기
        noteData = ScriptableObject.CreateInstance<NoteData>();

        if (File.Exists(midiFilePath))
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

    void ExtractFromMidi(byte[] midiBytes, NoteData noteData) // Midi 데이터 추출
    {
        MidiFile midiFile = MidiFile.Read(new MemoryStream(midiBytes)); // 미디 파일
        var noteList = midiFile.GetNotes(); // 미디 파일의 노트 리스트
        TempoMap tempoMap = midiFile.GetTempoMap(); // 미디 파일의 TempoMap

        noteData.notes = new List<Note>(); // noteData의 notes 필드 설정

        foreach (var note in noteList)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap); // 노트의 시작 시간 가져와서 초 단위로 변환
            double metricTimeSpanInSecond = metricTimeSpan.TotalSeconds;

            KeyCode noteKey; // 인게임에서의 노트 입력키

            switch(note.NoteName) // 노트 음역에 맞는 노트 입력키 정하기
            {
                case NoteName.G:
                    noteKey = KeyCode.A;
                    break;
                default:
                    noteKey = KeyCode.None;
                    break;
            }
            
            // midi 정보를 바탕으로 note 객체 생성
            Note noteSet = ScriptableObject.CreateInstance<Note>();
            noteSet.time = metricTimeSpanInSecond;
            noteSet.inputKey = noteKey;
            
            noteData.notes.Add(noteSet); // 생성된 노트 객체를 noteData.notes에 넣기
        }
    }
}
