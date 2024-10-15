using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : MonoBehaviour
{
    public GameObject alert;  // Reference to the alert GameObject


    public void AlartOn()
    {
        alert.SetActive(true);
    }
    public void AlartOff()
    {
        alert.SetActive(false);
    }
}
