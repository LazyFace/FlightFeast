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

public class Seat : MonoBehaviour
{
    public bool isOccupied { get; private set; }
    private PassangerController passenger;

    public void AssignPassenger(PassangerController passenger)
    {
        this.passenger = passenger;
        isOccupied = true;
        //Debug.Log("Seat assigned to passenger: " + passenger.name);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool ReceiveOrder(ObjectPooler.ObjectsToSpawn order)
    {
        if (isOccupied)
        {
            return passenger.OnOrderDelivered(order);
        }
        return false;
    }
}