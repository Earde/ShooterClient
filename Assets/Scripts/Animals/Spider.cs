using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Walking & Jumping spiders walking at constant y to random x,z
/// </summary>
public class Spider : MonoBehaviour
{
    public Animator animator;
    public float speed = 10.0f;
    public float walkDistance = 10.0f;

    private State state;
    private Vector3 target;

    private void Start()
    {
        state = State.Nothing;
    }

    void Update()
    {
        if (state == State.Nothing)
        {
            animator.SetFloat("Speed", 0.0f);
            animator.SetBool("Jumping", false);
            RandomAction();
        } 
        else if (state == State.Walking)
        {
            float move = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, move);
            transform.LookAt(target);
            //transform.Rotate(new Vector3(0.0f, 180f));
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                state = State.Nothing;
            }
            animator.SetFloat("Speed", 1.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        state = State.Nothing;
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
        state = State.Nothing;
    }

    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
        state = State.Nothing;
    }

    private void RandomAction()
    {
        int rand = Random.Range(0, 100);
        if (rand < 40)
        {
            state = State.Idle;
            StartCoroutine(Idle());
        } else if (rand < 80)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * walkDistance;
            target = transform.position + new Vector3(circle.x, 0.0f, circle.y);
            state = State.Walking;
        }
        else if (rand < 100)
        {
            state = State.Jumping;
            animator.SetBool("Jumping", true);
            StartCoroutine(Jump());
        }
    }

    public void Die()
    {
        state = State.Die;
        animator.SetFloat("Speed", 0.0f);
        animator.SetBool("Jumping", false);
        animator.SetBool("Die", true);
    }

    enum State
    {
        Nothing = 0,
        Idle,
        Walking,
        Jumping,
        Die
    }
}
