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

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    private float startingHeight;

    [Header("Input Configuration")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";

    [Header("Movement Variables")]
    [SerializeField] private float speed = 5f;
    private Vector3 direction;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    private float currentVelocity;
    private bool canMove = false;

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
            ChangeMovementState(true);
        }else if (state == GameState.Paused || state == GameState.GameOver)
        {
            ChangeMovementState(false);
        }
    }

    private void Start()
    {
        startingHeight = transform.position.y;
    }

    private void Update()
    {
        if(canMove)
        {
            MovePlayer();
            if (direction != Vector3.zero)
            {
                RotatePlayer();
            }
        }

        Vector3 position = characterController.transform.position;
        position.y = startingHeight;
        characterController.transform.position = position;
    }

    private Vector2 GetInput()
    {
        float xInput = Input.GetAxisRaw(horizontalInput);
        float zInput = Input.GetAxisRaw(verticalInput);
        return new Vector2(xInput, zInput).normalized;
    }

    private void MovePlayer()
    {
        Vector2 input = GetInput();
        direction = new Vector3(-input.x, 0, -input.y);
        characterController.Move(direction * speed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public Vector3 GetMoveDirection()
    {
        return direction;
    }

    public void ChangeMovementState(bool state)
    {
        canMove = state;
    }
}