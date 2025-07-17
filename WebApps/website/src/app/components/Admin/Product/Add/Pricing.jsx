import React, { useState, useEffect } from "react";
import Swal from "sweetalert2";

export default function Pricing() {
  const [hasAttributes, setHasAttributes] = useState(false);
  const [attributes, setAttributes] = useState([{ name: "", values: [""] }]);
  const [variants, setVariants] = useState([]);

  // ------------------------------
  // Attribute Handlers
  // ------------------------------
  const handleAddAttribute = () => {
    setAttributes([...attributes, { name: "", values: [""] }]);
  };

  const handleRemoveAttribute = (index) => {
    setAttributes(attributes.filter((_, i) => i !== index));
  };

  const handleAttributeNameChange = (index, name) => {
    const newAttributes = [...attributes];
    newAttributes[index].name = name;
    setAttributes(newAttributes);
  };

  const handleAddAttributeValue = (attrIndex) => {
    const newAttributes = [...attributes];
    newAttributes[attrIndex].values.push("");
    setAttributes(newAttributes);
  };

  const handleRemoveAttributeValue = (attrIndex, valueIndex) => {
    const newAttributes = [...attributes];
    newAttributes[attrIndex].values = newAttributes[attrIndex].values.filter(
      (_, i) => i !== valueIndex
    );
    setAttributes(newAttributes);
  };

  const handleAttributeValueChange = (attrIndex, valueIndex, value) => {
    const newAttributes = [...attributes];
    newAttributes[attrIndex].values[valueIndex] = value;
    setAttributes(newAttributes);
  };

  // ------------------------------
  // Generate Variant Combinations
  // ------------------------------
  const generateCombinations = (attrs) => {
    if (!attrs.length) return [];
    const values = attrs.map((a) => a.values.filter((v) => v.trim() !== ""));
    if (values.some((v) => v.length === 0)) return [];

    const combine = (arr) =>
      arr.reduce(
        (acc, curr) => acc.flatMap((x) => curr.map((y) => [...x, y])),
        [[]]
      );

    return combine(values);
  };

  const handleGenerateVariants = () => {
    const combinations = generateCombinations(attributes);
    const newVariants = combinations.map((combo) => ({
      attributes: combo,
      price: "",
      specialPrice: "",
      stock: "",
      sku: "",
    }));
    setVariants(newVariants);
  };

  const handleVariantChange = (idx, field, value) => {
    setVariants((prev) =>
      prev.map((v, i) => (i === idx ? { ...v, [field]: value } : v))
    );
  };

  const handleRemoveVariant = (idx) => {
    setVariants((prev) => prev.filter((_, i) => i !== idx));
  };

  // ------------------------------
  // Simple Form
  // ------------------------------
  const [simpleForm, setSimpleForm] = useState({
    price: "",
    specialPrice: "",
    stock: "",
    sku: "",
  });

  const handleSimpleFormChange = (e) => {
    const { name, value } = e.target;
    if (
      name === "specialPrice" &&
      value &&
      parseFloat(value) > parseFloat(simpleForm.price)
    ) {
      Swal.fire({
        icon: "warning",
        title: "قیمت ویژه",
        text: "قیمت ویژه نمی‌تواند بیشتر از قیمت اصلی باشد.",
      });
      return;
    }
    setSimpleForm((prev) => ({ ...prev, [name]: value }));
  };

  // ------------------------------
  // Render UI
  // ------------------------------
  const renderSimpleForm = () => (
    <div className="mt-4 grid gap-4">
      {["price", "specialPrice", "stock", "sku"].map((field) => (
        <div key={field}>
          <label className="block text-sm font-medium mb-1">
            {field === "price"
              ? "قیمت *"
              : field === "specialPrice"
              ? "قیمت ویژه"
              : field === "stock"
              ? "موجودی *"
              : "SKU"}
          </label>
          <input
            type={field === "sku" ? "text" : "number"}
            name={field}
            min="0"
            className="w-full p-2 border rounded"
            value={simpleForm[field]}
            onChange={handleSimpleFormChange}
          />
        </div>
      ))}
    </div>
  );

  const renderVariableForm = () => (
    <div className="mt-4">
      {/* Attributes Form */}
      <div className="p-4 border rounded-lg bg-gray-50 mb-6">
        <h3 className="text-lg font-semibold mb-3">ویژگی‌ها</h3>
        {attributes.map((attr, i) => (
          <div
            key={i}
            className="bg-white p-3 mb-3 border rounded-md shadow-sm"
          >
            <div className="flex items-center gap-4 mb-2">
              <input
                type="text"
                placeholder="نام ویژگی (مثلا: رنگ)"
                className="flex-grow p-2 border rounded"
                value={attr.name}
                onChange={(e) => handleAttributeNameChange(i, e.target.value)}
              />
              <button
                className="text-red-600 font-semibold"
                onClick={() => handleRemoveAttribute(i)}
              >
                حذف
              </button>
            </div>
            <div>
              {attr.values.map((val, j) => (
                <div key={j} className="flex items-center gap-2 mt-1">
                  <input
                    type="text"
                    value={val}
                    className="flex-grow p-1 border rounded"
                    onChange={(e) =>
                      handleAttributeValueChange(i, j, e.target.value)
                    }
                    placeholder="مقدار (مثلا: قرمز)"
                  />
                  <button
                    onClick={() => handleRemoveAttributeValue(i, j)}
                    className="text-xs text-red-500"
                  >
                    ×
                  </button>
                </div>
              ))}
              <button
                onClick={() => handleAddAttributeValue(i)}
                className="mt-2 text-sm text-blue-600 font-medium"
              >
                افزودن مقدار
              </button>
            </div>
          </div>
        ))}
        <button
          onClick={handleAddAttribute}
          className="w-full p-2 mt-2 bg-blue-100 text-blue-700 rounded hover:bg-blue-200 font-semibold"
        >
          افزودن ویژگی جدید
        </button>
      </div>

      {/* Generate Variants */}
      <div className="mb-4">
        <button
          onClick={handleGenerateVariants}
          className="px-4 py-2 bg-green-600 text-white rounded font-semibold hover:bg-green-700"
        >
          ساخت خودکار متغیرها
        </button>
      </div>

      {/* Variants Table */}
      {variants.length > 0 && (
        <div className="overflow-x-auto">
          <h3 className="text-lg font-semibold mb-3">قیمت‌گذاری متغیرها</h3>
          <table className="min-w-full border">
            <thead>
              <tr>
                {attributes.map((attr, i) => (
                  <th key={i} className="px-2 py-1 border">
                    {attr.name || "ویژگی"}
                  </th>
                ))}
                <th className="border px-2 py-1">قیمت</th>
                <th className="border px-2 py-1">قیمت ویژه</th>
                <th className="border px-2 py-1">موجودی</th>
                <th className="border px-2 py-1">SKU</th>
                <th className="border px-2 py-1">حذف</th>
              </tr>
            </thead>
            <tbody>
              {variants.map((variant, idx) => (
                <tr key={idx}>
                  {variant.attributes.map((val, i) => (
                    <td key={i} className="border px-2 py-1 text-center">
                      {val}
                    </td>
                  ))}
                  {["price", "specialPrice", "stock", "sku"].map((f) => (
                    <td key={f} className="border px-2 py-1">
                      <input
                        type={f === "sku" ? "text" : "number"}
                        className="w-20 p-1 border rounded"
                        value={variant[f]}
                        onChange={(e) =>
                          handleVariantChange(idx, f, e.target.value)
                        }
                      />
                    </td>
                  ))}
                  <td className="border text-center">
                    <button
                      onClick={() => handleRemoveVariant(idx)}
                      className="text-red-500 font-semibold"
                    >
                      حذف
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );

  return (
    <div className="p-6 bg-gray-100 rounded-lg shadow-md">
      <h2 className="text-xl font-bold text-gray-800 mb-4">
        قیمت‌گذاری و موجودی
      </h2>
      <div className="flex gap-4 mb-6">
        <button
          onClick={() => setHasAttributes(false)}
          className={`px-6 py-2 rounded font-semibold ${
            !hasAttributes ? "bg-blue-600 text-white" : "bg-white border"
          }`}
        >
          محصول ساده
        </button>
        <button
          onClick={() => setHasAttributes(true)}
          className={`px-6 py-2 rounded font-semibold ${
            hasAttributes ? "bg-blue-600 text-white" : "bg-white border"
          }`}
        >
          محصول متغیر
        </button>
      </div>
      <hr className="mb-4" />
      {hasAttributes ? renderVariableForm() : renderSimpleForm()}
    </div>
  );
}
