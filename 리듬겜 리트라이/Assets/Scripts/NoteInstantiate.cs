using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteInstantiate : MonoBehaviour
{
    public GameObject notePrefab; // 노트 Prefab
    public Transform noteFolder; // 노트 담아놓을 폴더
    void Start()
    {
        StartCoroutine(InstantiateNotes());
    }

    void Update()
    {

    }

    public IEnumerator InstantiateNotes() // 노트 생성 메서드 (ReadMIDI의 noteData 변수값을 바탕으로 노트  생성)
    {
        yield return new WaitForSeconds(3f);

        foreach (Note note in ReadMIDI.instance.noteData.notes)
        {
            GameObject newNote = Instantiate(notePrefab);
            newNote.transform.SetParent(noteFolder);
            newNote.transform.position = note.position;
        }
    }
}
