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
    public double time;
    public KeyCode inputKey;
    public Vector2 position;
}
