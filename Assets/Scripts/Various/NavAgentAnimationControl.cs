﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentAnimationControl : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        animator = GetComponent<Animator>();
    }

    public void Animate()
    {
        if (agent.velocity.magnitude > 0)
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);


        if (agent.velocity != Vector3.zero)
            gameObject.transform.forward = agent.velocity;
    }

    public void LookTowards(Vector3 target)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        gameObject.transform.LookAt(target);
        agent.isStopped = false;
    }
}
