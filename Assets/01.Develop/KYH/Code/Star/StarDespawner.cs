using UnityEngine;

public class StarDespawner : MonoBehaviour
{
    public StarSpawner spawner; // 스포너 참조


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Star") && !collision.gameObject.GetComponent<StarMover>().isCatch)
        {
            Debug.Log("별이 감지됨: " + collision.gameObject.name);
            Destroy(collision.gameObject);

            if (spawner != null)
            {
                spawner.SpawnStars(1);
            }
        }
    }

}
