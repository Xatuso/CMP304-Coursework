using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        Hide();
    }

    private void Start()
    {
        BirdControls.GetInstance().onDied += Bird_onDied;
    }

    private void Bird_onDied(object sender, System.EventArgs e)
    {
        scoreText.text = Level.GetInstace().GetPipesPassed().ToString();
        Debug.Log("THISISCALLEd");
        Show();
    }

    public void RetryButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        
    }
}
