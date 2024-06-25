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
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingBar.value = 0f;
        loadingScreen.SetActive(true);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        while (scene.progress < 0.9f)
        {
            loadingBar.value = Mathf.Clamp01(scene.progress / 0.9f);

            // Agrega una espera artificial para simular carga más larga
            yield return new WaitForSeconds(0.1f);
        }

        loadingBar.value = 1f;
        yield return new WaitForSeconds(0.5f);

        scene.allowSceneActivation = true;

        yield return new WaitForSeconds(0.5f);
        loadingScreen.SetActive(false);
    }
}
