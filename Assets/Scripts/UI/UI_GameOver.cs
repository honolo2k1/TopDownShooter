using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
    public TextMeshProUGUI GameOverText;

    public void ShowGameOverMessage(string message)
    {
        GameOverText.text = message;
    }
}
