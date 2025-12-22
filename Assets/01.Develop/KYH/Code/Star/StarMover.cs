using System;
using UnityEngine;

public class StarMover : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 moveDirection;
    public bool isCatch;
    public Action OnDestroy;

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        if (isCatch) return;
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetStop(bool isStop)
    {
        isCatch = isStop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Star") && isCatch)
        {
            OnDestroy?.Invoke();
        }
    }
}
