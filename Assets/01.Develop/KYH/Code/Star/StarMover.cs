using UnityEngine;

public class StarMover : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 moveDirection;
    public bool isCatch;

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        if (isCatch) return;
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetStop(Transform parent)
    {
        isCatch = true;
        transform.SetParent(parent);
    }
}
