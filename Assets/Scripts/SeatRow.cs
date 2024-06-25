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
using System.Linq;
using System.Collections.Generic;

public class SeatRow : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Seat> seats;
    private Queue<Seat> availableSeats;

    public void InitializeSeatsQueue()
    {
        availableSeats = new Queue<Seat>(seats.OrderBy(x => Random.value).ToList());
    }

    public bool HasAvailableSeat()
    {
        return availableSeats.Count > 0;
    }

    public void SetSeat(PassangerController passanger)
    {
        if (availableSeats.Count > 0)
        {
            Seat seat = availableSeats.Dequeue();
            if (passanger != null)
            {
                passanger.SetAssignedSeat(seat);
                passanger.SetTargetPosition(seat.GetPosition());
                seat.AssignPassenger(passanger);
            }
        }
    }

    private bool ReceiveOrder(ObjectPooler.ObjectsToSpawn order)
    {
        foreach (Seat seat in seats)
        {
            if (seat.ReceiveOrder(order))
            {
                //Debug.Log("Order delivered to one of the passengers in the row");
                return true;
            }
        }
        //Debug.Log("No passenger in this row has the order");
        return false;
    }

    public void Interact(PickAndUse interactor)
    {
        if(interactor.heldItem != null) 
        {
            if (ReceiveOrder(interactor.heldItem.GetComponent<Order>().orderType))
            {
                interactor.DisableHeldItem();
            }
        }
    }
}