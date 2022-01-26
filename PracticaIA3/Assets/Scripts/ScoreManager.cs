using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;

    [SerializeField]
    private Text score;

    [SerializeField]
    private Text highScore;

    private bool isHighScore = false;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;

            highScore.text = "" + PlayerPrefs.GetFloat("HighScore");
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore()
    {
        score.text = "" + (int.Parse(score.text) + 1);
        if (isHighScore || int.Parse(score.text) > int.Parse(highScore.text))
        {
            highScore.text = score.text;
            PlayerPrefs.SetFloat("HighScore", int.Parse(highScore.text));
            isHighScore = true;
        }
    }

    public void ResetScore()
    {
        score.text = "0";
        isHighScore = false;
    }
}
