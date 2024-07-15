using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitFeedbackUI : MonoBehaviour
{
    public TextMeshProUGUI comboUI;

    public void changeText(string text)
    {
        comboUI.text = text;
    }
}
