using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    // Odkaz na komponentu Rigidbody
    public Rigidbody rb;

    // Prefab exploze
    public GameObject explosion;

    // Vrstva, ve které se nachází nepřátelé
    public LayerMask whatIsEnemy;

    // Odráživost střely
    [Range(0f, 1f)]
    public float bounciness;

    // Používat gravitaci
    public bool useGravity;

    // Poškození exploze
    public int explosionDamage;

    // Dosah exploze
    public float explosionRange;

    // Síla exploze
    public float explosionForce;

    // Maximální počet kolizí
    public int maxCollisions;

    // Maximální doba života střely
    public float maxLifetime;

    // Explodovat při dotyku
    public bool explodeOnTouch = true;

    // Střela patří hráči
    public bool isPlayerBullet;

    // Počet kolizí
    int collisions;

    // Fyzikální materiál
    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // Explodovat, pokud je překročen maximální počet kolizí
        if (collisions > maxCollisions) Explode();

        // Odečítat čas od doby života
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    // Provede explozi
    private void Explode()
    {
        // Vytvoření exploze
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        // Zjištění koliderů nepřátel v dosahu exploze
        Collider[] enemies =
            Physics
                .OverlapSphere(transform.position, explosionRange, whatIsEnemy);
        for (int i = 0; i < enemies.Length; i++)
        {
            // Zavolání metody pro zabití nepřítele
            enemies[i].GetComponent<EnemyAI>()?.Die();

            // Přidání explozní síly na nepřítele
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i]
                    .GetComponent<Rigidbody>()
                    .AddExplosionForce(explosionForce,
                    transform.position,
                    explosionRange);
        }

        // Zničení střely
        Destroy (gameObject);
    }

    // Zpracování kolize
    private void OnCollisionEnter(Collision collision)
    {
        // Ignorovat kolizi se střelou, pokud patří hráči
        if (isPlayerBullet && collision.collider.CompareTag("Player"))
        {
            return;
        }

        // Způsobit poškození hráči, pokud střela nepatří hráči a koliduje s hráčem
        if (!isPlayerBullet && collision.collider.CompareTag("Player"))
        {
            collision
                .collider
                .GetComponent<PlayerHealth>()?
                .TakeDamage(explosionDamage);
            return;
        }

        // Ignorovat kolizi se střelou
        if (collision.collider.CompareTag("whatIsBullet")) return;

        // Inkrementace počtu kolizí
        collisions++;

        // Explodovat při dotyku s nepřítelem, pokud je zapnutá možnost
        if (collision.collider.CompareTag("whatIsEnemy") && explodeOnTouch)
            Explode();

        // Explodovat
        Explode();
    }

    // Nastavení střely
    private void Setup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics_mat;

        rb.useGravity = useGravity;
    }

    // Přizpůsobení rotace střely směru pohybu
    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = rb.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    // Vykreslení vizuální reprezentace dosahu exploze ve scéně
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
