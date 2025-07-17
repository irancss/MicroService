"use client";
import Input from "@components/General/Input";
import { useState } from "react";

export default function AddAttributePage() {
  const [attributeName, setAttributeName] = useState("");
  const [attributeNameError, setAttributeNameError] = useState("");
  const inputs = [
    {
      id: "attributeName",
      label: "نام ویژگی",
      placeholder: "نام ویژگی را وارد کنید",
      value: attributeName,
      error: attributeNameError,
      onChange: (e) => setAttributeName(e.target.value),
    },
    {
      id: "attributeUrl",
      label: "آدرس ویژگی",
      placeholder: "آدرس ویژگی را وارد کنید",
      value: attributeUrl,
      error: attributeUrlError,
      onChange: (e) => setAttributeUrl(e.target.value),
    },
    {
      id: "attributeValue",
      label: "مقدار ویژگی",
      placeholder: "مقدار ویژگی را وارد کنید",
      value: attributeValue,
      error: attributeValueError,
      onChange: (e) => setAttributeValue(e.target.value),
    },
  ];
  return (
    <div >
      <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
        افزودن ویژگی جدید
      </h2>
      {inputs.map((input) => (
        <Input
          key={input.id}
          type="text"
          label={input.label}
          placeholder={input.placeholder}
          value={input.value}
          error={input.error}
          onChange={input.onChange}
        />
      ))}
    </div>
  );
}
