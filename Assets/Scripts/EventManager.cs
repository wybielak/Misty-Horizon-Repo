using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour // Klasa utworzona, �eby prze��cza� scen� z startowej, na g��wn�
{                                         // Scena startowa prosi o pozwolenie i potem mapa ju� �adnie dzia�a
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openMainGameScene() // Przelaczanie na glowna gre - mape
    {
        SceneManager.LoadScene("MainGame");
    }

}
