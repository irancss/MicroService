"use client";
import React from "react";
import Table from "@components/General/Table";
import Input from "@components/General/Input";

export default function VendorsReportPage() {
  const columns = [
    { key: "name", label: "نام فروشنده / فروشگاه" },
    { key: "membershipDate", label: "تاریخ عضویت" },
    { key: "activeProducts", label: "تعداد محصولات فعال" },
    { key: "totalSales", label: "تعداد کل فروش‌ها" },
    { key: "totalRevenue", label: "مجموع درآمد" },
    { key: "commissionPaid", label: "کمیسیون پرداخت‌شده" },
    { key: "averageRating", label: "امتیاز کاربران (میانگین)" },
    { key: "returnedOrders", label: "سفارشات برگشتی" },
    {
      key: "vendorStatus",
      label: "وضعیت فروشنده (فعال / غیرفعال / در انتظار تأیید)",
    },
  ];

  const dummyData = [
    {
      key: "1",
      name: "فروشنده A",
      membershipDate: "1402/01/15",
      activeProducts: 25,
      totalSales: 150,
      totalRevenue: "5,000,000 تومان",
      commissionPaid: "500,000 تومان",
      averageRating: 4.5,
      returnedOrders: 5,
      vendorStatus: "فعال",
    },
    {
      key: "2",
      name: "فروشنده B",
      membershipDate: "1401/12/10",
      activeProducts: 10,
      totalSales: 80,
      totalRevenue: "2,500,000 تومان",
      commissionPaid: "250,000 تومان",
      averageRating: 3.8,
      returnedOrders: 2,
      vendorStatus: "غیرفعال",
    },
  ];

  const [filters, setFilters] = React.useState({
    startDate: "",
    endDate: "",
    vendorName: "",
    minRevenue: "",
    maxRevenue: "",
    minRating: "",
    maxRating: "",
    status: "",
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
      if (filters.vendorName && !item.name.includes(filters.vendorName)) {
        passes = false;
      }
      // Note: Date comparison might need a robust library for production if dates are in different formats or timezones.
      // For "YYYY/MM/DD" format, direct string comparison might work for some cases but new Date() is safer.
      if (
        filters.startDate &&
        item.membershipDate.replace(/\//g, "") <
          filters.startDate.replace(/-/g, "")
      ) {
        passes = false;
      }
      if (
        filters.endDate &&
        item.membershipDate.replace(/\//g, "") >
          filters.endDate.replace(/-/g, "")
      ) {
        passes = false;
      }
      if (
        filters.minRevenue &&
        parseInt(item.totalRevenue.replace(/[^0-9]/g, "")) <
          parseInt(filters.minRevenue)
      ) {
        passes = false;
      }
      if (
        filters.maxRevenue &&
        parseInt(item.totalRevenue.replace(/[^0-9]/g, "")) >
          parseInt(filters.maxRevenue)
      ) {
        passes = false;
      }
      if (
        filters.minRating &&
        item.averageRating < parseFloat(filters.minRating)
      ) {
        passes = false;
      }
      if (
        filters.maxRating &&
        item.averageRating > parseFloat(filters.maxRating)
      ) {
        passes = false;
      }
      if (filters.status && item.vendorStatus !== filters.status) {
        passes = false;
      }
      return passes;
    });
  }, [filters, dummyData]);

  return (
    <div className="p-6  min-h-screen">
      <div className=" rounded-lg p-6 mb-8">
        <h2 className="text-xl font-semibold mb-6 text-gray-700 border-b pb-3">
          فیلترها
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div>
            <label
              htmlFor="vendorName"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              نام فروشنده:
            </label>
            <input
              type="text"
              id="vendorName"
              value={filters.vendorName}
              onChange={handleFilterChange}
              name="vendorName"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>
          <div>
            <label
              htmlFor="startDate"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              تاریخ عضویت (از):
            </label>
            <input
              type="date"
              id="startDate"
              value={filters.startDate}
              onChange={handleFilterChange}
              name="startDate"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>
          <div>
            <label
              htmlFor="endDate"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              تاریخ عضویت (تا):
            </label>
            <input
              type="date"
              id="endDate"
              value={filters.endDate}
              onChange={handleFilterChange}
              name="endDate"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>
          <div>
            <label
              htmlFor="minRevenue"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              حداقل درآمد:
            </label>
            <input
              type="number"
              id="minRevenue"
              value={filters.minRevenue}
              onChange={handleFilterChange}
              name="minRevenue"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
              placeholder="مثال: 1000000"
            />
          </div>
          <div>
            <label
              htmlFor="maxRevenue"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              حداکثر درآمد:
            </label>
            <input
              type="number"
              id="maxRevenue"
              value={filters.maxRevenue}
              onChange={handleFilterChange}
              name="maxRevenue"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
              placeholder="مثال: 5000000"
            />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label
                htmlFor="minRating"
                className="block text-sm font-medium text-gray-600 mb-1"
              >
                حداقل امتیاز:
              </label>
              <input
                type="number"
                id="minRating"
                step="0.1"
                value={filters.minRating}
                onChange={handleFilterChange}
                name="minRating"
                className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="مثال: 3.0"
              />
            </div>
            <div>
              <label
                htmlFor="maxRating"
                className="block text-sm font-medium text-gray-600 mb-1"
              >
                حداکثر امتیاز:
              </label>
              <input
                type="number"
                id="maxRating"
                step="0.1"
                value={filters.maxRating}
                onChange={handleFilterChange}
                name="maxRating"
                className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="مثال: 5.0"
              />
            </div>
          </div>
          <div>
            <label
              htmlFor="status"
              className="block text-sm font-medium text-gray-600 mb-1"
            >
              وضعیت فروشنده:
            </label>
            <select
              id="status"
              value={filters.status}
              onChange={(e) =>
                setFilters({ ...filters, status: e.target.value })
              }
              name="status"
              className="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500"
            >
              <option value="">همه</option>
              <option value="فعال">فعال</option>
              <option value="غیرفعال">غیرفعال</option>
              <option value="در انتظار تأیید">در انتظار تأیید</option>
            </select>
          </div>
        </div>
      </div>

      <div className="rounded-lg p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800">گزارش فروشندگان</h1>
          <button
            onClick={() => {
              alert(
                "دکمه 'ارسال هشدار به فروشنده' کلیک شد. برای عملکرد کامل، نیاز به انتخاب فروشنده و منطق ارسال هشدار است."
              );
            }}
            className="bg-yellow-500 hover:bg-yellow-700 text-white font-bold py-2 px-4 rounded"
          >
            ارسال هشدار به فروشنده
          </button>
        </div>
        <Table columns={columns} data={filteredData} />
      </div>
    </div>
  );
}
