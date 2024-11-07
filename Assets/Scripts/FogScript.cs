using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour // Skrypt odpowiadaj¹cy za dzia³anie mg³y
{
    [SerializeField]
    GameObject _fogPlane; // Referencja do plane'a zrobionego z verticli

    [SerializeField]
    GameObject _rayReciver; // Referencja do obiektu który jest drugim koñcem promienia, kamera 

    [SerializeField]
    LayerMask _fogLayer; // Warstwa w której znajduje siê plane

    //[SerializeField]
    //float _fogRadius = 30f; // Promieñ odkrycia mg³y

    public Transform[] markers { set; get; } // Referencja do tablicy punktów w których ma zostaæ odkryta mg³a
    public int[] manyRadius { set; get; } // Referencja do tablicy z promieniami odkrycia dla ka¿dego punktu

    //float _radiusCircle { get { return _fogRadius * _fogRadius; } } // Promieñ odkrycia mg³y

    Mesh _mesh; // Zmienna na siatkê plane
    Vector3[] _vertices; // Tablica na verticle
    Color[] _colors; // Tablica na kolor ka¿dego verticla

    // Start is called before the first frame update
    void Start()
    {
        markers = new Transform[0]; // Trzeba zadeklarowaæ, ¿eby nie by³o puste na pocz¹tku
        initialize(); // Inicjalizowanie zmiennych prywatnych i ustawianie mg³y na ca³oœciowo zakryt¹
    }

    // Update is called once per frame
    void Update()
    {
        initialize(); // Ponowne inicjalizowanie - restartowanie mg³y, przy ka¿dej klatce. Musi siê to wykonywaæ poniewa¿
                      // shadowPlane jest przypiêty do kamery, wiêc siê z ni¹ porusza, natomiast punkty s¹ sztywne na mapie
                      // wiêc przy ka¿dym poruszeniu lub obrocie gracza - kamery, plane bêdzie musia³ odkryæ siê w innym miejscu

        for (int p = 0; p < markers.Length; p++) // Przechodzenie przez tablicê ze wszystkimi markerami
        {
            var marker = markers[p];
            var markerRadiusCircle = manyRadius[p] * manyRadius[p];

            Ray r = new Ray(_rayReciver.transform.position, marker.position - _rayReciver.transform.position); // Tworzenie promienia od pozycji bie¿¹cego obiektu (Kamera) do pozycji markera
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, 1000, _fogLayer, QueryTriggerInteraction.Collide)) // Sprawdzanie czy promieñ trafia w jakikolwiek obiekt w odleg³oœci 1000 jednostek (m)
            {                                                                                  // nale¿¹cy do warstwy FogLayer, jeœli tak hit przechowuje informacje o trafieniu
                for (int i = 0; i < _vertices.Length; i++)
                {
                    Vector3 v = _fogPlane.transform.TransformPoint(_vertices[i]); // Przeliczanie pozycji lokalnej verticla na pozycjê globaln¹
                    float distance = Vector3.SqrMagnitude(v - hit.point); // Odleg³oœæ miêdzy verticlem a punktem w którym promieñ uderzy³ w obiekt

                    if (distance < markerRadiusCircle) // Je¿eli verticl znajduje siê w okreœlonej odleg³oœci od punktu trafienia
                    {
                        float alpha = Mathf.Min(_colors[i].a, distance / markerRadiusCircle); // Pozwala na stopniowe rozjaœnianie mg³y od miejsca trafienia
                        _colors[i].a = alpha; // Ustawiamy color tego verticla na przeŸroczysty
                    }
                }
                updateColors(); // Funkcja ustawia kolory mesh na nowo przypisane kolory
            }
        }
    }

    public void updateColors() // Funkcja ustawia kolory mesh na nowo przypisane kolory
    {
        _mesh.colors = _colors;
    }

    void initialize() // Funkcja inicjalizuj¹ca mg³ê, ustawia wszystkie verticle na wartoœæ pocz¹tkow¹
    {
        _mesh = _fogPlane.GetComponent<MeshFilter>().mesh;
        _vertices = _mesh.vertices;
        _colors = new Color[_vertices.Length];

        for (int i = 0; i < _colors.Length; i++)
        {
            _colors[i] = Color.grey;
        }
        updateColors();
    }
}
