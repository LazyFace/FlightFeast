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

public class PickAndUse : MonoBehaviour
{
    public GameObject heldItem { get; private set; }
    private bool canInteract = false;

    [Header("Ray Variables")]
    [SerializeField] private Transform rayTransform;
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private LayerMask layerMask;

    [Header("Hands")]
    [SerializeField] private Transform handsTransform;

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
        if (state == GameState.PassangersSeated || state == GameState.Continue)
        {
            ChangeInteractionState(true);
        }
        else if (state == GameState.Paused || state == GameState.GameOver)
        {
            ChangeInteractionState(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            Collider[] hitColliders = Physics.OverlapBox(rayTransform.position, boxSize / 2, rayTransform.rotation, layerMask);
            bool interactableFound = false;

            foreach (Collider hitCollider in hitColliders)
            {
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    interactableFound = true;
                    break;
                }
            }

            if (!interactableFound)
            {
                //Debug.Log("No IInteractable component found on any of the hit objects.");
            }
        }
    }

    public void PickUpItem(GameObject item)
    {
        if (heldItem == null)
        {
            heldItem = item;
            heldItem.transform.SetParent(handsTransform);
            heldItem.transform.position = handsTransform.position;
            heldItem.GetComponent<Collider>().enabled = false;
        }
    }

    public void DisableHeldItem()
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Collider>().enabled = true;
            heldItem.SetActive(false);
            heldItem = null;
            //Debug.Log("ItemUsed");
        }
    }

    public bool HasItem()
    {
        if (heldItem)
        {
            return true;
        }
        return false;
    }

    public void ChangeInteractionState(bool state)
    {
        canInteract = state;
    }

    private void OnDrawGizmos()
    {
        if (rayTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rayTransform.position, boxSize);
        }
    }
}