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
using System.Collections.Generic;

public class OrdersManager : MonoBehaviour
{
    public static OrdersManager Instance;

    [SerializeField] private int maxActiveOrders = 5;
    private List<ObjectPooler.ObjectsToSpawn> activeOrders = new List<ObjectPooler.ObjectsToSpawn>();

    [SerializeField] private ObjectPooler.ObjectsToSpawn[] possibleOrders;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanMakeOrder()
    {
        //Debug.Log("CanMakeOrder: " + (activeOrders.Count < maxActiveOrders));
        return activeOrders.Count < maxActiveOrders;
    }

    public void CreateOrder(ObjectPooler.ObjectsToSpawn order)
    {
        activeOrders.Add(order);
    }

    public void OrderCompletedOrCanceled(ObjectPooler.ObjectsToSpawn order)
    {
        //Debug.Log("Order completed or canceled: " + order);
        activeOrders.Remove(order);
    }

    public ObjectPooler.ObjectsToSpawn[] GetPossibleOrders()
    {
        return (possibleOrders != null && possibleOrders.Length > 0) ? possibleOrders : null;
    }
}