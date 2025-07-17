import React, { useState } from "react";
import Tiptap from "@components/General/Tiptap";
import Quill from "@components/General/Quill";
export default function Information({ product }) {
  const initialTitle = product?.title || "";
  const initialDesc = product?.description || "";

  const [title, setTitle] = useState(initialTitle);
  const [description, setDescription] = useState(initialDesc);
  const [titleLength, setTitleLength] = useState(initialTitle.length);
  const [descLength, setDescLength] = useState(initialDesc.length);

  const handleTitleChange = (e) => {
    setTitle(e.target.value);
    setTitleLength(e.target.value.length);
  };

  const handleDescChange = (e) => {
    setDescription(e.target.value);
    setDescLength(e.target.value.length);
  };

  return (
    <div className="p-6 ">
      <h2 className="text-2xl font-bold text-blue-700 mb-6 flex items-center gap-2">
        <svg width="28" height="28" fill="none" viewBox="0 0 24 24">
          <path
            fill="#2563eb"
            d="M12 2a10 10 0 100 20 10 10 0 000-20zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"
          />
        </svg>
        اطلاعات محصول
      </h2>
      <hr className="border-gray-300 mb-4" />
      <div className="mb-4">
        <label className="block text-sm font-bold mb-1 text-gray-700">
          عنوان ({titleLength} کاراکتر)
          <span className="text-red-500">*</span>
        </label>
        <input
          type="text"
          name="title"
          className="w-full p-3 border-2 border-purple-200 rounded-lg focus:border-purple-400 transition"
          value={title}
          onChange={handleTitleChange}
          required
          placeholder="عنوان محصول را وارد کنید..."
        />
      </div>
      <div className="mb-4">
        <label className="block text-sm font-bold mb-1 text-gray-700">
          توضیحات کامل ({descLength} کاراکتر)
          <span className="text-red-500">*</span>
        </label>
        <Quill
          name="description"
          value={description}
          onChange={handleDescChange}
          placeholder="توضیحات کامل درباره محصول..."
        />
      </div>
      <div className="mb-4">
        <label className="block text-sm font-bold mb-1 text-gray-700">
          توضیح کوتاه
        </label>
        <Quill
          name="shortDescription"
          value={description}
          onChange={handleDescChange}
          placeholder="توضیحات کوتاه درباره محصول..."
        />
      </div>
      <div className="mb-4">
        <label className="block text-sm font-bold mb-1 text-gray-700">
          برند
        </label>
        <select
          name="brand"
          className="w-full p-3 border-2 border-purple-200 rounded-lg focus:border-purple-400 transition"
          defaultValue={product?.brand || ""}
        >
          <option value="">انتخاب برند</option>
          <option value="brand1">برند ۱</option>
          <option value="brand2">برند ۲</option>
        </select>
      </div>
    </div>
  );
}
