using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainShip : MonoBehaviour
{
    public static int PowerDestroyed;
    public static int BuildingDestroyed;
    public GameObject DestroyFx;

    public TextMeshProUGUI buildingDestroyedText;

    void Update()
    {
        if (PowerDestroyed == 4)
        {
            GameOver gameOverScript = FindObjectOfType<GameOver>();
            gameOverScript.Win();
            Instantiate(DestroyFx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (BuildingDestroyed >= 5)
        {
            GameOver gameOverScript = FindObjectOfType<GameOver>();
            gameOverScript.Game_OverFor5();
        }

        buildingDestroyedText.text = "BUILDING DESTROYED: " + BuildingDestroyed.ToString() + "/" + 5.ToString();    
    }
}
