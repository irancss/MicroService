"use client";
import { useState } from "react";
import Table from "@components/General/Table";
import Swal from "sweetalert2";

export default function DiscountCodes() {
  const [codes, setCodes] = useState([
    {
      id: 1,
      code: "DISCOUNT10",
      type: "percent",
      percent: 10,
      discountAmount: 1000,
      description: "تخفیف 10 درصدی برای خرید اول",
      usage: 50,
      maxUsage: 100,
      minimumPurchase: 20000,
      maximumPurchase: 500000,
      startDate: "2023-01-01",
      endDate: "2023-12-31",
      status: "فعال",
    },
    {
      id: 2,
      code: "FREESHIP",
      type: "fixed",
      percent: null,
      discountAmount: 5000,
      description: "ارسال رایگان برای خرید بالای 100 هزار تومان",
      usage: 30,
      maxUsage: 50,
      minimumPurchase: 100000,
      maximumPurchase: 1000000,
      startDate: "2023-02-01",
      endDate: "2023-11-30",
      status: "فعال",
    },
    {
      id: 3,
      code: "SUMMER20",
      type: "percent",
      percent: 20,
      discountAmount: 2000,
      description: "تخفیف تابستانه 20 درصدی",
      usage: 10,
      maxUsage: 20,
      minimumPurchase: 50000,
      startDate: "2023-06-01",
      endDate: "2023-08-31",
      status: "غیرفعال",
    },
  ]);

  const columns = [
    { key: "code", label: "کد تخفیف" },
    {
      key: "type",
      label: "نوع کد تخفیف",
      render: (item) => {
        // Assuming 'item' is the row object based on other renderers
        return item.type === "fixed" ? "ثابت" : "درصدی";
      },
    },
    { key: "discount", label: "تخفیف" },
    {
      key: "type",
      label: "مبالغ تخفیف",
      render: (item) => {
        // 'item' is the full row object
        if (item.type === "percent") {
          return `${item.discount}%`;
        } else if (item.type === "fixed") {
          // Add handling for 'fixed' type
          return `${item.discount} تومان`;
        }
        return ""; // Fallback for other types or if item.type is undefined
      },
    },
    { key: "description", label: "توضیحات" },
    { key: "usage", label: "مصرف" },
    { key: "maxUsage", label: "محدودیت" },
    { key: "minimumPurchase", label: "حداقل خرید" },
    { key: "maximumPurchase", label: "حداکثر خرید" },
    {
      key: "startDate",
      label: "تاریخ شروع",
      render: (codes) => {
        return new Date(codes.startDate).toLocaleDateString("fa-IR");
      },
    },
    {
      key: "endDate",
      label: "تاریخ پایان",
      render: (codes) => {
        return new Date(codes.endDate).toLocaleDateString("fa-IR");
      },
    },
    { key: "status", label: "وضعیت" },
  ];

  return (
    <>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت کدهای تخفیف
        </h2>
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          onClick={() => {
            Swal.fire({
              title: "افزودن کد تخفیف جدید",
              html: `
                <div class="mb-3">
                  <label for="code" class="block text-sm font-medium text-gray-700 mb-1">کد تخفیف</label>
                  <input type="text" id="code" class="swal2-input w-full" placeholder="کد تخفیف">
                </div>
                <div class="mb-3">
                  <label for="discount" class="block text-sm font-medium text-gray-700 mb-1">مقدار تخفیف</label>
                  <input type="number" id="discount" class="swal2-input w-full" placeholder="مقدار تخفیف">
                </div>
                <div class="mb-3">
                  <label for="type" class="block text-sm font-medium text-gray-700 mb-1">نوع کد تخفیف</label>
                  <select id="type" class="swal2-input w-full">
                    <option value="percent">درصدی</option>
                    <option value="fixed">ثابت</option>
                  </select>
                  <div class="mt-2 text-sm text-gray-500">نوع کد تخفیف را انتخاب کنید.</div>
                </div>
                <div class="mb-3">
                  <label for="usage" class="block text-sm font-medium text-gray-700 mb-1">مصرف</label>
                  <input type="number" id="usage" class="swal2-input w-full" placeholder="مصرف">
                </div>
                <div class="mb-3">
                  <label for="maxUsage" class="block text-sm font-medium text-gray-700 mb-1">حداکثر مصرف</label>
                  <input type="number" id="maxUsage" class="swal2-input w-full" placeholder="حداکثر مصرف">
                </div>

                <div class="mb-3">
                  <label for="minimumPurchase" class="block text-sm font-medium text-gray-700 mb-1">حداقل خرید</label>
                  <input type="number" id="minimumPurchase" class="swal2-input w-full" placeholder="حداقل خرید">
                </div>
                <div class="mb-3">
                  <label for="maximumPurchase" class="block text-sm font-medium text-gray-700 mb-1">حداکثر خرید</label>
                  <input type="number" id="maximumPurchase" class="swal2-input w-full" placeholder="حداکثر خرید">
                </div>
              <div class="mb-3">
                <label for="startDate" class="block text-sm font-medium text-gray-700 mb-1">تاریخ شروع</label>
                <input type="date" id="startDate" class="swal2-input w-full">
              </div>
              <div class="mb-3">
                <label for="endDate" class="block text-sm font-medium text-gray-700 mb-1">تاریخ پایان</label>
                <input type="date" id="endDate" class="swal2-input w-full">
              </div>
              <div class="mb-3">
                <label for="description" class="block text-sm font-medium text-gray-700 mb-1">توضیحات</label>
                <textarea id="description" class="swal2-textarea w-full" placeholder="توضیحات"></textarea>
              </div>
              `,
              focusConfirm: false,
              preConfirm: () => {
                const code = document.getElementById("code").value;
                const discount = document.getElementById("discount").value;
                const startDate = document.getElementById("startDate").value;
                const endDate = document.getElementById("endDate").value;

                if (!code || !discount || !startDate || !endDate) {
                  Swal.showValidationMessage("لطفاً همه فیلدها را پر کنید");
                  return;
                }

                const newCode = {
                  id: codes.length + 1,
                  code,
                  discount: parseFloat(discount),
                  startDate,
                  endDate,
                  status:
                    startDate < endDate &&
                    usage < maxUsage &&
                    endDate < new Date()
                      ? "فعال"
                      : "غیرفعال",
                };

                setCodes((prevCodes) => [...prevCodes, newCode]);
              },
            });
          }}
        >
          افزودن کد تخفیف جدید
        </button>
      </div>
      <hr className="border-b border-blue-200 mb-6" />
      <Table columns={columns} data={codes} />
    </>
  );
}
