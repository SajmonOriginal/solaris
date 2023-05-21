using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent; // Reference na komponentu NavMeshAgent

    public Transform player; // Reference na transformaci hráče

    public LayerMask

            whatIsGround,
            whatIsPlayer,
            whatIsWall; // Definice vrstev pro raycasty

    public float walkPointRange; // Dosah náhodných bodů pro procházení

    private Vector3 walkPoint; // Aktuální náhodný bod

    private bool walkPointSet; // Stav, zda je nastaven náhodný bod pro procházení

    public float timeBetweenAttacks; // Čas mezi útoky

    private bool alreadyAttacked; // Stav, zda již byl proveden útok

    public float

            sightRange,
            attackRange; // Dosahy pro viditelnost hráče a útok

    private bool

            playerInSightRange,
            playerInAttackRange; // Stav, zda je hráč ve viditelnosti a útočném dosahu

    public GameObject projectile; // Prefab projektilu

    public Transform attackPoint; // Pozice pro vypuštění projektilu

    public AudioSource shootAudioSource; // Zvukový zdroj pro střelbu

    public Material

            green,
            yellow,
            red; // Materiály pro různé stavy nepřítele

    private MeshRenderer meshRenderer; // Renderer pro zobrazení meshe

    public GameObject EyeR; // Reference na pravé oko

    public GameObject EyeL; // Reference na levé oko

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform; // Nalezení transformace hráče
        agent = GetComponent<NavMeshAgent>(); // Získání komponenty NavMeshAgent
        meshRenderer = GetComponent<MeshRenderer>(); // Získání komponenty MeshRenderer
    }

    private void Update()
    {
        // Kontrola dosahu viditelnosti a útoku hráče
        playerInSightRange =
            Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange =
            Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange)
        {
            // Kontrola, zda je přímý pohled na hráče blokován stěnou
            if (
                Physics
                    .Linecast(transform.position, player.position, whatIsWall)
            )
            {
                playerInSightRange = false;
            }
        }

        if (playerInAttackRange)
        {
            // Kontrola, zda je přímý pohled na hráče blokován stěnou
            if (
                Physics
                    .Linecast(transform.position, player.position, whatIsWall)
            )
            {
                playerInAttackRange = false;
            }
        }

        // Rozhodování o akci na základě dosahu hráče
        if (!playerInAttackRange && !playerInSightRange)
        {
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint(); // Hledání náhodného bodu pro procházení
        }
        else
        {
            agent.SetDestination (walkPoint); // Nastavení cílové pozice pro procházení

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                walkPointSet = false; // Resetování stavu náhodného bodu, pokud byl dosažen
            }
        }

        meshRenderer.material = green; // Nastavení zeleného materiálu
    }

    private void SearchWalkPoint()
    {
        Vector3 randomPoint =
            Random.insideUnitSphere * walkPointRange + transform.position; // Náhodný bod v určeném dosahu okolo nepřítele
        NavMeshHit hit;

        if (
            NavMesh
                .SamplePosition(randomPoint,
                out hit,
                walkPointRange,
                NavMesh.AllAreas)
        )
        {
            walkPoint = hit.position; // Nastavení náhodného bodu
            walkPointSet = true; // Nastavení stavu náhodného bodu
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position); // Nastavení cílové pozice pro pronásledování hráče
        meshRenderer.material = yellow; // Nastavení žlutého materiálu
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position); // Zastavení pohybu
        transform.LookAt (player); // Otočení směrem k hráči

        if (!alreadyAttacked)
        {
            GameObject bullet =
                Instantiate(projectile,
                attackPoint.position,
                Quaternion.identity); // Vytvoření projektilu
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse); // Přidání síly projektilu

            shootAudioSource.Play(); // Přehrání zvuku střelby

            CustomBullet customBullet = bullet.GetComponent<CustomBullet>(); // Získání skriptu CustomBullet z projektilu
            if (customBullet != null)
            {
                customBullet.isPlayerBullet = false; // Nastavení proměnné isPlayerBullet na false
            }

            alreadyAttacked = true; // Nastavení stavu útoku na true
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // Spuštění funkce ResetAttack po časovém prodlevě
        }

        meshRenderer.material = red; // Nastavení červeného materiálu
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; // Resetování stavu útoku
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Vykreslení kruhu pro útočný dosah
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); // Vykreslení kruhu pro dosah viditelnosti
    }

    public void Die()
    {
        Rigidbody eyeRRb = EyeR.AddComponent<Rigidbody>(); // Přidání komponenty Rigidbody na pravé oko
        EyeR.transform.parent = null; // Odpojení pravého oka od rodiče
        eyeRRb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic; // Nastavení detekce kolizí pro pravé oko

        Rigidbody eyeLRb = EyeL.AddComponent<Rigidbody>(); // Přidání komponenty Rigidbody na levé oko
        EyeL.transform.parent = null; // Odpojení levého oka od rodiče
        eyeLRb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic; // Nastavení detekce kolizí pro levé oko

        Destroy (gameObject); // Zničení objektu nepřítele
    }
}
