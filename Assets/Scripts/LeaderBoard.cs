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
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class LeaderBoard : MonoBehaviour
{
    private const string databaseURL = "";

    public TextMeshProUGUI[] scoreTexts;

    private void OnEnable()
    {
        if (scoreTexts.Length != 10)
        {
            Debug.LogError("Please assign exactly 10 TextMeshProUGUI elements in the inspector");
            return;
        }

        StartCoroutine(GetScores());
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
                //Debug.Log(jsonResponse);

                var scores = JsonConvert.DeserializeObject<Dictionary<string, Data>>(jsonResponse);
                var listOfScores = scores.Values.ToList();
                listOfScores.Sort((a, b) => b.score.CompareTo(a.score));
                int index = 0;

                foreach (var score in listOfScores)
                {
                    if (index < scoreTexts.Length)
                    {
                        scoreTexts[index].text = $"User: {score.user}, Score: {score.score}";
                        index++;
                    }
                }

                for (int i = index; i < scoreTexts.Length; i++)
                {
                    scoreTexts[i].text = "";
                }
            }
        }
    }
}

[System.Serializable]
public class Data
{
    public string user;
    public int score;
}