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
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class SendFinalScore : MonoBehaviour
{
    private const string databaseURL = "";

    private List<Data> scoreList = new List<Data>();

    [SerializeField] private TMP_InputField usernameInputField;
    private int finalScore = 0;

    private void Awake()
    {
        EventManager.OnScoreChanged += UpdateFinalScore;
    }

    private void OnDestroy()
    {
        EventManager.OnScoreChanged -= UpdateFinalScore;
    }

    public void SendScore()
    {
        //Debug.Log("SendScore called");
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            //Debug.Log("Username is empty. Using default username");
            StartCoroutine(HandleSubmitScore("000", finalScore));
        }
        else
        {
            //Debug.Log("Username is: " + usernameInputField.text);
            StartCoroutine(HandleSubmitScore(usernameInputField.text, finalScore));
        }
    }

    private IEnumerator HandleSubmitScore(string username, int score)
    {
        //Debug.Log("HandleSubmitScore started");
        yield return StartCoroutine(GetScores());

        if (IsHighScore(score))
        {
            //Debug.Log("New high score. Updating score list");
            UpdateScoreList(username, score);
            yield return StartCoroutine(SendScores());
        }else
        {
            //Debug.Log("Score is not high enough to be added to the leaderboard");
            LoadingSceneManager.Instance.LoadScene("MainMenu");
        }
    }

    private IEnumerator GetScores()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(databaseURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                //Debug.Log("Scores fetched: " + jsonResponse);
                scoreList = JsonConvert.DeserializeObject<Dictionary<string, Data>>(jsonResponse).Values.ToList();
            }
        }
    }

    private bool IsHighScore(int score)
    {
        if (scoreList.Count < 10)
        {
            return true;
        }

        return score > scoreList[scoreList.Count - 1].score;
    }

    private void UpdateScoreList(string username, int score)
    {
        scoreList.Add(new Data { user = username, score = score });
        scoreList.Sort((a, b) => b.score.CompareTo(a.score));

        if (scoreList.Count > 10)
        {
            scoreList.RemoveAt(scoreList.Count - 1);
        }

        //Debug.Log("Score list updated. Current scores:");
    }

    private IEnumerator SendScores()
    {
        var scoreDict = scoreList.Select((score, index) => new { Key = "user" + (index + 1).ToString("D2"), Value = score })
                                 .ToDictionary(x => x.Key, x => x.Value);

        string jsonData = JsonConvert.SerializeObject(scoreDict);

        using (UnityWebRequest webRequest = UnityWebRequest.Put(databaseURL, jsonData))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Scores updated successfully.");
                LoadingSceneManager.Instance.LoadScene("MainMenu");
            }
        }
    }

    private void UpdateFinalScore(int newScore)
    {
        finalScore = newScore;
        //Debug.Log("Score updated: " + finalScore);
    }
}