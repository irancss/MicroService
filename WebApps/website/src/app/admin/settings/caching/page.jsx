"use client";
import { useState } from "react";
import Select from "@/src/app/components/General/Select";
import Input from "@components/General/Input";
import toast from "react-hot-toast";

export default function CachingSettings() {
  // Define the caching options with can disable for every page or regular expression or custom URL
  const cachingOptions = [
    { value: "all", label: "تمام صفحات" },
    { value: "home", label: "صفحه اصلی" },
    { value: "category", label: "صفحات دسته‌بندی" },
    { value: "product", label: "صفحات محصول" },
    { value: "custom", label: "آدرس سفارشی" },
  ];
  const [selectedOption, setSelectedOption] = useState(cachingOptions[0].value);
  const [customUrl, setCustomUrl] = useState("");

  const handleOptionChange = (e) => {
    setSelectedOption(e.target.value);
  };

  const handleCustomUrlChange = (e) => {
    setCustomUrl(e.target.value);
  };
  const handleSubmit = (e) => {
    e.preventDefault();
    // Handle form submission logic here
  };
  toast.success("تنظیمات کش با موفقیت ذخیره شد!");
  // State for active and disabled caches
  const [activeCaches, setActiveCaches] = useState([
    { type: "home", url: "/" },
    { type: "category", url: "/category/electronics" },
  ]);
  const [disabledCaches, setDisabledCaches] = useState([
    { type: "product", url: "/product/12345" },
    { type: "custom", url: "/custom-page" },
  ]);

  // Function to handle removing a cache
  const handleRemoveCache = (cache, index, isActive) => {
    if (isActive) {
      const newActiveCaches = [...activeCaches];
      newActiveCaches.splice(index, 1);
      setActiveCaches(newActiveCaches);
      toast.success(`کش ${cache.type} با موفقیت حذف شد.`);
    } else {
      const newDisabledCaches = [...disabledCaches];
      newDisabledCaches.splice(index, 1);
      setDisabledCaches(newDisabledCaches);
      toast.success(`کش غیرفعال ${cache.type} با موفقیت حذف شد.`);
    }
  };

  // Function to render cache list items
  const renderCacheList = (caches, isActive) => {
    return caches.map((cache, index) => (
      <li
        key={index}
        className="py-2 px-4 border-b border-gray-200 last:border-b-0 flex justify-between items-center"
      >
        <div>
          <span className="font-medium">
            {getCacheLabelByValue(cache.type)}:{" "}
          </span>
          <span className="text-gray-600">{cache.url}</span>
        </div>
        <button
          type="button"
          className="text-red-500 hover:text-red-700"
          onClick={() => handleRemoveCache(cache, index, isActive)}
        >
          حذف
        </button>
      </li>
    ));
  };

  // Function to get label by value from cachingOptions
  const getCacheLabelByValue = (value) => {
    const option = cachingOptions.find((opt) => opt.value === value);
    return option ? option.label : value;
  };
  return (
    <>
      <div className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto">
        <h2 className="text-2xl font-bold text-blue-700 mb-6">تنظیمات کش</h2>
        <form onSubmit={handleSubmit} className="space-y-6">
          <Select
            label="انتخاب نوع کش"
            options={cachingOptions}
            value={selectedOption}
            onChange={handleOptionChange}
          />
          {selectedOption === "custom" && (
            <Input
              label="آدرس سفارشی"
              type="text"
              placeholder="آدرس سفارشی را وارد کنید"
              value={customUrl}
              onChange={handleCustomUrlChange}
            />
          )}
          <button
            type="submit"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            ذخیره تنظیمات
          </button>
        </form>
      </div>
    </>
  );
}
