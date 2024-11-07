namespace Mapbox.Examples
{
    using UnityEngine;
    using Mapbox.Utils;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.MeshGeneration.Factories;
    using Mapbox.Unity.Utilities;
    using System.Collections.Generic;
    using GeoCoordinatePortable;
    using System;
    using Mapbox.Unity.Location;
    using UnityEngine.UIElements;
    using TMPro;

    public class SpawnOnMapMarker : MonoBehaviour // Klasa utworzona na podstawie klasy domyślej z mapbox SpawnOnMap
    {                                             // Zawiera modyfikacje renderujące punkty tylko wokół gracza

        AbstractLocationProvider _locationProvider = null; // Zmienna na aktualnego locationProvidera
        Location _currLoc; // Zmienna do przekazywania aktualnej lokalizacji

        [SerializeField]
        AbstractMap _map;

        Vector2d[] _locations; // Lista wszystkich punktów
        Vector2d[] _locationsCurr; // Lista obecnie wyrenderowanych punktów

        long[] _locationsTimestamps; // Lista timestampów dla wszystkich punktów
        long[] _locationsTimestampsCurr; // Lista timestampów dla wyrenderowanych punktów

        int[] _locationsRanges; // Lista promieni dla wszystkich punktów
        int[] _locationsRangesCurr; // Lista promieni dla wyrenderowanych punktów

        [SerializeField]
        float _spawnScale = 100f;

        [SerializeField]
        GameObject _markerPrefab;

        List<GameObject> _spawnedObjects; // Lista obiektów punktów widocznych na mapie

        LocationStatus _playerLocation; // Lokalizacja gracza
        GeoCoordinate _playerLocationGeo;

        [SerializeField]
        int _screenRenderRange = 500; // Zakres widoczności punktów

        [SerializeField]
        int _pointRadius = 50; // Odległość między punktami

        [SerializeField]
        GameObject _fogManager;

        FogScript _fogScript;

        // Wyświetlanie wartości do debugowania
        [SerializeField]
        TMP_Text _currLocVal;

        [SerializeField]
        TMP_Text _allLocVal;

        [SerializeField]
        TMP_Text _lastTimeVal;

        [SerializeField]
        TMP_Text _lastRangeVal;
        // Koniec

        void Start()
        {
            _fogScript = _fogManager.GetComponent<FogScript>(); //GameObject.Find("Main Camera").GetComponent<ShadowScript>(); // Ustawianie referencji do skryptu mgły, aby przekazać listę punktów do odkrycia

            if (null == _locationProvider) // Ustawianie locationProvidera na ten aktualnie wybrany
            {
                _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
            }

            _locations = new Vector2d[0];
            _locationsCurr = new Vector2d[_locations.Length];

            _locationsTimestamps = new long[0];
            _locationsTimestampsCurr = new long[_locationsTimestamps.Length];

            _locationsRanges = new int[0];
            _locationsRangesCurr = new int[_locationsRanges.Length];

            _spawnedObjects = new List<GameObject>();
        }

        private void Update()
        {
            _currLoc = _locationProvider.CurrentLocation; // Pobieranie aktualnej lokalizacji z providera
            _playerLocationGeo = new GeoCoordinate(_currLoc.LatitudeLongitude.x, _currLoc.LatitudeLongitude.y); // Konwersja na GeoCoordinate
            
            AddPlayerLocation(); // Sprawdzanie czy można dodać nową lokalizację do odwiedzonych

            DestroyAllClones(); // Usuwanie wszystkich markerów z mapy
            CheckRenderZone(); // Obliczanie które lokalizacje znajdują się wokół gracza i z których można utworzyć markery
            RenderMarkersOnMap(); // Dodawanie markerów do mapy
            initFog(); // Przekazywanie lokalizacji markerów do skryptu mgły

            _currLocVal.text = _locationsCurr.Length.ToString(); // Wyświetlanie ilości punktów do debugowania
            _allLocVal.text = _locations.Length.ToString();

            int count = _spawnedObjects.Count; // Przeliczanie współrzędnych geo na lokalizację na ekranie gry, co klatkę
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locationsCurr[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }

        public void AddPlayerLocation() // Funkcja ustawiająca markery lokalizacji za graczem w określonej odległości
        {
            var newMarkerFlag = true; // Flaga informująca czy można utworzyć nowy punkt

            var location = Conversions.StringToLatLon(_playerLocationGeo.ToString());

            int count = _locations.Length;
            for (int i = 0; i < count; i++) // Pętla przechodząca przez wszystkie punkty i sprawdzająca odległość każdego z nich od gracza
            {
                var tmpLoc = _locations[i];
                var tmpLocGeo = new GeoCoordinate(tmpLoc[0], tmpLoc[1]);

                if (tmpLocGeo.GetDistanceTo(_playerLocationGeo) < _pointRadius) // Warunek sprawdzający czy można utworzyć nowy punkt
                {
                    newMarkerFlag = false; // Jeśli jest zbyt blisko do jakiegoś innego punktu to nie można
                }
            }

            if (_playerLocationGeo == new GeoCoordinate(0,0)) // Zanim wczyta się pierwsza lokalizacja gracz jest w punkcie 0.0000, 0.0000, nie chcemy żeby on się dodawał
            {
                newMarkerFlag = false;
            }

            if (newMarkerFlag) // Jeśli można dodać nową lokalizację
            {
                long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Pobieramy aktualny czas

                _locationsTimestamps = AddToArray(_locationsTimestamps, unixTimestamp); // Dodajemy go do listy czasów

                if (_locationsTimestamps.Length > 1) // Jeśli mamy więcej niż jeden czas, można przybliżyć prędkość
                { 
                    long velocityTime = _locationsTimestamps[_locationsTimestamps.Length - 1] - _locationsTimestamps[_locationsTimestamps.Length - 2]; // Czas przebycia 50m

                    if (velocityTime > 10) // Jeżeli potrzebny czas na przejście od pkt 1 do pkt 2 jest większy niż 10sec, większy zasięg odkrycia
                    {
                        _locationsRanges = AddToArray(_locationsRanges, 40);
                    }
                    else if (velocityTime <= 10 && velocityTime > 2) // Jeżeli potrzebny czas na przejście od pkt 1 do pkt 2 jest mniejszy niż 10sec ale wiek. 2s, mniejszy zasięg odkrycia
                    {
                        _locationsRanges = AddToArray(_locationsRanges, 25);
                    }
                    else // Jeżeli potrzebny czas na przejście od pkt 1 do pkt 2 jest mniejszy niż 2sec, najmniejszy zasięg odkrycia
                    {
                        _locationsRanges = AddToArray(_locationsRanges, 15);
                    }

                } 
                else
                {
                    _locationsRanges = AddToArray(_locationsRanges, 40);
                }

                _locations = AddToArray(_locations, location); // Rozszerzanie tablic istniejących punktów o nowo utworzony

                _lastTimeVal.text = _locationsTimestamps[_locationsTimestamps.Length - 1].ToString();
                _lastRangeVal.text = _locationsRanges[_locationsRanges.Length - 1].ToString();
            }
        }

        private void DestroyAllClones() // Funkcja usuwająca markery z mapy
        {
            foreach (GameObject clone in _spawnedObjects)
            {
                Destroy(clone);
            }
            _spawnedObjects = new List<GameObject>();
            _locationsCurr = new Vector2d[0];
            _locationsTimestampsCurr = new long[0];
            _locationsRangesCurr = new int[0];
        }

        private void CheckRenderZone() // Funkcja sprawdzająca które punkty są blisko gracza i które można dodać
        {
            for (int i = 0; i < _locations.Length; i++)
            {
                var tmpLoc = _locations[i];
                var geoLoc = new GeoCoordinate(tmpLoc[0], tmpLoc[1]);
                if (geoLoc.GetDistanceTo(_playerLocationGeo) < _screenRenderRange)
                {
                    _locationsCurr = AddToArray(_locationsCurr, _locations[i]);
                    _locationsTimestampsCurr = AddToArray(_locationsTimestampsCurr, _locationsTimestamps[i]);
                    _locationsRangesCurr = AddToArray(_locationsRangesCurr, _locationsRanges[i]);
                }
            }
        }

        private void RenderMarkersOnMap() // Funkcja renderująca na mapie punkty w zasięgu renderowania (ekranu) 
        {
            for (int i = 0; i < _locationsCurr.Length; i++)
            {
                var instance = Instantiate(_markerPrefab); // Tworzenie nowego obiektu punktu - markera

                instance.transform.localPosition = _map.GeoToWorldPosition(_locationsCurr[i], true); // Ustawianie lokalizacji obiektu na lokalizację punktu
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);  // przeliczoną na pozycję na ekranie
                _spawnedObjects.Add(instance);
            }
        }

        private void initFog() // Funkcja przekazująca markery z mapy do mgły
        {
            Transform[] tmpArr = new Transform[_spawnedObjects.Count]; // Pomocnicza tablica, przechowująca transform z obiektów punktów

            for (int j = 0; j < _spawnedObjects.Count; j++)
            {
                tmpArr[j] = _spawnedObjects[j].transform;
            }

            _fogScript.markers = tmpArr; // Ustawianie punktów odkrycia mgły na punkty w których był gracz
            _fogScript.manyRadius = _locationsRangesCurr;
        }

        private T[] AddToArray<T>(T[] array, T newItem) // Append do tablicy
        {
            T[] newArray = new T[array.Length + 1];

            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            newArray[array.Length] = newItem;

            return newArray;
        }

        public static Vector2d[] RemoveElementAt(Vector2d[] array, int index) // Usuwanie elementu z konkertnego indexu
        {
            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            Vector2d[] newArray = new Vector2d[array.Length - 1];

            for (int i = 0; i < index; i++)
            {
                newArray[i] = array[i];
            }

            for (int i = index + 1; i < array.Length; i++)
            {
                newArray[i - 1] = array[i];
            }

            return newArray;
        }

        public int getAllPlayerMarkerCount()
        {
            return _locations.Length;
        }

        public int getCurrPlayerMarkerCount()
        {
            return _spawnedObjects.Count;
        }
    }
}