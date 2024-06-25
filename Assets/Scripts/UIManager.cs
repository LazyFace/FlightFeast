// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using TMPro;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject initialUI;
    [SerializeField] private GameObject skipText;
    [SerializeField] private GameObject pauseMenu;

    [Header("Game progression")]
    [SerializeField] private GameObject indicators;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Level Finish")]
    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;

    private void Awake()
    {
        GameManager.OnStateChange += GameManagerOnStateChange;
        EventManager.OnScoreChanged += UpdateScoreUI;
        EventManager.OnTimerChange += UpdateTimerUI;
    }

    private void OnDestroy()
    {
        GameManager.OnStateChange -= GameManagerOnStateChange;
        EventManager.OnScoreChanged -= UpdateScoreUI;
        EventManager.OnTimerChange -= UpdateTimerUI;
    }

    private void GameManagerOnStateChange(GameState state)
    {
        if (state == GameState.Paused)
        {
            pauseMenu.SetActive(true);
        }
        else if (state == GameState.Continue)
        {
            pauseMenu.SetActive(false);
        }
        else if (state == GameState.PassangersSeated)
        {
            initialUI.SetActive(false);
            indicators.SetActive(true);
        }
        else if (state == GameState.SkipBoarding)
        {
            skipText.SetActive(false);
        }
        else if(state == GameState.GameOver)
        {
            isGameOver = true;
            gameOver.SetActive(true);
            indicators.SetActive(false);
            finalScoreText.text = scoreText.text;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isGameOver)
            {
                GameManager.Instance.UpdateGameState(GameState.SkipBoarding);
            }
            else
            {
                //LoadingSceneManager.Instance.LoadScene("MainMenu");
            }
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            timerText.text = timeText;
        }
    }
}