using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

[CreateAssetMenu(fileName = "NoteData", menuName = "ScriptableObjects/NoteData", order = 1)]
public class NoteData : ScriptableObject
{
    public List<Note> notes;
}

[CreateAssetMenu(fileName = "Note", menuName = "ScriptableObjects/Note", order = 2)]
public class Note : ScriptableObject
{
    public double inputTime; // 입력 시간
    public KeyCode inputKey; // 입력 키
    public Vector2 position; // 노트 위치
    public NoteName noteName; // 노트 음 이름
}

public class LongNote : Note
{
    //NoteType noteType = NoteType.LongNote;
}
