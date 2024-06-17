using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "ScriptableObjects/NoteData", order = 1)]
public class NoteData : ScriptableObject
{
    public List<Note> notes;
}

[CreateAssetMenu(fileName = "Note", menuName = "ScriptableObjects/Note", order = 2)]
public class Note : ScriptableObject
{
    public enum NoteType { // 노트 종류 모음
        Note,
        LongNote
    }
    public double inputTime; // 입력 시간
    public KeyCode inputKey; // 입력 키
    public Vector2 position; // 노트 위치
}

public class LongNote : Note
{
    NoteType noteType = NoteType.LongNote;
}
