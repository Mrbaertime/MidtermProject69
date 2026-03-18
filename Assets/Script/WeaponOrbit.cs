using UnityEngine;

public class WeaponOrbit : MonoBehaviour
{
    public Transform player;
    public float radius = 1.5f;
    public float speed = 180f;
    public int damage = 1;

    private float angle;

    void Update()
    {
        angle += speed * Time.deltaTime;

        float rad = angle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

        transform.position = (Vector2)player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ เช็ค Tag ก่อน
        if (collision.CompareTag("Enemy"))
        {
            IDamageable dmg = collision.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }
}
