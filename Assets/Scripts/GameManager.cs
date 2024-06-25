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

using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState state { get; private set; }

    public static event Action<GameState> OnStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.PassangersBoarding);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (state == GameState.Continue || state == GameState.PassangersSeated)
            {
                UpdateGameState(GameState.Paused);
                Time.timeScale = 0f;
            }
            else if (state == GameState.Paused)
            {
                Time.timeScale = 1f;
                UpdateGameState(GameState.Continue);
            }
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.PassangersBoarding:
                break;
            case GameState.SkipBoarding:
                break;
            case GameState.PassangersSeated:
                break;
            case GameState.Continue:
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
            default:
                Debug.Log("What?");
                break;
        }

        OnStateChange?.Invoke(newState);
    }
}

public enum GameState
{
    PassangersBoarding,
    SkipBoarding,
    PassangersSeated,
    Continue,
    Paused,
    GameOver
}