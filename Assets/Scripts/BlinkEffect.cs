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
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 1.0f;
    private TextMeshProUGUI textMeshPro;

    private Color originalColor;
    private float timer;

    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
        originalColor = textMeshPro.color;
        timer = 0.0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Abs(Mathf.Sin(timer * Mathf.PI / blinkDuration));
        Color newColor = originalColor;
        newColor.a = alpha;
        textMeshPro.color = newColor;

        // Reset timer to prevent overflow
        if (timer >= blinkDuration * 2)
        {
            timer -= blinkDuration * 2;
        }
    }
}
