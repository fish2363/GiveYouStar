using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rope : MonoBehaviour
{
    public event Action OnFinishRope;
    public event Action<StarMover> OnCatchStar;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 18f;
    [SerializeField] private float speedByCharge = 10f;
    [SerializeField] private float maxLifeTime = 2.5f;
    [SerializeField] private float maxDistance = 18f;

    [Header("Rope Visual")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private Transform origin;

    private Vector2 launchDir;
    private float speed;
    private Vector2 startPos;
    private float alive;

    public void Launch(Transform originTransform, Vector2 direction, float charge01)
    {
        origin = originTransform;
        launchDir = direction.normalized;
        speed = baseSpeed + speedByCharge * Mathf.Clamp01(charge01);

        rb.position = transform.position;
        rb.linearVelocity = launchDir * speed;

        startPos = rb.position;
        alive = 0f;

        SetupLineIfNeeded();
        UpdateLine();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        SetupLineIfNeeded();
    }

    private void FixedUpdate()
    {
        if (origin == null)
        {
            OnFinishRope?.Invoke();
            Destroy(gameObject);
            return;
        }

        alive += Time.fixedDeltaTime;
        if (alive >= maxLifeTime || Vector2.Distance(startPos, rb.position) >= maxDistance)
        {
            OnFinishRope?.Invoke();
            Destroy(gameObject);
            return;
        }

        // ➤ 로프는 직진만 하도록 (휘는 로직 제거)
        rb.linearVelocity = launchDir * speed;
    }

    private void LateUpdate()
    {
        UpdateLine();
    }

    private void SetupLineIfNeeded()
    {
        if (line != null) return;

        var go = new GameObject("RopeLine");
        go.transform.SetParent(transform, worldPositionStays: true);

        line = go.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
    }

    private void UpdateLine()
    {
        if (line == null || origin == null) return;
        line.SetPosition(0, origin.position);
        line.SetPosition(1, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            var star = collision.GetComponent<StarMover>();
            if (star != null)
            {
                OnCatchStar?.Invoke(star);
                Destroy(gameObject);
            }
        }
    }
}
