using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float gravity = 9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public LocalPlayerController localPlayerController;
    public CharacterController characterController;

    private float saveTime = 3f;
    private float yVelocity = 0f;
    private List<UserInput> inputList = new List<UserInput>();
    private PlayerState serverState;

    private void Start()
    {
        serverState = new PlayerState { position = transform.position, rotation = transform.rotation, time = Time.time, yVelocity = yVelocity };
    }

    /// <summary>
    /// Set latest server position
    /// </summary>
    /// <param name="state"></param>
    public void SetLastAcceptedPosition(PlayerState state)
    {
        if (state != null && state.time > serverState.time) serverState = state;
    }

    private void FixedUpdate()
    {
        float currentTime = Time.time;
        SendInputToServer(currentTime);
        if (inputList.Count > saveTime / Time.fixedDeltaTime) inputList.RemoveAt(0);
        ClientPrediction(currentTime);
    }

    /// <summary>
    /// Get and send user input to server
    /// </summary>
    /// <param name="currentTime"></param>
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
            UserInput input = new UserInput { inputs = _inputs, tForward = transform.forward, tRight = transform.right, time = currentTime };
            inputList.Add(input);
            ClientSend.PlayerMovement(input.inputs, transform.rotation, input.time);
        }
    }

    /// <summary>
    /// Set latest interpolation position (latest server position + client prediction)
    /// </summary>
    /// <param name="currentTime"></param>
    private void ClientPrediction(float currentTime)
    {
        currentTime += Time.fixedDeltaTime;

        //Set last accepted server position
        characterController.enabled = false;
        characterController.transform.position = serverState.position;
        yVelocity = serverState.yVelocity;
        characterController.enabled = true;

        //Execute all unprocessed inputs
        if (inputList.Count > 0)
        {
            List<UserInput> movesToProcess = inputList.Where(a => a.time >= serverState.time).OrderBy(x => x.time).ToList();
            //Finish move before playerState
            float moveTime = movesToProcess.Count > 0 ? movesToProcess[0].time : currentTime;
            moveTime -= serverState.time;
            UserInput previousTick = inputList.Where(a => a.time < serverState.time).OrderByDescending(x => x.time).FirstOrDefault();
            if (previousTick != default && previousTick != null)
            {
                Move(previousTick.inputs, previousTick.tRight, previousTick.tForward, moveTime);
            }
            //Do moves after playerState
            if (movesToProcess.Count > 0)
            {
                for (int i = 0; i < movesToProcess.Count; i++)
                {
                    if (i == movesToProcess.Count - 1)
                    {
                        moveTime = currentTime - movesToProcess[i].time;
                        Move(movesToProcess[i].inputs, transform.right, transform.forward, moveTime);
                    }
                    else
                    {
                        moveTime = movesToProcess[i + 1].time - movesToProcess[i].time;
                        Move(movesToProcess[i].inputs, movesToProcess[i].tRight, movesToProcess[i].tForward, moveTime);
                    }
                }
            }
        }

        localPlayerController.AddPlayerState(new PlayerState { position = transform.position, rotation = transform.rotation, time = currentTime, yVelocity = yVelocity });
    }

    /// <summary>
    /// Apply user input for duration x to Character Controller
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="moveDuration"></param>
    private void Move(bool[] inputs, Vector3 tRight, Vector3 tForward, float moveDuration)
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
        if (_inputDirection.magnitude > 1.5f)
        {
            _inputDirection *= 0.7071f; //Keer wortel van 0.5 om _inputDirection.magnitude van 1 te krijgen
        }

        Vector3 moveDirection = (tRight * _inputDirection.x * moveSpeed) + (tForward * _inputDirection.y * moveSpeed);

        if (characterController.isGrounded)
        {
            yVelocity = 0;
            if (inputs[4]) { yVelocity = jumpSpeed; }
        }

        moveDirection.y = yVelocity;

        if (!characterController.isGrounded)
        {
            yVelocity -= gravity * moveDuration;
        }

        characterController.Move(moveDirection * moveDuration);
    }
}
