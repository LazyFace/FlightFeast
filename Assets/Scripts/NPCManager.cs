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
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    [Header("Passangers Spawn Points")]
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform leftSpawnPoint;
    private bool spawnNext = true;
    [SerializeField] private GameObject rightEntrance;
    [SerializeField] private GameObject leftEntrance;

    [SerializeField] private List<SeatRow> seatsRows;

    [SerializeField] private float spawnDelay = 1.0f;

    private bool allNPCsSpawned = false;
    private bool allNPCsSeated = false;

    [SerializeField] private GameObject[] passengersToSpawn;
    private List<GameObject> passengersSpawned = new List<GameObject>();

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
        if(state == GameState.SkipBoarding)
        {
            if (!allNPCsSpawned)
            {
                StartCoroutine(SpawnAllNPCsCoroutine());
            }
        }
        else if(state == GameState.PassangersBoarding)
        {
            StartNPCSpawn();
        }else if(state == GameState.PassangersSeated)
        {
            StartMakingOrders();
        }else if(state == GameState.GameOver)
        {
            StopMakingOrders();
        }
    }

    private void LateUpdate()
    {
        if (allNPCsSpawned && !allNPCsSeated)
        {
            CheckAllNPCsSeated();
        }
    }

    public void StartNPCSpawn()
    {
        InitializeTargetQueues();
        StartCoroutine(SpawnNPCsEquitably());
    }

    private void InitializeTargetQueues()
    {
        foreach (var seatRow in seatsRows)
        {
            seatRow.InitializeSeatsQueue();
        }
    }

    private IEnumerator SpawnNPCsEquitably()
    {
        bool spawnedThisCycle = false;

        while (!allNPCsSpawned)
        {
            spawnedThisCycle = false;

            foreach (var seatRow in seatsRows)
            {
                if (seatRow.HasAvailableSeat())
                {
                    GameObject selectedPassenger = passengersToSpawn[Random.Range(0, passengersToSpawn.Length)];
                    GameObject npc;
                    if (spawnNext)
                    {
                        npc = Instantiate(selectedPassenger, rightSpawnPoint);
                        npc.transform.SetParent(null);
                        spawnNext = false;
                    }
                    else
                    {
                        npc = Instantiate(selectedPassenger, leftSpawnPoint);
                        npc.transform.SetParent(null);
                        spawnNext = true;
                    }
                    seatRow.SetSeat(npc.GetComponent<PassangerController>());
                    passengersSpawned.Add(npc);
                    spawnedThisCycle = true;
                }
                yield return new WaitForSeconds(spawnDelay);
            }

            if (!spawnedThisCycle)
            {
                allNPCsSpawned = true;
            }
        }
    }

    private void CheckAllNPCsSeated()
    {
        allNPCsSeated = true;
        foreach (GameObject npc in passengersSpawned)
        {
            PassangerController controller = npc.GetComponent<PassangerController>();
            if (controller != null && !controller.IsAgentAtDestination())
            {
                allNPCsSeated = false;
                break;
            }
        }

        if (allNPCsSeated)
        {
            GameManager.Instance.UpdateGameState(GameState.PassangersSeated);
            rightEntrance.SetActive(false);
            leftEntrance.SetActive(false);
        }
    }

    private void StartMakingOrders()
    {
        if (allNPCsSeated)
        {
            foreach (GameObject npc in passengersSpawned)
            {
                PassangerController controller = npc.GetComponent<PassangerController>();
                controller.StartMakingOrders();
            }
        }
    }

    private void StopMakingOrders()
    {
        if (allNPCsSeated)
        {
            foreach (GameObject npc in passengersSpawned)
            {
                PassangerController controller = npc.GetComponent<PassangerController>();
                controller.StopMakingOrders();
            }
        }
    }

    private IEnumerator SpawnAllNPCsCoroutine()
    {
        spawnDelay = 0f;

        yield return new WaitForSeconds(2f);

        foreach (GameObject npc in passengersSpawned)
        {
            PassangerController controller = npc.GetComponent<PassangerController>();
            controller.TeleportToTargetPosition();
            yield return new WaitForSeconds(0.1f);
        }
    }
}