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

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public PlayerMovementController movementController;
    public PickAndUse pickUpItem;

    private void Update()
    {
        Vector3 moveDirection = movementController.GetMoveDirection();
        bool isMoving = moveDirection.magnitude >= 0.1f;

        animator.SetBool("isMoving", isMoving);

        if(pickUpItem != null)
        {
            animator.SetBool("hasAnOrder", pickUpItem.HasItem());
        }
    }
}
