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

using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float levelDuration = 300f;
    private float timeRemaining = 0f;
    private bool isLevelActive = false;
    private bool isPaused = false;


    private void Awake()
    {
        GameManager.OnStateChange += GameManagerOnStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnStateChange -= GameManagerOnStateChange;
    }

    private void GameManagerOnStateChange(GameState state)
    {
        if (state == GameState.PassangersSeated)
        {
            StartLevelTimer();
        }
        else if (state == GameState.Paused)
        {
            PauseLevelTimer();
        }
        else if (state == GameState.Continue)
        {
            ResumeLevelTimer();
        }
    }

    private void Update()
    {
        if (isLevelActive && !isPaused)
        {
            timeRemaining -= Time.deltaTime;
            EventManager.TriggerTimerChanged(timeRemaining);
            if (timeRemaining <= 0)
            {
                EndLevel();
            }
        }
    }

    public void StartLevelTimer()
    {
        timeRemaining = levelDuration;
        isLevelActive = true;
        isPaused = false;
    }

    public void PauseLevelTimer()
    {
        isPaused = true;
    }

    public void ResumeLevelTimer()
    {
        isPaused = false;
    }

    private void EndLevel()
    {
        isLevelActive = false;
        GameManager.Instance.UpdateGameState(GameState.GameOver);
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}
