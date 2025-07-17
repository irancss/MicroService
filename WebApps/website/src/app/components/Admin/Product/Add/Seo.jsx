"use client";
import React, { useState, useRef } from "react";

// Helper for measuring pixel width of text
function getTextWidth(text, font = "16px Arial") {
    if (typeof window === "undefined") return 0;
    const canvas = getTextWidth.canvas || (getTextWidth.canvas = document.createElement("canvas"));
    const context = canvas.getContext("2d");
    context.font = font;
    return context.measureText(text).width;
}

export default function Seo({ product, setProduct }) {
    const [tags, setTags] = useState(product?.tags || []);
    const [inputValue, setInputValue] = useState("");
    const inputRef = useRef(null);
    const [serpDevice, setSerpDevice] = useState("desktop");

    // Handle input changes for meta fields
    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        let newValue = value;
        if (name === "customUrl") {
            newValue = value.replace(/\s+/g, "-");
        }
        setProduct((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : newValue,
        }));
    };

    // Tag management
    const handleTagRemove = (removedTag) => {
        const newTags = tags.filter((tag) => tag !== removedTag);
        setTags(newTags);
        setProduct((prev) => ({
            ...prev,
            tags: newTags,
        }));
    };

    const handleInputChange = (e) => {
        setInputValue(e.target.value);
    };

    const handleInputConfirm = () => {
        const newTag = inputValue.trim();
        if (newTag && !tags.includes(newTag)) {
            const newTags = [...tags, newTag];
            setTags(newTags);
            setProduct((prev) => ({
                ...prev,
                tags: newTags,
            }));
        }
        setInputValue("");
    };

    // Fix: Only count default values for length/pixel if user has not entered anything
    const metaTitleValue = product?.metaTitle ?? "";
    const metaDescriptionValue = product?.metaDescription ?? "";

    const metaTitle = metaTitleValue || product?.name || "عنوان محصول";
    const metaDescription = metaDescriptionValue || "توضیحات متا را اینجا وارد کنید.";
    const customUrl = product?.customUrl || "/custom-url";

    // Only count user input for length/pixel, not placeholder/default
    const titleLength = metaTitleValue.length;
    const titlePixel = getTextWidth(metaTitleValue, "bold 20px Arial");
    let titleColor = "text-green-600";
    if (titlePixel > 580) {
        titleColor = "text-red-600";
    } else if (titleLength > 60) {
        titleColor = "text-red-600";
    } else if (titleLength > 55) {
        titleColor = "text-yellow-500";
    } else if (titleLength === 0) {
        titleColor = "text-gray-400";
    }

    const descLength = metaDescriptionValue.length;
    let descColor = "text-green-600";
    if (descLength > 165) {
        descColor = "text-red-600";
    } else if (descLength < 120 && descLength > 0) {
        descColor = "text-yellow-500";
    }

    return (
        <div >
            <h2 className="text-2xl font-bold text-blue-700 mb-6 flex items-center gap-2">
                <svg width="28" height="28" fill="none" viewBox="0 0 24 24"><path fill="#2563eb" d="M12 2a10 10 0 100 20 10 10 0 000-20zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/></svg>
                تنظیمات سئو محصول
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div>
                    <div className="mb-6">
                        <label className="block text-sm font-semibold mb-2 text-blue-800">
                            عنوان متا
                        </label>
                        <input
                            type="text"
                            name="metaTitle"
                            className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
                            value={metaTitleValue}
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
                                    ? "کاراکتر زیاد است!"
                                    : titleLength > 55
                                    ? "نزدیک به حد مجاز"
                                    : titleLength === 0
                                    ? "خالی است"
                                    : "مناسب"}
                            </span>
                        </div>
                    </div>
                    <div className="mb-6">
                        <label className="block text-sm font-semibold mb-2 text-blue-800">
                            توضیحات متا
                        </label>
                        <textarea
                            name="metaDescription"
                            rows="3"
                            className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
                            value={metaDescriptionValue}
                            onChange={handleChange}
                            placeholder="توضیحات متا را وارد کنید"
                        ></textarea>
                        <div className="flex justify-between mt-2 text-xs">
                            <span className={descColor}>{descLength} کاراکتر</span>
                            <span className={descColor}>
                                {descLength > 165
                                    ? "خیلی طولانی"
                                    : descLength < 120 && descLength > 0
                                    ? "کوتاه"
                                    : "مناسب"}
                            </span>
                        </div>
                    </div>
                    <div className="mb-6">
                        <label className="block text-sm font-semibold mb-2 text-blue-800">
                            آدرس اینترنتی
                        </label>
                        <input
                            type="text"
                            name="customUrl"
                            className="w-full p-3 border border-blue-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
                            value={product?.customUrl || ""}
                            onChange={handleChange}
                            placeholder="مثال: /my-product"
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
                            value={product?.canonicalLink || ""}
                            onChange={handleChange}
                            placeholder="مثال: https://example.com/my-product"
                        />
                    </div>
                    <div className="mb-6 flex items-center gap-2">
                        <input
                            type="checkbox"
                            name="noIndex"
                            className="accent-blue-500 w-5 h-5"
                            checked={!!product?.noIndex}
                            onChange={handleChange}
                        />
                        <label className="text-sm font-semibold text-blue-800">
                            عدم ایندکس (No Index)
                        </label>
                    </div>
                    <div className="mb-8">
                        <h3 className="mb-2 font-semibold text-blue-700 flex items-center gap-1">
                            <svg width="18" height="18" fill="none" viewBox="0 0 24 24"><path fill="#2563eb" d="M17.657 6.343a8 8 0 11-11.314 0 8 8 0 0111.314 0zm-1.414 1.414a6 6 0 10-8.486 8.486 6 6 0 008.486-8.486z"/></svg>
                            تگ‌ها
                        </h3>
                        <div className="flex flex-wrap gap-2">
                            {tags.map((tag) => (
                                <span
                                    key={tag}
                                    className="bg-blue-100 text-blue-700 px-3 py-1 rounded-full flex items-center text-xs shadow-sm border border-blue-200"
                                >
                                    {tag}
                                    <button
                                        type="button"
                                        className="ml-2 text-red-400 hover:text-red-700 transition"
                                        onClick={() => handleTagRemove(tag)}
                                        aria-label="حذف تگ"
                                    >
                                        ×
                                    </button>
                                </span>
                            ))}
                            <input
                                ref={inputRef}
                                type="text"
                                className="border border-blue-200 p-1 w-28 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white transition"
                                value={inputValue}
                                onChange={handleInputChange}
                                placeholder="تگ جدید"
                                onKeyDown={(e) => e.key === "Enter" && handleInputConfirm()}
                            />
                            <button
                                type="button"
                                onClick={handleInputConfirm}
                                className="px-3 py-1 bg-blue-500 text-white rounded-full hover:bg-blue-600 text-xs shadow transition"
                            >
                                افزودن
                            </button>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="mb-4">
                        <h3 className="mb-3 font-semibold text-green-700 flex items-center gap-1">
                            <svg width="18" height="18" fill="none" viewBox="0 0 24 24"><path fill="#059669" d="M12 2a10 10 0 100 20 10 10 0 000-20zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/></svg>
                            پیش‌نمایش SERP
                        </h3>
                        <div className="mb-3 flex gap-2">
                            <button
                                type="button"
                                className={`px-4 py-1 rounded-full border transition ${serpDevice === "desktop" ? "bg-blue-500 text-white border-blue-500 shadow" : "bg-gray-100 border-gray-300 text-gray-600"}`}
                                onClick={() => setSerpDevice("desktop")}
                            >
                                دسکتاپ
                            </button>
                            <button
                                type="button"
                                className={`px-4 py-1 rounded-full border transition ${serpDevice === "mobile" ? "bg-blue-500 text-white border-blue-500 shadow" : "bg-gray-100 border-gray-300 text-gray-600"}`}
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
                            <div className={`font-bold truncate ${titleColor} ${serpDevice === "mobile" ? "text-base" : "text-lg"}`}>
                                {metaTitle}
                            </div>
                            <div className="text-green-700 text-xs mb-1 truncate">
                                {typeof window !== "undefined" ? window.location.origin : ""}{customUrl}
                            </div>
                            <div className="text-gray-600 truncate text-sm">
                                {metaDescription}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}