"use client";
import Quill from "@components/General/Quill";
import React, { useState, useEffect } from "react";
import Image from "next/image";

// Helper for measuring pixel width of text
function getTextWidth(text, font = "16px Arial") {
  if (typeof window === "undefined") return 0;
  const canvas =
    getTextWidth.canvas ||
    (getTextWidth.canvas = document.createElement("canvas"));
  const context = canvas.getContext("2d");
  context.font = font;
  return context.measureText(text).width;
}

export default function AddBrand({ brand, setBrand }) {
  const [serpDevice, setSerpDevice] = useState("desktop");

  // Local state for title, description, and meta fields
  const [title, setTitle] = useState(brand?.title || "");
  const [description, setDescription] = useState(brand?.description || "");
  const [metaTitle, setMetaTitle] = useState(brand?.metaTitle || "");
  const [metaDescription, setMetaDescription] = useState(
    brand?.metaDescription || ""
  );
  const [customUrl, setCustomUrl] = useState(brand?.customUrl || "");
  const [localCanonicalLink, setLocalCanonicalLink] = useState(brand?.canonicalLink || "");
  const [brandImagePreview, setBrandImagePreview] = useState(null);


  // Derived values
  const titleLength = metaTitle.length;
  const titlePixel = getTextWidth(metaTitle, "16px Arial");
  const titleColor =
    titlePixel > 580 || titleLength > 60
      ? "text-red-500"
      : titlePixel > 500 || titleLength > 55
      ? "text-yellow-500"
      : "text-green-600";

  // Handle input changes
  const handleChange = (e) => {
    const { name, value, type, checked, files } = e.target;

    if (name === "brandImage" && type === "file") {
      const file = files && files[0];
      if (brandImagePreview) {
        URL.revokeObjectURL(brandImagePreview); // Revoke old preview URL if exists
      }
      if (file) {
        const previewUrl = URL.createObjectURL(file);
        setBrandImagePreview(previewUrl);
        setBrand((prev) => ({
          ...prev,
          brandImageFile: file, 
        }));
      } else {
        setBrandImagePreview(null);
        setBrand((prev) => ({
          ...prev,
          brandImageFile: null,
        }));
      }
      return; 
    }

    let processedValue = value;

    // Update local state for controlled inputs
    if (name === "title") setTitle(value);
    else if (name === "metaTitle") setMetaTitle(value);
    else if (name === "metaDescription") setMetaDescription(value);
    else if (name === "customUrl") {
      processedValue = value.replace(/\s+/g, "-").toLowerCase();
      setCustomUrl(processedValue);
    } else if (name === "canonicalLink") {
      setLocalCanonicalLink(value);
      // processedValue is 'value' for setBrand
    }
    
    setBrand((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : processedValue,
    }));
  };

  // For Quill editor
  const handleDescriptionChange = (value) => {
    setDescription(value);
    setBrand((prev) => ({
      ...prev,
      description: value,
    }));
  };
  
  useEffect(() => {
    // Initialize local states if brand prop changes (e.g., when editing an existing brand)
    if (brand) {
      setTitle(brand.title || "");
      setDescription(brand.description || "");
      setMetaTitle(brand.metaTitle || "");
      setMetaDescription(brand.metaDescription || "");
      setCustomUrl(brand.customUrl || "");
      setLocalCanonicalLink(brand.canonicalLink || "");
      // Note: brandImagePreview is handled by file input or if you load an existing image URL
      // If brand.imageUrl exists and you want to show it initially:
      // setBrandImagePreview(brand.imageUrl || null); 
    }
  }, [brand]);


  // Effect for cleaning up object URL
  useEffect(() => {
    return () => {
      if (brandImagePreview && brandImagePreview.startsWith("blob:")) {
        URL.revokeObjectURL(brandImagePreview);
      }
    };
  }, [brandImagePreview]);

  return (
    <div >
      <h2 className="text-2xl font-bold text-blue-700 mb-6 flex items-center gap-2">
        افزودن برند جدید
      </h2>
      <div className="grid  grid-cols-1 gap-8">
        <div>
          <div className="mb-4">
            <label className="block text-sm font-bold mb-1 text-gray-700">
              عنوان برند ({title.length} کاراکتر)
              <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              name="title"
              className="w-full p-3 border-2 border-purple-200 rounded-lg focus:border-purple-400 transition"
              value={title}
              onChange={handleChange}
              required
              placeholder="عنوان برند را وارد کنید..."
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-bold mb-1 text-gray-700">
              توضیحات برند
              <span className="text-red-500">*</span>
            </label>
            <Quill
              name="description"
              value={description}
              onChange={handleDescriptionChange}
              placeholder="توضیحات کامل درباره برند..."
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-bold mb-1 text-gray-700">
              لوگو برند
            </label>
            <input
              type="file"
              name="brandImage"
              className="w-full text-sm text-slate-500 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100 border-2 border-purple-200 rounded-lg p-2 focus:border-purple-400 transition"
              onChange={handleChange}
              accept="image/*"
            />
            {brandImagePreview && (
              <div className="mt-2">
                <Image src={brandImagePreview} alt="پیش نمایش لوگو" width={160} height={160} className="object-contain max-h-40 w-auto rounded border border-gray-300 p-1" />
              </div>
            )}
          </div>
          </div>
          <div className="mb-6">
            <label className="block text-sm font-semibold mb-2 text-blue-800 ">
              عنوان متا
            </label>
            <input
              type="text"
              name="metaTitle"
              className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
              value={metaTitle}
              onChange={handleChange}
              placeholder="عنوان متا را وارد کنید"
            />
            <div className="flex justify-between mt-2 text-xs">
              <span className={titleColor}>
                {titleLength} کاراکتر | {Math.round(titlePixel)} پیکسل
              </span>
              <span className={titleColor}>
                {titlePixel > 580
                  ? "طول پیکسل زیاد است!"
                  : titleLength > 60
                  ? "تعداد کاراکتر زیاد است!"
                  : ""}
              </span>
            </div>
          </div>
          <div className="mb-6">
            <label className="block text-sm font-semibold mb-2 text-blue-800">
              توضیحات متا
            </label>
            <input
              type="text"
              name="metaDescription"
              className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
              value={metaDescription}
              onChange={handleChange}
              placeholder="توضیحات متا را وارد کنید"
            />
          </div>
          <div className="mb-6">
            <label className="block text-sm font-semibold mb-2 text-blue-800">
              آدرس اینترنتی (Custom URL)
            </label>
            <input
              type="text"
              name="customUrl"
              className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
              value={customUrl}
              onChange={handleChange}
              placeholder="مثال: my-brand-url"
            />
          </div>
          <div className="mb-6">
            <label className="block text-sm font-semibold mb-2 text-blue-800">
              لینک Canonical
            </label>
            <input
              type="text"
              name="canonicalLink"
              className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
              value={localCanonicalLink}
              onChange={handleChange}
              placeholder="مثال: https://example.com/my-brand"
            />
          </div>
          <div className="mb-6 flex items-center gap-2">
            <input
              type="checkbox"
              name="noIndex"
              className="accent-blue-500 w-5 h-5"
              checked={!!brand?.noIndex}
              onChange={handleChange}
            />
            <label className="text-sm font-semibold text-blue-800">
              عدم ایندکس (No Index)
            </label>
          </div>
        </div>
        <div>
          <div className="mb-4">
            <h3 className="mb-3 font-semibold text-green-700 flex items-center gap-1">
              <svg width="18" height="18" fill="none" viewBox="0 0 24 24">
                <path
                  fill="#059669"
                  d="M12 2a10 10 0 100 20 10 10 0 000-20zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"
                />
              </svg>
              پیش‌نمایش SERP
            </h3>
            <div className="mb-3 flex gap-2">
              <button
                type="button"
                className={`px-4 py-1 rounded-full border transition ${
                  serpDevice === "desktop"
                    ? "bg-blue-500 text-white border-blue-500 shadow"
                    : "bg-gray-100 border-gray-300 text-gray-600"
                }`}
                onClick={() => setSerpDevice("desktop")}
              >
                دسکتاپ
              </button>
              <button
                type="button"
                className={`px-4 py-1 rounded-full border transition ${
                  serpDevice === "mobile"
                    ? "bg-blue-500 text-white border-blue-500 shadow"
                    : "bg-gray-100 border-gray-300 text-gray-600"
                }`}
                onClick={() => setSerpDevice("mobile")}
              >
                موبایل
              </button>
            </div>
            <div
              className={`border p-5 rounded-2xl bg-white shadow-inner transition-all duration-300 ${
                serpDevice === "mobile" ? "max-w-xs" : "max-w-lg"
              }`}
            >
              <div
                className={`font-bold truncate ${titleColor} ${
                  serpDevice === "mobile" ? "text-base" : "text-lg"
                }`}
              >
                {metaTitle}
              </div>
              <div className="text-green-700 text-xs mb-1 truncate">
                {typeof window !== "undefined" ? window.location.origin : ""}
                /{customUrl || (brand?.customUrl && brand.customUrl.startsWith('/') ? "brand/" + brand.customUrl.substring(1) : "brand/" + brand?.customUrl || "")}
              </div>
              <div className="text-gray-600 truncate text-sm">
                {metaDescription}
              </div>
            </div>
          </div>
        </div>
      </div>
  );
}
