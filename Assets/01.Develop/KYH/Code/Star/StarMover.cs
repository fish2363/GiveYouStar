using _01.Develop.LSW._01._Scripts.So;
using System;
using UnityEngine;

public class StarMover : MonoBehaviour
{
    float speed = 2f;

    private Vector3 moveDirection;
    public bool isCatch;
    public Action OnDestroy;
    [SerializeField] private ParticleSystem particle;
    public StarSo MyInfo { get; private set; }

    public void Initialize(StarSo star)
    {
        MyInfo = star;
        speed = star.speed;
    }

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
        particle.Play();
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
