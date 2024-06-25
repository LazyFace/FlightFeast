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

public class OrderSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectPooler.ObjectsToSpawn orderToSpawn;
    [SerializeField] private GameObject[] orderToSpawnVisuals;

    private void Start()
    {
        foreach (var orderVisual in orderToSpawnVisuals)
        {
            Order orderComponent = orderVisual.GetComponent<Order>();
            if (orderComponent != null && orderComponent.orderType == orderToSpawn)
            {
                orderVisual.SetActive(true);
                break;
            }
        }
    }

    public void Interact(PickAndUse interactor)
    {
        if(interactor.heldItem == null)
        {
            GameObject orden = ObjectPooler.Instance.SpawnFromPool(orderToSpawn, gameObject.transform.position, gameObject.transform.rotation);
            interactor.PickUpItem(orden);
        }
        else
        {
            if(interactor.heldItem.transform.GetComponent<Order>().orderType == orderToSpawn)
            {
                interactor.DisableHeldItem();
            }
        }
    }
}