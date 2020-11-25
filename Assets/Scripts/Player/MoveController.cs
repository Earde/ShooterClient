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

    private float saveTime = 1f;
    private float yVelocity = 0f;
    private List<UserInput> inputList = new List<UserInput>();
    private PlayerState serverState;

    private void Start()
    {
        serverState = new PlayerState { _position = transform.position, _rotation = transform.rotation, _time = Time.time, _yVelocity = yVelocity };
        Debug.Log(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Set latest server position
    /// </summary>
    /// <param name="state"></param>
    public void SetLastAcceptedPosition(PlayerState state)
    {
        if (state != null || state._time > serverState._time) serverState = state;
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
            UserInput input = new UserInput { 
                inputs = _inputs, 
                tForward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z), 
                tRight = new Vector3(transform.right.x, transform.right.y, transform.right.z), 
                time = currentTime 
            };
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

        Quaternion curRotation = transform.rotation;

        //Set last accepted server information
        characterController.enabled = false;
        characterController.transform.position = serverState._position;
        characterController.transform.rotation = serverState._rotation;
        yVelocity = serverState._yVelocity;
        characterController.enabled = true;
        characterController.Move(Vector3.zero); //Reset isGrounded

        //Execute all unprocessed inputs
        if (inputList.Count > 0)
        {
            List<UserInput> movesAfterTick = inputList.Where(a => a.time > serverState._time).OrderBy(x => x.time).ToList();
            UserInput movePreTick = inputList.Where(a => a.time <= serverState._time).OrderByDescending(x => x.time).FirstOrDefault();
            if (movePreTick != default && movePreTick != null)
            {
                movePreTick.time = serverState._time;
                movesAfterTick.Insert(0, movePreTick);
            }
            float moveTime = 0.0f;
            if (movesAfterTick.Count > 0)
            {
                for (int i = 0; i < movesAfterTick.Count; i++)
                {
                    if (i == movesAfterTick.Count - 1)
                    {
                        moveTime = currentTime - movesAfterTick[i].time;
                        Move(movesAfterTick[i].inputs, transform.right, transform.forward, moveTime);
                    }
                    else
                    {
                        moveTime = movesAfterTick[i + 1].time - movesAfterTick[i].time;
                        Move(movesAfterTick[i].inputs, movesAfterTick[i].tRight, movesAfterTick[i].tForward, moveTime);
                    }
                }
            }
        }

        transform.rotation = curRotation;

        localPlayerController.AddPlayerState(new PlayerState { _position = transform.position, _rotation = transform.rotation, _time = currentTime, _yVelocity = yVelocity });
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
            _inputDirection *= 0.7071f; //Keer door de wortel van 0.5 = 0.7071 om _inputDirection.magnitude van 1 te krijgen
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
