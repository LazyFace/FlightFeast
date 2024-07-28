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

public class SoundsUIController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if(AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);
    }
}
