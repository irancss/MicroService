"use client";
import AnimatedHr from "@/app/components/Animated/Hr";
import React, { useState } from "react";
import * as Yup from "yup";

const validationSchema = Yup.object({
  name: Yup.string().required("نام و نام خانوادگی الزامی است"),
  dob: Yup.date().required("تاریخ تولد الزامی است"),
  nationalId: Yup.string().required("کد ملی الزامی است"),
  email: Yup.string().email("ایمیل نامعتبر است").required("ایمیل الزامی است"),
  cardNumber: Yup.string().required("شماره کارت الزامی است"),
  accountNumber: Yup.string().required("شماره حساب الزامی است"),
  bankAccountNumber: Yup.string().required("شماره شبا الزامی است"),
  economicCode: Yup.string(),
});

const inputClass =
  "w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-400";
const labelClass = "block mb-1 font-medium";
const errorClass = "text-red-500 text-sm mt-1";
const fieldClass = "mb-4";

export default function AccountPage() {
  const [values, setValues] = useState({
    name: "",
    dob: "",
    nationalId: "",
    email: "",
    cardNumber: "",
    accountNumber: "",
    bankAccountNumber: "",
    economicCode: "",
  });
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await validationSchema.validate(values, { abortEarly: false });
      setErrors({});
      alert(JSON.stringify(values, null, 2));
    } catch (err) {
      if (err.inner) {
        const formErrors = {};
        err.inner.forEach((error) => {
          formErrors[error.path] = error.message;
        });
        setErrors(formErrors);
      }
    }
  };

  return (
    <div className=" mx-auto   ounded-lg p-8">
      <h2 className="text-2xl font-bold mb-6  text-blue-700">
        اطلاعات حساب کاربری
      </h2>
      <AnimatedHr className="mb-8" />
      <form onSubmit={handleSubmit}>
        <div className={fieldClass}>
          <label className={labelClass}>نام و نام خانوادگی:</label>
          <input
            name="name"
            value={values.name}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.name && <div className={errorClass}>{errors.name}</div>}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>تاریخ تولد:</label>
          <input
            name="dob"
            type="date"
            value={values.dob}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.dob && <div className={errorClass}>{errors.dob}</div>}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>کد ملی:</label>
          <input
            name="nationalId"
            value={values.nationalId}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.nationalId && (
            <div className={errorClass}>{errors.nationalId}</div>
          )}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>ایمیل:</label>
          <input
            name="email"
            type="email"
            value={values.email}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.email && <div className={errorClass}>{errors.email}</div>}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>شماره کارت:</label>
          <input
            name="cardNumber"
            value={values.cardNumber}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.cardNumber && (
            <div className={errorClass}>{errors.cardNumber}</div>
          )}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>شماره حساب:</label>
          <input
            name="accountNumber"
            value={values.accountNumber}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.accountNumber && (
            <div className={errorClass}>{errors.accountNumber}</div>
          )}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>شماره شبا:</label>
          <input
            name="bankAccountNumber"
            value={values.bankAccountNumber}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.bankAccountNumber && (
            <div className={errorClass}>{errors.bankAccountNumber}</div>
          )}
        </div>
        <div className={fieldClass}>
          <label className={labelClass}>کد اقتصادی (اختیاری):</label>
          <input
            name="economicCode"
            value={values.economicCode}
            onChange={handleChange}
            className={inputClass}
          />
          {errors.economicCode && (
            <div className={errorClass}>{errors.economicCode}</div>
          )}
        </div>
        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition"
        >
          ثبت
        </button>
      </form>
    </div>
  );
}
