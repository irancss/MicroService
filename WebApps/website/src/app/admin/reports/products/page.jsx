"use client";
import Table from "@components/General/Table";
import React from "react";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";

export default function ProductsReportPage() {
  const dummyData = [
    {
      key: "1",
      productName: "محصول A",
      category: "دسته‌بندی 1",
      brand: "برند X",
      totalSales: 150,
      views: 1200,
      conversionRate: "12.5%",
      currentPrice: "$50",
      netProfit: "$2000",
      stockStatus: "موجود",
      seller: "فروشنده 1",
      activeStatus: "فعال",
      lastUpdated: "2023-10-01",
    },
    {
      key: "2",
      productName: "محصول B",
      category: "دسته‌بندی 2",
      brand: "برند Y",
      totalSales: 80,
      views: 600,
      conversionRate: "13.3%",
      currentPrice: "$30",
      netProfit: "$1000",
      stockStatus: "ناموجود",
      seller: "فروشنده 2",
      activeStatus: "غیرفعال",
      lastUpdated: "2023-09-15",
    },
  ];
  const columns = [
    { label: "نام محصول", key: "productName" },
    { label: "دسته‌بندی", key: "category" },
    { label: "برند", key: "brand" },
    { label: "تعداد کل فروش", key: "totalSales" },
    { label: "تعداد بازدید", key: "views" },
    { label: "نرخ تبدیل (CTR)", key: "conversionRate" },
    { label: "قیمت فعلی", key: "currentPrice" },
    { label: "سود خالص", key: "netProfit" },
    { label: " موجودی", key: "stockStatus" },
    { label: "فروشنده ", key: "seller" },
    { label: "وضعیت", key: "activeStatus" },
    {
      label: "به‌روزرسانی",
      key: "lastUpdated",
      render: (row) => new Date(row.lastUpdated).toLocaleDateString("fa-IR"),
    },
  ];

  const [filters, setFilters] = React.useState({
    category: "",
    brand: "",
    seller: "",
    startDate: "",
    endDate: "",
    minSales: "",
    maxSales: "",
  });

  const uniqueCategories = Array.from(
    new Set(dummyData.map((item) => item.category))
  );
  const uniqueBrands = Array.from(new Set(dummyData.map((item) => item.brand)));
  const uniqueSellers = Array.from(
    new Set(dummyData.map((item) => item.seller))
  );
  const filtersConfig = [
    {
      name: "category",
      label: "دسته‌بندی",
      type: "select",
      options: uniqueCategories.map((category) => ({
        value: category,
        label: category,
      })),
    },
    {
      name: "brand",
      label: "برند",
      type: "select",
      options: uniqueBrands.map((brand) => ({
        value: brand,
        label: brand,
      })),
    },
    {
      name: "seller",
      label: "فروشنده",
      type: "select",
      options: uniqueSellers.map((seller) => ({
        value: seller,
        label: seller,
      })),
    },
    {
      name: "startDate",
      label: "تاریخ شروع",
      type: "date",
    },
    {
      name: "endDate",
      label: "تاریخ پایان",
      type: "date",
    },
    {
      name: "minSales",
      label: "حداقل فروش",
      type: "number",
    },
    {
      name: "maxSales",
      label: "حداکثر فروش",
      type: "number",
    },
  ];

  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = "";
    return acc;
  }, {});

  const { filteredData, handleFilterChange, resetFilters } = useTableFilter(
    initialFilters,
    dummyData
  );

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-6 text-center">گزارش محصولات</h1>
      <TableFilter
        filters={filters}
        setFilters={setFilters}
        filtersConfig={filtersConfig}
        resetFilters={resetFilters}
      />
      <div>
        <h2 className="text-xl font-semibold mb-4">نتایج فیلتر شده</h2>
        <Table columns={columns} data={filteredData} rowsPerPage={10} />
      </div>
    </div>
  );
}
