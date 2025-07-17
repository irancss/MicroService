"use client";
import React from "react";
import Table from "@components/General/Table";

export default function InventoryReportPage() {
  const columns = [
    { key: "productName", label: "نام محصول" },
    { key: "vendor", label: "فروشنده" },
    { key: "category", label: "دسته‌بندی" },
    { key: "currentStock", label: "موجودی فعلی" },
    { key: "minimumStock", label: "حداقل موجودی مجاز" },
    { key: "alertStatus", label: "وضعیت هشدار (اگر کمتر از حداقل)" },
    { key: "reservedStock", label: "موجودی رزرو شده" },
    { key: "lastUpdated", label: "تاریخ آخرین بروزرسانی" },
    { key: "storageLocation", label: "انبار / محل نگهداری" },
  ];

  const dummyData = [
    {
      key: "1",
      productName: "محصول A",
      vendor: "فروشنده A",
      category: "دسته‌بندی A",
      currentStock: 100,
      minimumStock: 50,
      alertStatus: "فعال",
      reservedStock: 20,
      lastUpdated: "1402/01/15",
      storageLocation: "انبار مرکزی",
    },
    {
      key: "2",
      productName: "محصول B",
      vendor: "فروشنده B",
      category: "دسته‌بندی B",
      currentStock: 30,
      minimumStock: 50,
      alertStatus: "غیرفعال",
      reservedStock: 5,
      lastUpdated: "1402/01/10",
      storageLocation: "انبار فرعی",
    },
  ];

  const [filters, setFilters] = React.useState({
    productName: "",
    vendor: "",
    category: "",
    minStock: "",
    maxStock: "",
    storageLocation: "",
    lastUpdated: "",
    alertStatus: "",
    reservedStock: "",
    minimumStock: "",
    maximumStock: "",
  });
  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const filteredData = React.useMemo(() => {
    return dummyData.filter((item) => {
      let passes = true;
      if (
        filters.productName &&
        !item.productName.includes(filters.productName)
      ) {
        passes = false;
      }
      if (filters.vendor && !item.vendor.includes(filters.vendor)) {
        passes = false;
      }
      if (filters.category && !item.category.includes(filters.category)) {
        passes = false;
      }
      if (filters.minStock && item.currentStock < parseInt(filters.minStock)) {
        passes = false;
      }
      if (filters.maxStock && item.currentStock > parseInt(filters.maxStock)) {
        passes = false;
      }
      if (
        filters.storageLocation &&
        !item.storageLocation.includes(filters.storageLocation)
      ) {
        passes = false;
      }
      if (filters.lastUpdated) {
        const itemDate = new Date(item.lastUpdated);
        const filterDate = new Date(filters.lastUpdated);
        if (itemDate < filterDate) {
          passes = false;
        }
      }
      if (filters.alertStatus && item.alertStatus !== filters.alertStatus) {
        passes = false;
      }
      if (
        filters.reservedStock &&
        item.reservedStock < parseInt(filters.reservedStock)
      ) {
        passes = false;
      }

      if (
        filters.minimumStock &&
        item.minimumStock < parseInt(filters.minimumStock)
      ) {
        passes = false;
      }
      if (
        filters.maximumStock &&
        item.currentStock > parseInt(filters.maximumStock)
      ) {
        passes = false;
      }

      return passes;
    });
  }, [filters]);

  return (
    <div className="">
      <h2 className="text-xl font-semibold mb-4 text-gray-700">گزارش موجودی</h2>
      <div className="mb-6 p-4   rounded-lg">
        <h3 className="text-lg font-medium text-gray-800 mb-3">فیلترها</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div>
            <label
              htmlFor="productName"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              نام محصول
            </label>
            <input
              type="text"
              name="productName"
              id="productName"
              value={filters.productName}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="vendor"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              فروشنده
            </label>
            <input
              type="text"
              name="vendor"
              id="vendor"
              value={filters.vendor}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="category"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              دسته‌بندی
            </label>
            <input
              type="text"
              name="category"
              id="category"
              value={filters.category}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="minStock"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              حداقل موجودی فعلی
            </label>
            <input
              type="number"
              name="minStock"
              id="minStock"
              value={filters.minStock}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="maxStock"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              حداکثر موجودی فعلی
            </label>
            <input
              type="number"
              name="maxStock"
              id="maxStock"
              value={filters.maxStock}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="storageLocation"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              انبار / محل نگهداری
            </label>
            <input
              type="text"
              name="storageLocation"
              id="storageLocation"
              value={filters.storageLocation}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="lastUpdated"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              تاریخ آخرین بروزرسانی (بعد از)
            </label>
            <input
              type="date"
              name="lastUpdated"
              id="lastUpdated"
              value={filters.lastUpdated}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="alertStatus"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              وضعیت هشدار
            </label>
            <select
              name="alertStatus"
              id="alertStatus"
              value={filters.alertStatus}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            >
              <option value="">همه</option>
              <option value="فعال">فعال</option>
              <option value="غیرفعال">غیرفعال</option>
            </select>
          </div>
          <div>
            <label
              htmlFor="reservedStock"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              حداقل موجودی رزرو شده
            </label>
            <input
              type="number"
              name="reservedStock"
              id="reservedStock"
              value={filters.reservedStock}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="minimumStock"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              حداقل موجودی مجاز (بزرگتر از)
            </label>
            <input
              type="number"
              name="minimumStock"
              id="minimumStock"
              value={filters.minimumStock}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          <div>
            <label
              htmlFor="maximumStock"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              حداکثر موجودی مجاز (کوچکتر از)
            </label>
            <input
              type="number"
              name="maximumStock"
              id="maximumStock"
              value={filters.maximumStock}
              onChange={handleFilterChange}
              className="mt-1 block w-full px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
        </div>
      </div>
      <Table columns={columns} data={filteredData} />
    </div>
  );
}
