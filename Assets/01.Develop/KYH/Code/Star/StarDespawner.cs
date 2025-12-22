using UnityEngine;

public class StarDespawner : MonoBehaviour
{
    public StarSpawner spawner; // 스포너 참조

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            Debug.Log("별이 감지됨: " + collision.name);
            Destroy(collision.gameObject);

            if (spawner != null)
            {
                spawner.SpawnStars(1);
            }
        }
    }

}
