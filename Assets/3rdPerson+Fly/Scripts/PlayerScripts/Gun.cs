using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Gun : MonoBehaviour
{
    public int damage = 10;                         
    public float range = 100f;                   
    public float fireRate = 15f;              
    public float impactForce = 30f;                   
    public int maxAmmo = 10;                         
    private int currentAmmo;                           
    public float reloadTime = 1f;                      
    private bool isReloading = false;                  

    public Camera fpsCam;                               
    public ParticleSystem muzzleFlash;                 
    public Transform FirePoint;
    public GameObject Bullet;
    private float nextTimeToFire = 0f;                

    void Start()
    {
        currentAmmo = maxAmmo;                       
    }

    void OnEnable()
    {
        isReloading = false;                          
    }

    void Update()
    {
        if (isReloading)
            return;

      
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

  
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }


    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        currentAmmo--;

        Instantiate(Bullet, FirePoint.position, FirePoint.rotation);
    }
}
