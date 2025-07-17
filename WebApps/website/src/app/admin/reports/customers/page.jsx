"use client";
import React from "react";
import Table from "@components/General/Table";

export default function ProductsReportPage() {
  const columns = [
    { key: "userName", label: "نام کاربر" },
    { key: "phoneNumber", label: "شماره تماس" },
    { key: "registrationDate", label: "تاریخ عضویت" },
    { key: "totalOrders", label: "تعداد کل سفارشات" },
    { key: "totalSpent", label: "مجموع خرید" },
    { key: "averageCartValue", label: "متوسط سبد خرید" },
    { key: "lastOrderDate", label: "تاریخ آخرین سفارش" },
    { key: "status", label: "وضعیت" },
    {
      key: "acquisitionChannel",
      label: "کانال جذب",
    },
  ];

  const dummyData = [
    {
      key: "1",
      userName: "کاربر A",
      phoneNumber: "09123456789",
      registrationDate: "1402/01/15",
      totalOrders: 10,
      totalSpent: "1,000,000 تومان",
      averageCartValue: "100,000 تومان",
      lastOrderDate: "1402/02/10",
      status: "فعال",
      acquisitionChannel: "گوگل",
    },
    {
      key: "2",
      userName: "کاربر B",
      phoneNumber: "09123456780",
      registrationDate: "1401/12/10",
      totalOrders: 5,
      totalSpent: "500,000 تومان",
      averageCartValue: "100,000 تومان",
      lastOrderDate: "1402/01/20",
      status: "غیرفعال",
      acquisitionChannel: "ارجاع",
    },
  ];

  const [filters, setFilters] = React.useState({
    startDate: "",
    endDate: "",
    userName: "",
    phoneNumber: "",
    totalOrders: "",
    totalSpent: "",
    averageCartValue: "",
    lastOrderDate: "",
    status: "",
    acquisitionChannel: "",
    membershipDuration: "",
    purchaseAmount: "",
    lastOrder: "",
    inactiveUsers: "",
    loyalCustomers: "",
  });
  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters((prevFilters) => ({
      ...prevFilters,
      [name]: value,
    }));
  };
  const filteredData = React.useMemo(() => {
    return dummyData.filter((item) => {
          let passes = true;
        if (filters.userName && !item.userName.includes(filters.userName)) {
            passes = false;
            }
        if (filters.phoneNumber && !item.phoneNumber.includes(filters.phoneNumber)) {
            passes = false;
          }
        if (filters.startDate) {
            const itemDate = new Date(item.registrationDate); // Note: JavaScript's Date object may not correctly parse Jalali dates like "1402/01/15".
            const startDate = new Date(filters.startDate);
            if (itemDate < startDate) {
              passes = false;
            }
          }
        if (filters.endDate) {
            const itemDate = new Date(item.registrationDate); // Note: JavaScript's Date object may not correctly parse Jalali dates.
            const endDate = new Date(filters.endDate);
            if (itemDate > endDate) {
              passes = false;
            }
          }
        if (filters.totalOrders && item.totalOrders < parseInt(filters.totalOrders, 10)) {
            passes = false;
          }
        if (filters.totalSpent && parseInt(item.totalSpent.replace(/[^0-9]/g, ""), 10) < parseInt(String(filters.totalSpent).replace(/[^0-9]/g, ""), 10)) {
            passes = false;
          }
        if (filters.averageCartValue && parseInt(item.averageCartValue.replace(/[^0-9]/g, ""), 10) < parseInt(String(filters.averageCartValue).replace(/[^0-9]/g, ""), 10)) {
            passes = false;
          }
        if (filters.lastOrderDate) {
            const itemDate = new Date(item.lastOrderDate); // Note: JavaScript's Date object may not correctly parse Jalali dates.
            const lastOrderDateFilter = new Date(filters.lastOrderDate);
            if (itemDate < lastOrderDateFilter) {
              passes = false;
            }
          }
        if (filters.status && item.status !== filters.status) {
            passes = false;
          }
        if (filters.acquisitionChannel && item.acquisitionChannel !== filters.acquisitionChannel) {
            passes = false;
          }
        if (filters.membershipDuration && !item.registrationDate.includes(filters.membershipDuration)) {
            // This filter logic might not be as intended given the placeholder "مدت عضویت (مثلا 6 ماه)".
            // It currently performs a string inclusion check.
            passes = false;
          }
        if (filters.purchaseAmount && parseInt(item.totalSpent.replace(/[^0-9]/g, ""), 10) < parseInt(String(filters.purchaseAmount).replace(/[^0-9]/g, ""), 10)) {
            passes = false;
          }
        if (filters.lastOrder) {
            const itemDate = new Date(item.lastOrderDate); // Note: JavaScript's Date object may not correctly parse Jalali dates.
            const filterDate = new Date(filters.lastOrder);
            if (itemDate < filterDate) {
              passes = false;
            }
          }
        if (filters.inactiveUsers && item.status !== "غیرفعال") {
            passes = false;
          }
        if (filters.loyalCustomers && item.status !== "فعال") {
            passes = false;
          }
        return passes;
    });
  }, [filters]);

  return (
    <div className="">
      <div className=" mb-8">
        <h2 className="text-xl font-semibold mb-4 text-gray-700">فیلترها</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
          <input
            type="text"
            name="userName"
            placeholder="نام کاربر"
            value={filters.userName}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="text"
            name="phoneNumber"
            placeholder="شماره تماس"
            value={filters.phoneNumber}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="date"
            name="startDate"
            value={filters.startDate}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
            placeholder="تاریخ شروع"
          />
          <input
            type="date"
            name="endDate"
            value={filters.endDate}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
            placeholder="تاریخ پایان"
          />
          <input
            type="number"
            name="totalOrders"
            placeholder="تعداد کل سفارشات"
            value={filters.totalOrders}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="text"
            name="totalSpent"
            placeholder="مجموع خرید"
            value={filters.totalSpent}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="text"
            name="averageCartValue"
            placeholder="متوسط سبد خرید"
            value={filters.averageCartValue}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="date"
            name="lastOrderDate"
            value={filters.lastOrderDate}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
            placeholder="تاریخ آخرین سفارش"
          />
          <select
            name="status"
            value={filters.status}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">وضعیت: همه</option>
            <option value="فعال">فعال</option>
            <option value="غیرفعال">غیرفعال</option>
          </select>
          <select
            name="acquisitionChannel"
            value={filters.acquisitionChannel}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">کانال جذب: همه</option>
            <option value="گوگل">گوگل</option>
            <option value="ارجاع">ارجاع</option>
            <option value="شبکه‌های اجتماعی">شبکه‌های اجتماعی</option>
            <option value="ایمیل">ایمیل</option>
            <option value="سایر">سایر</option>
          </select>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {/* The following options seem to be misplaced or part of an incomplete select element.
              They are removed as they don't fit the current structure.
              If they belong to a new filter, a new select element should be created for them.
            <option value="social_media">شبکه‌های اجتماعی</option>
            <option value="email">ایمیل</option>
            <option value="other">سایر</option>
          */}
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <input
            type="text"
            name="membershipDuration"
            placeholder="مدت عضویت (مثلا 6 ماه)"
            value={filters.membershipDuration}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="text"
            name="purchaseAmount"
            placeholder="میزان خرید (مثلا 1,000,000 تومان)"
            value={filters.purchaseAmount}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="date"
            name="lastOrder"
            value={filters.lastOrder}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
            placeholder="آخرین سفارش"
          />
          <input
            type="text"
            name="inactiveUsers"
            placeholder="تعداد کاربران غیرفعال"
            value={filters.inactiveUsers}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
          <input
            type="text"
            name="loyalCustomers"
            placeholder="تعداد مشتریان وفادار"
            value={filters.loyalCustomers}
            onChange={handleFilterChange}
            className="border p-2 rounded-md w-full focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      <div className=" ">
        <h1 className="text-2xl font-semibold mb-6 text-gray-800">
          گزارش کاربران
        </h1>
        <Table columns={columns} data={filteredData} />
      </div>
    </div>
  );
}
