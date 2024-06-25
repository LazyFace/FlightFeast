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

public class ScoreManager : MonoBehaviour, IObserver
{
    private int score = 0;

    [SerializeField] private int correctOrderScoreToAdd = 100;
    [SerializeField] private int orderLostScoreToRemove = 25;
    [SerializeField] private int scoreToRemovePilotAsleep = 5;

    private void Awake()
    {
        GameManager.OnStateChange += GameManagerOnStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnStateChange -= GameManagerOnStateChange;

        PassangerController[] passangers = FindObjectsOfType<PassangerController>();
        Pilot pilot = FindObjectOfType<Pilot>();

        foreach (var passanger in passangers)
        {
            passanger.Unsubscribe(this);
        }

        if (pilot != null)
        {
            pilot.Unsubscribe(this);
        }
    }

    private void GameManagerOnStateChange(GameState state)
    {
        if (state == GameState.PassangersSeated)
        {
            PassangerController[] passangers = FindObjectsOfType<PassangerController>();
            Pilot pilot = FindObjectOfType<Pilot>();

            foreach (var passanger in passangers)
            {
                passanger.Subscribe(this);
            }
            pilot.Subscribe(this);
        }else if(state == GameState.GameOver)
        {
            EventManager.TriggerScoreChanged(score);
        }
    }

    public void Notify(Actions action)
    {
        switch (action)
        {
            case Actions.CorrectOrderReceived:
                AddScore(correctOrderScoreToAdd);
                break;
            case Actions.OrderLost:
                RemoveScore(orderLostScoreToRemove);
                break;
            case Actions.PilotAsleep:
                RemoveScore(scoreToRemovePilotAsleep);
                break;
            default:
                Debug.Log("Wtf");
                break;
        }
    }

    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        EventManager.TriggerScoreChanged(score);
    }

    private void RemoveScore(int scoreToRemove)
    {
        score -= scoreToRemove;

        if (score < 0)
        {
            score = 0;
        }

        EventManager.TriggerScoreChanged(score);
    }
}