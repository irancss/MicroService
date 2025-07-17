import React, { useRef, useState } from "react";

export default function Media() {
    const [featuredImage, setFeaturedImage] = useState(null);
    const [galleryImages, setGalleryImages] = useState([]);
    const featuredInputRef = useRef(null);
    const galleryInputRef = useRef(null);

    const handleFeaturedChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setFeaturedImage(URL.createObjectURL(file));
        }
    };

    const handleGalleryChange = (e) => {
        const files = Array.from(e.target.files);
        setGalleryImages(files.map(file => URL.createObjectURL(file)));
    };

    const removeFeatured = () => {
        setFeaturedImage(null);
        if (featuredInputRef.current) featuredInputRef.current.value = "";
    };

    const removeGalleryImage = (idx) => {
        setGalleryImages(galleryImages.filter((_, i) => i !== idx));
        if (galleryInputRef.current) galleryInputRef.current.value = "";
    };

    return (
        <div className="mt-6 space-y-8">
            {/* Featured Image */}
            <div>
                <label className="block text-base font-semibold mb-2 text-gray-700">
                    تصویر اصلی محصول
                    <span className="text-red-500">*</span>
                </label>
                <div className="border-2 border-dashed border-gray-300 bg-gray-50 p-8 rounded-xl shadow-sm flex flex-col items-center transition hover:border-blue-400 hover:bg-blue-50">
                    <input
                        type="file"
                        name="image"
                        className="hidden"
                        id="featuredImage"
                        accept="image/*"
                        ref={featuredInputRef}
                        onChange={handleFeaturedChange}
                    />
                    <label htmlFor="featuredImage" className="cursor-pointer flex flex-col items-center">
                        <div className="text-5xl mb-2 text-blue-400">📁</div>
                        <span className="text-gray-500">برای آپلود کلیک یا فایل را بکشید</span>
                    </label>
                    {featuredImage && (
                        <div className="mt-6 flex flex-col items-center">
                            <img src={featuredImage} alt="Featured" className="h-36 w-36 object-cover rounded-lg shadow-md border" />
                            <button
                                type="button"
                                className="mt-3 px-4 py-1 bg-red-100 text-red-600 rounded hover:bg-red-200 transition text-sm font-medium"
                                onClick={removeFeatured}
                            >
                                حذف تصویر
                            </button>
                        </div>
                    )}
                </div>
            </div>
            {/* Gallery Images */}
            <div>
                <label className="block text-base font-semibold mb-2 text-gray-700">
                    گالری تصاویر
                </label>
                <div className="border-2 border-dashed border-gray-300 bg-gray-50 p-8 rounded-xl shadow-sm flex flex-col items-center transition hover:border-blue-400 hover:bg-blue-50">
                    <input
                        type="file"
                        name="gallery"
                        className="hidden"
                        id="galleryImages"
                        accept="image/*"
                        multiple
                        ref={galleryInputRef}
                        onChange={handleGalleryChange}
                    />
                    <label htmlFor="galleryImages" className="cursor-pointer flex flex-col items-center">
                        <div className="text-5xl mb-2 text-green-400">➕</div>
                        <span className="text-gray-500">افزودن تصویر</span>
                    </label>
                    {galleryImages.length > 0 && (
                        <div className="mt-6 flex flex-wrap gap-5 justify-center">
                            {galleryImages.map((img, idx) => (
                                <div key={idx} className="relative group">
                                    <img src={img} alt={`Gallery ${idx}`} className="h-24 w-24 object-cover rounded-lg shadow border" />
                                    <button
                                        type="button"
                                        className="absolute top-1 right-1 bg-white/80 text-red-500 rounded-full px-2 py-0 text-lg opacity-0 group-hover:opacity-100 transition"
                                        onClick={() => removeGalleryImage(idx)}
                                        title="حذف"
                                    >
                                        ×
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}