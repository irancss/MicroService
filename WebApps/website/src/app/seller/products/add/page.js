"use client";
import Pricing from "@components/Admin/Product/Add/Pricing";
import Media from "@components/Admin/Product/Add/Media";
import Information from "@components/Admin/Product/Add/Information";
import { useState } from "react";

const initialProduct = {
  title: "",
  description: "",
  shortDescription: "",
  brand: "",
  price: 0,
  specialPrice: 0,
  stock: 0,
  sku: "",
  tags: [],
  categories: [],
  images: [],
  hasAttributes: false,
  attributes: [],
};

const tabs = [
  { label: "اطلاعات پایه", component: Information },
  { label: "تصاویر", component: Media },
  { label: "قیمت", component: Pricing },
];

export default function AddProductPage() {
  const [activeTab, setActiveTab] = useState(0);
  const [product, setProduct] = useState(initialProduct);

  const ActiveComponent = tabs[activeTab].component;

  const handleSubmit = (e) => {
    e.preventDefault();
    // Here you can handle product submit logic
  };

  // Check if there is a "title" query param to determine if editing
  const isEdit =
    typeof window !== "undefined" &&
    new URLSearchParams(window.location.search).has("title");

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">
        {isEdit ? "ویرایش محصول" : "افزودن محصول"}
      </h1>
      <form onSubmit={handleSubmit}>
        <div className="mb-6">
          <div className="flex border-b">
            {tabs.map((tab, idx) => (
              <button
                key={tab.label}
                type="button"
                className={`py-2 px-4 ${
                  activeTab === idx ? "border-b-2 border-blue-500" : ""
                }`}
                onClick={() => setActiveTab(idx)}
              >
                {tab.label}
              </button>
            ))}
          </div>
          <ActiveComponent product={product} setProduct={setProduct} />
        </div>
        <div className="flex justify-end mt-6">
          <button
            type="submit"
            className="bg-blue-500 text-white px-6 py-2 rounded font-medium hover:bg-blue-600"
          >
            {activeTab === tabs.length - 1 ? "ذخیره" : "بعدی"}
          </button>
        </div>
      </form>
    </div>
  );
}
