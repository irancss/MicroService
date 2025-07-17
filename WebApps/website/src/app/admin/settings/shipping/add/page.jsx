"use client";
import { useState } from "react";
import Input from "@components/General/Input";
import toast from "react-hot-toast";
import dynamic from "next/dynamic";

export default function AddShippingMethod() {
  const [deliveryTime, setDeliveryTime] = useState("");
  const [shippingMethod, setShippingMethod] = useState("");
  const [shippingCost, setShippingCost] = useState("");
  const [selectedRegions, setSelectedRegions] = useState();
  const [selectedCities, setSelectedCities] = useState([]);

  const regionOptions = [
    {
      value: "tehran",
      label: "تهران",
      cities: [
        { value: "north", label: "شمال تهران" },
        { value: "south", label: "جنوب تهران" },
        { value: "east", label: "شرق تهران" },
        { value: "west", label: "غرب تهران" },
      ],
    },
    {
      value: "karaj",
      label: "کرج",
      cities: [
        { value: "north", label: "شمال کرج" },
        { value: "south", label: "جنوب کرج" },
        { value: "east", label: "شرق کرج" },
        { value: "west", label: "غرب کرج" },
      ],
    },
  ];

  const formatNumber = (num) => {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  };

  const unformatNumber = (str) => {
    return str.replace(/,/g, "");
  };

  const handleCostChange = (e) => {
    const rawValue = unformatNumber(e.target.value);
    if (!isNaN(rawValue) || rawValue === "") {
      setShippingCost(rawValue === "" ? "" : formatNumber(rawValue));
    }
  };

  const handleShippingMethodChange = (e) => {
    setShippingMethod(e.target.value);
  };

  const handleDeliveryTimeChange = (e) => {
    setDeliveryTime(e.target.value);
  };

  const handleRegionChange = (selectedOptions) => {
    const newRegions = Array.isArray(selectedOptions) ? selectedOptions : []; // Result []
    setSelectedRegions(newRegions);
    setSelectedCities([]); // Reset cities when regions change
  };

  const handleCityChange = (selectedOptions) => {
    setSelectedCities(selectedOptions || []);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    toast.success("روش ارسال با موفقیت اضافه شد!");
  };

  const cityOptions = selectedRegions.flatMap(
    (region) =>
      region.cities?.map((city) => ({
        ...city,
        regionValue: region.value,
      })) || []
  );

  return (
    <div className="p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-4">افزودن روش ارسال جدید</h2>
      <form onSubmit={handleSubmit}>
        <Input
          label="نام روش ارسال"
          name="name"
          type="text"
          placeholder="نام روش ارسال را وارد کنید"
          required
          value={shippingMethod}
          onChange={handleShippingMethodChange}
        />
        <Input
          label="هزینه ارسال"
          name="cost"
          type="text"
          placeholder="هزینه ارسال را وارد کنید"
          required
          value={shippingCost}
          onChange={handleCostChange}
        />

        <div className="mb-4"></div>
        <label className="block text-gray-700 text-sm font-bold mb-2">
          استان‌ها
        </label>
        <select
          multiple
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          onChange={(e) => {
            const selected = Array.from(e.target.selectedOptions).map(
              (option) =>
                regionOptions.find((region) => region.value === option.value)
            );
            handleRegionChange(selected);
          }}
        >
          {regionOptions.map((region) => (
            <option key={region.value} value={region.value}>
              {region.label}
            </option>
          ))}
        </select>
        <p className="text-xs text-gray-500 mt-1">
          برای انتخاب چندگانه، دکمه Ctrl را نگه دارید
        </p>

        {selectedRegions?.length > 0 && (
          <div className="mb-4">
            <label className="block text-gray-700 text-sm font-bold mb-2">
              شهرها
            </label>
            <select
              multiple
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              onChange={(e) => {
                const selected = Array.from(e.target.selectedOptions).map(
                  (option) => {
                    const [regionValue, cityValue] = option.value.split("-");
                    const region = regionOptions.find(
                      (r) => r.value === regionValue
                    );
                    const city = region?.cities.find(
                      (c) => c.value === cityValue
                    );
                    return { ...city, regionValue };
                  }
                );
                handleCityChange(selected);
              }}
            >
              {cityOptions.map((city) => (
                <option
                  key={`${city.regionValue}-${city.value}`}
                  value={`${city.regionValue}-${city.value}`}
                >
                  {city.label}
                </option>
              ))}
            </select>
            <p className="text-xs text-gray-500 mt-1">
              برای انتخاب چندگانه، دکمه Ctrl را نگه دارید
            </p>
          </div>
        )}

        <Input
          label="زمان تحویل"
          name="deliveryTime"
          type="text"
          placeholder="مثلا: 3-5 روز کاری"
          required
          value={deliveryTime}
          onChange={handleDeliveryTimeChange}
        />

        <button
          type="submit"
          className="mt-4 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن روش ارسال
        </button>
      </form>
    </div>
  );
}
