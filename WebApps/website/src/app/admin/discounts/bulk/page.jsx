"use client";
import Select from "@components/General/Select";
import Input from "@components/General/Input";

import { useState } from "react";
import toast from "react-hot-toast";

export default function BulkDiscounts() {
  const initialFormData = {
    quantity: "",
    type: "", // مقدار پیش‌فرض برای نوع
    discountAmount: "",
    maxUsage: "",
    description: "",
    startDate: "",
    endDate: "",
  };

  const [formData, setFormData] = useState(initialFormData);

  const selects = [
    {
      label: "نوع کد تخفیف",
      name: "type",
      type: "select",
      options: [
        { value: "percent", label: "درصدی" },
        { value: "fixed", label: "ثابت" },
      ],
      required: true,
    },
  ];
  const inputs = [
    {
      label: "تعداد تخفیف",
      name: "quantity",
      type: "number",
      placeholder: "تعداد کدهای تخفیف برای ایجاد",
      required: true,
    },
    {
      label: "مقدار تخفیف",
      name: "discountAmount",
      type: "number",
      placeholder: "مقدار تخفیف را وارد کنید",
      required: true,
    },
    {
      label: "تاریخ شروع",
      name: "startDate",
      type: "date",
      required: true,
    },
    {
      label: "تاریخ انقضا",
      name: "endDate",
      type: "date",
      required: true,
    },
    {
      label: "حداکثر استفاده (اختیاری)",
      name: "maxUsage",
      type: "number",
      placeholder: "حداکثر تعداد استفاده از هر کد",
    },
    {
      label: "توضیحات (اختیاری)",
      name: "description",
      type: "textarea",
      placeholder: "توضیحات مربوط به کد تخفیف را وارد کنید",
    },
  ];

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  // تابع ساده برای تولید کد تخفیف تصادفی
  const generateRandomCode = (length = 8) => {
    const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    let result = "";
    for (let i = 0; i < length; i++) {
      result += characters.charAt(
        Math.floor(Math.random() * characters.length)
      );
    }
    return result;
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const {
      quantity,
      type,
      discountAmount,
      startDate,
      endDate,
      maxUsage,
      description,
    } = formData;

    if (!quantity || !type || !discountAmount || !startDate || !endDate) {
      toast.error("لطفاً تمام فیلدهای اجباری را پر کنید.");
      return;
    }

    const numQuantity = parseInt(quantity, 10);
    if (isNaN(numQuantity) || numQuantity <= 0) {
      toast.error("تعداد تخفیف باید یک عدد مثبت معتبر باشد.");
      return;
    }
    if (new Date(startDate) >= new Date(endDate)) {
      toast.error("تاریخ شروع باید قبل از تاریخ انقضا باشد.");
      return;
    }

    const generatedCodes = [];
    for (let i = 0; i < numQuantity; i++) {
      generatedCodes.push({
        code: generateRandomCode(), // تولید کد تصادفی
        type,
        amount: discountAmount,
        startDate,
        endDate,
        maxUsage: maxUsage || null, // اگر خالی بود null بفرست
        description: description || "",
        // سایر اطلاعات مورد نیاز برای هر کد
      });
    }

    // در اینجا می‌توانید generatedCodes را به API ارسال کنید
    toast.success(`${numQuantity} کد تخفیف با موفقیت ایجاد شد!`);

    // اختیاری: فرم را پس از ارسال موفقیت‌آمیز پاک کنید
    // setFormData(initialFormData);
  };

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">
        ایجاد کدهای تخفیف به صورت عمده
      </h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        {selects.map((select) => (
          <Select
            key={select.name}
            label={select.label}
            name={select.name}
            options={select.options}
            required={select.required}
            value={formData[select.name]}
          />
        ))}
        {inputs.map((input) => (
          <Input
            key={input.name}
            label={input.label}
            name={input.name}
            type={input.type}
            placeholder={input.placeholder}
            options={input.options}
            required={input.required}
            value={formData[input.name]}
            onChange={handleChange}
          />
        ))}
        <button
          type="submit"
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          ایجاد کدهای تخفیف
        </button>
      </form>
    </div>
  );
}
