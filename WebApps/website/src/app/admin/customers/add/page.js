"use client";
import Select from "@components/General/Select";
import Input from "@components/General/Input";
import { motion } from "framer-motion";
export default function AddCustomer() {
  const inputs = [
    {
      id: 1,
      label: "نام و نام خانوادگی",
      type: "text",
      placeholder: "نام و نام خانوادگی را وارد کنید",
      required: true,
    },
    {
      id: 2,
      label: "شماره موبایل",
      type: "text",
      placeholder: "شماره موبایل را وارد کنید",
      required: true,
    },
    {
      id: 3,
      label: "آدرس",
      type: "text",
      placeholder: "آدرس را وارد کنید",
      required: true,
    },
    {
      id: 4,
      label: "شهر",
      type: "text",
      placeholder: "شهر را وارد کنید",
      required: true,
    },
    {
      id: 5,
      label: "استان",
      type: "text",
      placeholder: "استان را وارد کنید",
      required: true,
    },
    {
      id: 6,
      label: "کد پستی",
      type: "text",
      placeholder: "کد پستی را وارد کنید",
      required: false,
    },
    {
      id: 7,
      label: "ایمیل",
      type: "email",
      placeholder: "ایمیل را وارد کنید",
      required: false,
    },
    {
      id: 8,
      label: "تاریخ تولد",
      type: "date",
      placeholder: "تاریخ تولد را وارد کنید",
      required: false,
    },
    {
      id: 9,
      label: "رمز عبور",
      type: "password",
      placeholder: "رمز عبور را وارد کنید",
      required: false,
    },
  ];
  const selects = [
    {
      id: 1,
      label: "جنسیت",
      options: [
        { value: "مرد", label: "مرد" },
        { value: "زن", label: "زن" },
      ],
      required: false,
    },
    {
      id: 2,
      label: "وضعیت تاهل",
      options: [
        { value: "مجرد", label: "مجرد" },
        { value: "متاهل", label: "متاهل" },
      ],
      required: false,
    },
    {
      id: 3,
      label: "نوع مشتری",
      options: [
        { value: "عادی", label: "عادی" },
        { value: "VIP", label: "VIP" },
      ],
      required: false,
    },
    {
      id: 4,
      label: "وضعیت مشتری",
      options: [
        { value: "فعال", label: "فعال" },
        { value: "غیرفعال", label: "غیرفعال" },
      ],
      required: true,
    },
    {
        id: 5,
        label: "نقش مشتری",
        options: [
          { value: "کاربر", label: "کاربر" },
          { value: "مدیر", label: "مدیر" },
          { value: "فروشنده", label: "فروشنده" },
        ],
    }
  ];
  return (
    <motion.div
      initial={{ opacity: 0, y: -50 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -50 }}
      transition={{ duration: 0.3 }}
      id="add-customer"
      dir="rtl"
      lang="fa"
      role="form"
      aria-label="Add Customer Form"
      tabIndex={0}
      aria-live="polite"
      aria-atomic="true"
      aria-relevant="all"
      className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto"
    >
      <h2 className="text-2xl font-bold text-blue-700 mb-6">
        افزودن مشتری جدید
      </h2>
      <hr className="border-b border-blue-200 mb-6" />
      <form className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {inputs.map((input) => (
            <div key={input.id} className="mb-2">
              <label
                htmlFor={input.id}
                className="block text-lg font-medium text-gray-700"
              >
                {input.label}
              </label>
              <Input
                id={input.id}
                type={input.type}
                placeholder={input.placeholder}
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
          ))}
          {selects.map((select) => (
            <div key={select.id} className="mb-2">
              <label
                htmlFor={select.id}
                className="block text-lg font-medium text-gray-700"
              >
                {select.label}
              </label>
              <Select
                id={select.id}
                options={select.options}
                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
          ))}
        </div>
        <div className="mt-6">
          <button
            type="submit"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            افزودن مشتری
          </button>
        </div>
      </form>
    </motion.div>
  );
}
