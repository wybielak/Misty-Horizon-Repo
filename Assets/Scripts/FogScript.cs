using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour // Skrypt odpowiadaj�cy za dzia�anie mg�y
{
    [SerializeField]
    GameObject _fogPlane; // Referencja do plane'a zrobionego z verticli

    [SerializeField]
    GameObject _rayReciver; // Referencja do obiektu kt�ry jest drugim ko�cem promienia, kamera 

    [SerializeField]
    LayerMask _fogLayer; // Warstwa w kt�rej znajduje si� plane

    //[SerializeField]
    //float _fogRadius = 30f; // Promie� odkrycia mg�y

    public Transform[] markers { set; get; } // Referencja do tablicy punkt�w w kt�rych ma zosta� odkryta mg�a
    public int[] manyRadius { set; get; } // Referencja do tablicy z promieniami odkrycia dla ka�dego punktu

    //float _radiusCircle { get { return _fogRadius * _fogRadius; } } // Promie� odkrycia mg�y

    Mesh _mesh; // Zmienna na siatk� plane
    Vector3[] _vertices; // Tablica na verticle
    Color[] _colors; // Tablica na kolor ka�dego verticla

    // Start is called before the first frame update
    void Start()
    {
        markers = new Transform[0]; // Trzeba zadeklarowa�, �eby nie by�o puste na pocz�tku
        initialize(); // Inicjalizowanie zmiennych prywatnych i ustawianie mg�y na ca�o�ciowo zakryt�
    }

    // Update is called once per frame
    void Update()
    {
        initialize(); // Ponowne inicjalizowanie - restartowanie mg�y, przy ka�dej klatce. Musi si� to wykonywa� poniewa�
                      // shadowPlane jest przypi�ty do kamery, wi�c si� z ni� porusza, natomiast punkty s� sztywne na mapie
                      // wi�c przy ka�dym poruszeniu lub obrocie gracza - kamery, plane b�dzie musia� odkry� si� w innym miejscu

        for (int p = 0; p < markers.Length; p++) // Przechodzenie przez tablic� ze wszystkimi markerami
        {
            var marker = markers[p];
            var markerRadiusCircle = manyRadius[p] * manyRadius[p];

            Ray r = new Ray(_rayReciver.transform.position, marker.position - _rayReciver.transform.position); // Tworzenie promienia od pozycji bie��cego obiektu (Kamera) do pozycji markera
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, 1000, _fogLayer, QueryTriggerInteraction.Collide)) // Sprawdzanie czy promie� trafia w jakikolwiek obiekt w odleg�o�ci 1000 jednostek (m)
            {                                                                                  // nale��cy do warstwy FogLayer, je�li tak hit przechowuje informacje o trafieniu
                for (int i = 0; i < _vertices.Length; i++)
                {
                    Vector3 v = _fogPlane.transform.TransformPoint(_vertices[i]); // Przeliczanie pozycji lokalnej verticla na pozycj� globaln�
                    float distance = Vector3.SqrMagnitude(v - hit.point); // Odleg�o�� mi�dzy verticlem a punktem w kt�rym promie� uderzy� w obiekt

                    if (distance < markerRadiusCircle) // Je�eli verticl znajduje si� w okre�lonej odleg�o�ci od punktu trafienia
                    {
                        float alpha = Mathf.Min(_colors[i].a, distance / markerRadiusCircle); // Pozwala na stopniowe rozja�nianie mg�y od miejsca trafienia
                        _colors[i].a = alpha; // Ustawiamy color tego verticla na prze�roczysty
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

    void initialize() // Funkcja inicjalizuj�ca mg��, ustawia wszystkie verticle na warto�� pocz�tkow�
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
