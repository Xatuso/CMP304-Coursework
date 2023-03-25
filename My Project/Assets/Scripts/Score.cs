using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        scoreText.text = Level.GetInstace().GetPipesPassed().ToString();
    }
}
