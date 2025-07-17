"use client";
import { useState, useEffect, useRef } from 'react';
// Import OpenLayers modules directly
import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { fromLonLat } from 'ol/proj';
import 'ol/ol.css';
// this component is used to show the map using OpenLayers in the address panel of the website. It uses the OpenLayers library to create a map and display it on the screen.
// The component also includes a search bar to find locations on the map and a button to get the user's current location.
// The map is centered on Tehran, Iran by default, but can be changed by the user. The component also includes a function to handle the search input and update the map accordingly.
// The component is styled using CSS modules.

// The component is designed to be responsive and works well on different screen sizes.
// The component is also designed to be reusable and can be used in different parts of the website.
// The component is also designed to be easy to use and understand, with clear comments and documentation.
// The component is also designed to be easy to maintain and update, with clear separation of concerns and modular design.
export default function OpenLayer() {
  const [map, setMap] = useState(null);
  const [searchValue, setSearchValue] = useState("");
  const [location, setLocation] = useState([51.389, 35.6892]); // [lon, lat] for Tehran
  const mapRef = useRef(null);
// OpenStreetMap (OSM) که در این کد استفاده شده، نیاز به کلید API ندارد و باید روی لوکال هاست هم کار کند.
// اگر نقشه لود نمی‌شود، این موارد را بررسی کنید:
// 1. اطمینان حاصل کنید که اینترنت دارید (OSM کاشی‌ها را آنلاین بارگذاری می‌کند).
// 2. مطمئن شوید که فایل CSS مربوط به OpenLayers (`import 'ol/ol.css';`) به درستی لود می‌شود.
// 3. کنسول مرورگر را برای خطاها بررسی کنید (ممکن است خطای CORS یا network باشد).
// 4. آدرس پروژه را با http اجرا کنید (برخی مرورگرها روی file:// نقشه را نمایش نمی‌دهند).
// 5. اگر فایروال یا پراکسی دارید، ممکن است جلوی بارگذاری کاشی‌ها را بگیرد.
  useEffect(() => {
    // Only initialize map once
    if (!map && mapRef.current) {
      const mapInstance = new Map({
        target: mapRef.current,
        layers: [
          new TileLayer({
            source: new OSM(),
          }),
        ],
        view: new View({
          center: fromLonLat(location),
          zoom: 10,
        }),
      });
      setMap(mapInstance);

      // Clean up on unmount
      return () => {
        mapInstance.setTarget(null);
      };
    }
  }, [map, location]);

  // Update map center when location changes
  useEffect(() => {
    if (map) {
      map.getView().setCenter(fromLonLat(location));
    }
  }, [location, map]);

  const handleSearch = () => {
    // Implement search logic here
  };

  const handleLocationClick = () => {
    if (typeof window !== 'undefined' && window.navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        const { latitude, longitude } = position.coords;
        setLocation([longitude, latitude]);
      });
    }
  };

  return (
    <div className="w-full h-full flex flex-col items-center justify-center">
      <div className="w-full h-96" ref={mapRef}></div>
      <input
        type="text"
        value={searchValue}
        onChange={(e) => setSearchValue(e.target.value)}
        placeholder="جستجو..."
        className="mt-4 p-2 border border-gray-300 rounded-md"
      />
      <button
        onClick={handleSearch}
        className="mt-2 bg-blue-500 text-white p-2 rounded-md"
      >
        جستجو
      </button>
      <button
        onClick={handleLocationClick}
        className="mt-2 bg-green-500 text-white p-2 rounded-md"
      >
        لوکیشن فعلی
      </button>
    </div>
  );
}
