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
    public NoteData noteData;

    private void Start() {
        midiFilePath = $"Assets/MidiFiles/{midiFilePath}.mid"; // MidiFiles 폴더의 위치로 바로 설정하기

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
            
            // midi 정보를 바탕으로 note를 생성해서 noteData에 넣기
            Note noteSet = ScriptableObject.CreateInstance<Note>();
            noteSet.time = metricTimeSpanInSecond;
            noteSet.inputKey = SetNoteKey(note);

            noteData.notes.Add(noteSet);
        }
    }

    KeyCode SetNoteKey(Melanchall.DryWetMidi.Interaction.Note note) // 노트 음역에 맞는 키 가져오기
    {
        switch(note.NoteName)
            {
                case NoteName.G:
                    return KeyCode.A;
                default:
                    return KeyCode.None;
            }
    }
}
