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
using UnityEngine.AI;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class PassangerController : Subject
{
    [Header("NavMeshAgent Variables")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Order Management")]
    private OrdersManager orderManager;
    [SerializeField] private GameObject[] ordersVisuals;
    private ObjectPooler.ObjectsToSpawn[] possibleOrders;
    private ObjectPooler.ObjectsToSpawn currentOrder;
    [SerializeField] private float minTimeToOrder = 4f;
    [SerializeField] private float maxTimeToOrder = 12f;
    private float orderTimer;
    [SerializeField] private float orderTimeout = 15f;
    private bool canMakeOrders = false;
    private bool hasOrdered = false;

    [Header("Seat Management")]
    private Seat assignedSeat;
    private Vector3 seatPosition;

    [Header("Animator")]
    [SerializeField] private Animator passengerController;


    private void Start()
    {
        orderManager = OrdersManager.Instance;
    }

    private void Update()
    {
        if (canMakeOrders)
        {
            if (!hasOrdered)
            {
                orderTimer -= Time.deltaTime;
                if (orderTimer <= 0)
                {
                    MakeOrder();
                }
            }
            else
            {
                orderTimeout -= Time.deltaTime;
                if (orderTimeout <= 0)
                {
                    CancelCurrentOrder();
                }
            }
        }
    }

    public void SetTargetPosition(Vector3 destination)
    {
        seatPosition = destination;
        agent.destination = destination;
        //Debug.Log("Setting target position to: " + destination);
        StartCoroutine(CheckAndRotate(destination));
    }

    public void TeleportToTargetPosition()
    {
        //Debug.Log("Teleporting to position: " + seatPosition);
        agent.Warp(new Vector3(seatPosition.x, transform.position.y, seatPosition.z));
    }

    private IEnumerator CheckAndRotate(Vector3 destination)
    {
        while (!IsAgentAtDestination())
        {
            yield return null;
        }

        Vector3 forwardDirection = (destination - agent.transform.position).normalized;
        forwardDirection.y = 0;

        Quaternion finalRotation = Quaternion.LookRotation(forwardDirection);

        while (Quaternion.Angle(agent.transform.rotation, finalRotation) > 0.1f)
        {
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        agent.transform.rotation = finalRotation;
        passengerController.SetBool("isSeated", true);
    }

    public bool IsAgentAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f;
    }

    public void StartMakingOrders()
    {
        ResetOrderTimer();
        canMakeOrders = true;
    }

    public void StopMakingOrders()
    {
        ResetOrderTimer();
        ChangeOrderVisual(currentOrder, false);
        canMakeOrders = false;
        hasOrdered = false;
    }

    private void MakeOrder()
    {
        if (assignedSeat != null && assignedSeat.isOccupied)
        {
            possibleOrders = orderManager.GetPossibleOrders();
            if (orderManager.CanMakeOrder())
            {
                int orderIndex = Random.Range(0, possibleOrders.Length);
                currentOrder = possibleOrders[orderIndex];
                orderManager.CreateOrder(currentOrder);
                ChangeOrderVisual(currentOrder, true);
                orderTimeout = 15f;
                hasOrdered = true;
                //Debug.Log("Order made: " + currentOrder);
            }
            else
            {
                ResetOrderTimer();
                //Debug.Log("Failed to make order: Max orders reached");
            }
        }
        else
        {
            //Debug.Log("No assigned seat or seat is not occupied");
        }
    }

    private void CancelCurrentOrder()
    {
        orderManager.OrderCompletedOrCanceled(currentOrder);
        ChangeOrderVisual(currentOrder, false);
        hasOrdered = false;
        orderTimeout = 0;
        ResetOrderTimer();
        Notify(Actions.OrderLost);
        //Debug.Log("Order cancelled: " + currentOrder);
    }

    public bool OnOrderDelivered(ObjectPooler.ObjectsToSpawn orderReceived)
    {
        if (hasOrdered && orderReceived == currentOrder)
        {
            orderManager.OrderCompletedOrCanceled(currentOrder);
            ChangeOrderVisual(currentOrder, false);
            ResetOrderTimer();
            hasOrdered = false;
            orderTimeout = 0;
            Notify(Actions.CorrectOrderReceived);
            MakeOrderReceivedSound(currentOrder);
            //Debug.Log("Order delivered: " + order);
            return true;
        }
        else
        {
            //Debug.Log("Failed to deliver order: " + order);
            return false;
        }
    }

    public void SetAssignedSeat(Seat seat)
    {
        assignedSeat = seat;
    }

    private void ResetOrderTimer()
    {
        orderTimer = Random.Range(minTimeToOrder, maxTimeToOrder);
        //Debug.Log("Order timer reset to: " + orderTimer);
    }

    private void ChangeOrderVisual(ObjectPooler.ObjectsToSpawn order, bool state)
    {
        foreach (var orderVisual in ordersVisuals)
        {
            Order orderComponent = orderVisual.GetComponent<Order>();
            if (orderComponent != null && orderComponent.orderType == order)
            {
                orderVisual.SetActive(state);
                break;
            }
        }
    }

    private void MakeOrderReceivedSound(ObjectPooler.ObjectsToSpawn order)
    {
        switch(order)
        {
            case ObjectPooler.ObjectsToSpawn.chicken:
                AudioManager.Instance.PlaySFX("eatingchiken");
                break;
            case ObjectPooler.ObjectsToSpawn.soup:
                AudioManager.Instance.PlaySFX("eatingsoup");
                break;
            case ObjectPooler.ObjectsToSpawn.hamburger:
                AudioManager.Instance.PlaySFX("eatinghamburger");
                break;
            case ObjectPooler.ObjectsToSpawn.coffee:
                AudioManager.Instance.PlaySFX("drinkingcoffee");
                break;
        }
    }
}