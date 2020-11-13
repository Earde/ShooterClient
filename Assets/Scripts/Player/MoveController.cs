using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public LocalPlayerController localPlayerController;
    public CharacterController characterController;

    private float saveTime = 3f;
    private float yVelocity = 0f;
    private List<UserInput> inputList = new List<UserInput>();
    private PlayerState playerState;

    private void Start()
    {
        playerState = new PlayerState { position = transform.position, time = Time.time, yVelocity = yVelocity };
    }

    public void SetLastAcceptedPosition(PlayerState state)
    {
        if (playerState != null && state.time > playerState.time) playerState = state;
    }

    private void FixedUpdate()
    {
        float currentTime = Time.time;
        SendInputToServer(currentTime);
        if (inputList.Count > saveTime / Time.fixedDeltaTime) inputList.RemoveAt(0);
        ClientPrediction(currentTime);
    }

    private void SendInputToServer(float currentTime)
    {
        if (localPlayerController.IsAlive())
        {
            bool[] _inputs = new bool[]
            {
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.Space)
            };
            inputList.Add(new UserInput { inputs = _inputs, time = currentTime });
            ClientSend.PlayerMovement(inputList.Last().inputs, transform.rotation, inputList.Last().time);
        }
    }

    private void ClientPrediction(float currentTime)
    {
        currentTime += Time.fixedDeltaTime;

        characterController.enabled = false;
        characterController.transform.position = playerState.position;
        characterController.enabled = true;
        yVelocity = playerState.yVelocity;

        if (inputList.Count > 0)
        {
            List<UserInput> previousTickMoves = inputList.Where(a => a.time <= playerState.time).OrderBy(x => x.time).ToList();
            List<UserInput> movesToProcess = inputList.Where(a => a.time > playerState.time).ToList();
            if (movesToProcess.Count > 0)
            {
                float moveTime = movesToProcess[0].time - playerState.time;
                if (previousTickMoves.Count > 0)
                {
                    Move(previousTickMoves.Last().inputs, moveTime);
                }
                for (int i = 0; i < movesToProcess.Count; i++)
                {
                    if (i == movesToProcess.Count - 1)
                    {
                        moveTime = currentTime - movesToProcess[i].time;
                    }
                    else
                    {
                        moveTime = movesToProcess[i + 1].time - movesToProcess[i].time;
                    }
                    Move(movesToProcess[i].inputs, moveTime);
                }
            }
        }

        localPlayerController.SetNewState(new PlayerState { position = transform.position, time = currentTime, yVelocity = yVelocity });
    }

    private void Move(bool[] inputs, float moveDuration)
    {
        if (moveDuration <= 0.0f) return;

        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Vector3 moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        moveDirection *= moveSpeed * moveDuration;

        if (characterController.isGrounded)
        {
            yVelocity = 0;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity -= gravity * moveDuration;
        }
        
        moveDirection.y = yVelocity * moveDuration;
        characterController.Move(moveDirection);
    }
}
