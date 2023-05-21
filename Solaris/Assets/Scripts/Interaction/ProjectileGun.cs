using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    // Střela
    public GameObject bullet;

    // Síla střely
    public float shootForce, upwardForce;

    // Statistiky zbraně
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    // Zvuky
    public AudioSource shootAudioSource;
    public AudioSource reloadAudioSource;

    int bulletsLeft, bulletsShot;

    // Recoil
    public Rigidbody playerRb;
    public float recoilForce;

    // Boole
    bool shooting, readyToShoot, reloading;

    // Reference
    public Camera fpsCam;
    public Transform attackPoint;

    // Grafika
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    
    public bool allowInvoke = true;

    private void Awake()
    {
        // Zajištění plného zásobníku
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        // Nastavení zobrazení munice, pokud existuje :D
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // Kontrola, zda je povolené podržení tlačítka a příslušný vstup
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Nabíjení
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        // Automatické nabíjení při pokusu o střelbu bez munice
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        // Střelba
        if (!DragRigidbody.isDragging && readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            // Nastavení počtu vystřelených střel na 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Nalezení středu obrazovky
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0);
        Ray ray = fpsCam.ViewportPointToRay(screenCenter);

        // Vytvoření instance střely
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        // Výpočet směru od attackPoint ke středu obrazovky
        Vector3 direction = ray.direction;

        // Přidání síly k střele
        currentBullet.GetComponent<Rigidbody>().AddForce(direction * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        // Vytvoření efektu záblesku hlavně, pokud existuje
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        shootAudioSource.Play();
        bulletsLeft--;
        bulletsShot++;

        // Zavolání funkce ResetShot (pokud ještě nebyla zavolána), s časem timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            // Přidání zpětného rázu na hráče (mělo by být zavoláno pouze jednou)
            playerRb.AddForce(-direction.normalized * recoilForce, ForceMode.Impulse);
        }

        // Pokud je více než jedna střela na jedno stisknutí tlačítka, zopakujte funkci Shoot
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        // Povolení střelby a znovu zavolání Invoke
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime); // Zavolání funkce ReloadFinished po uplynutí doby reloadTime
    }

    private void ReloadFinished()
    {
        // Naplnění zásobníku
        bulletsLeft = magazineSize;
        reloadAudioSource.Play();
        reloading = false;
    }
}
