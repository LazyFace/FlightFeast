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

using System.Collections;
using UnityEngine;

public class Pilot : Subject, IInteractable
{
    [SerializeField] private float pilotSleepDelay1 = 20;
    [SerializeField] private float pilotSleepDelay2 = 30;
    [SerializeField] private GameObject sleepingImage;
    [SerializeField] private AudioSource pilotAudioSource;
    bool isAsleep = false;
    private Coroutine DelaySleepCoroutine;
    private Coroutine SleepingCoroutine;

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
            StartSleepDelay();
        }else if(state == GameState.GameOver)
        {
            StopSleepDelay();
        }
    }

    public void StartSleepDelay()
    {
        float delayDuration = Random.Range(pilotSleepDelay1, pilotSleepDelay2);
        if (DelaySleepCoroutine != null)
        {
            StopCoroutine(DelaySleepCoroutine);
        }
        DelaySleepCoroutine = StartCoroutine(DelaySleep(delayDuration));
    }

    public void StopSleepDelay()
    {
        if (DelaySleepCoroutine != null)
        {
            StopCoroutine(DelaySleepCoroutine);
        }
    }

    private IEnumerator DelaySleep(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartSleeping();
    }

    private void StartSleeping()
    {
        isAsleep = true;
        if (SleepingCoroutine != null)
        {
            StopCoroutine(SleepingCoroutine);
        }
        pilotAudioSource.Play();
        SleepingCoroutine = StartCoroutine(Sleep());
        sleepingImage.SetActive(true);
        //Debug.Log("Pilot has started sleeping");
    }

    private IEnumerator Sleep()
    {
        while (isAsleep)
        {
            Notify(Actions.PilotAsleep);
            yield return new WaitForSeconds(2);
        }
    }

    private void WakeUp()
    {
        if (isAsleep)
        {
            isAsleep = false;
            if (DelaySleepCoroutine != null)
            {
                pilotAudioSource.Stop();
                StopCoroutine(SleepingCoroutine);
                StartSleepDelay();
            }
            sleepingImage.SetActive(false);
            //Debug.Log("Pilot has been awakened");
            
        }
        else
        {
            //Debug.Log("The pilot is not asleep");
        }
    }

    public void Interact(PickAndUse interactor)
    {
        WakeUp();
    }
}