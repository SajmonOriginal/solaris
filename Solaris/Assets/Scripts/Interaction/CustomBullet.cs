using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemy;

    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    public bool isPlayerBullet;

    int collisions;
    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if (collisions > maxCollisions) Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemy);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyAI>()?.Die();

            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isPlayerBullet && collision.collider.CompareTag("Player"))
        {
            return;
        }

        if (!isPlayerBullet && collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
            return;
        }

        if (collision.collider.CompareTag("whatIsBullet")) return;

        collisions++;

        if (collision.collider.CompareTag("whatIsEnemy") && explodeOnTouch) Explode();


        Explode();

    }

    private void Setup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics_mat;

        rb.useGravity = useGravity;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = rb.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
