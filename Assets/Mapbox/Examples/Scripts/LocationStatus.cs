namespace Mapbox.Examples
{
	using Mapbox.Unity.Location;
	using Mapbox.Utils;
	using System.Collections;
	using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
	using UnityEngine.UI;

	public class LocationStatus : MonoBehaviour // Dodane modyfikacje umożliwiające pobieranie lokalizacji,
    {                                           // już nie potrzebne lokalizacja pobierana bezpośrednio z providera w innych klasach
        [SerializeField]
        TMP_Text _statusTextMP;

        private AbstractLocationProvider _locationProvider = null;

        Location currLoc; // Zmienna do przekazywania aktualnej lokalizacji, musi by jako składowa klasy

        void Start()
		{
			if (null == _locationProvider)
			{
				_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
			}
		}

		void Update()
		{
            //Location currLoc = _locationProvider.CurrentLocation;
            currLoc = _locationProvider.CurrentLocation;

            if (currLoc.IsLocationServiceInitializing)
			{
				_statusTextMP.text = "location services are initializing";
			}
			else
			{
				if (!currLoc.IsLocationServiceEnabled)
				{
                    _statusTextMP.text = "location services not enabled";
				}
				else
				{
					if (currLoc.LatitudeLongitude.Equals(Vector2d.zero))
					{
                        _statusTextMP.text = "Waiting for location ....";
					}
					else
					{
                        _statusTextMP.text = string.Format("{0}", currLoc.LatitudeLongitude);
					}
				}
			}

		}

		public double GetLocationLat() // Getter lat
        {
			return currLoc.LatitudeLongitude.x;
		}

        public double GetLocationLon() // Getter lon
        {
            return currLoc.LatitudeLongitude.y;
        }
    }
}
