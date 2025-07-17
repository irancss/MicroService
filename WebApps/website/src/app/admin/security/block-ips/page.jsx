"use client";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";
import Table from "@components/General/Table";
import Link from "next/link";

export default function BlockedIPs() {
  const dummyData = [
    {
      id: 1,
      ip: "192.168.1.1",
      createdAt: "2023-01-01",
    },
    {
      id: 2,
      ip: "192.168.1.2",
      createdAt: "2023-01-02",
    },
    {
      id: 3,
      ip: "192.168.1.3",
      createdAt: "2023-01-03",
    },
    // Example: To test the empty state, comment out the above entries or use an empty array.
    // const blockedIPs = [];
  ];

  const columns = [
    { key: "id", label: "شناسه" },
    { key: "ip", label: "آدرس IP" },
    { key: "createdAt", label: "تاریخ مسدود شدن" },
    {
      key: "actions",
      label: "عملیات",
      render: (
        item // 'item' refers to the row data object
      ) => (
        <Link
          href={`/admin/security/block-ips/${item.id}`}
          className="text-blue-600 hover:underline"
        >
          جزئیات
        </Link>
      ),
    },
  ];

  const filtersConfig = [
    {
      name: "ip",
      label: "آدرس IP",
      type: "text",
      placeholder: "جستجوی آدرس IP",
    },
    {
      name: "createdAt",
      label: "تاریخ مسدود شدن",
      type: "date",
    },
  ];

  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = ""; // Initialize all filters to empty
    return acc;
  }, {});

  const { filters, handleFilterChange, filteredData, resetFilters } =
    useTableFilter(initialFilters, dummyData);

  return (
    <div className="container mx-auto p-4">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
        <h1 className="text-2xl font-bold">آدرس‌های IP مسدود شده</h1>
        <Link
          href="/admin/security/block-ips/new"
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded transition duration-150 ease-in-out"
        >
          مسدود کردن IP جدید
        </Link>
      </div>

      <TableFilter
        filtersConfig={filtersConfig}
        filters={filters}
        onFilterChange={handleFilterChange}
      />

      {filteredData.length > 0 ? (
        <>
          <p className="mb-4 text-gray-700">
            تعداد کل IP های مسدود شده: {filteredData.length}
          </p>
          <Table columns={columns} data={filteredData} />
        </>
      ) : (
        <div className="text-center py-10 border rounded-lg shadow-sm bg-gray-50">
          <svg
            className="mx-auto h-12 w-12 text-gray-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            aria-hidden="true"
          >
            <path
              vectorEffect="non-scaling-stroke"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <h3 className="mt-2 text-xl font-medium text-gray-900">
            هیچ آدرس IP مسدود شده‌ای وجود ندارد.
          </h3>
          <p className="mt-1 text-sm text-gray-500">
            با مسدود کردن اولین IP شروع کنید.
          </p>
          <div className="mt-6">
            <Link
              href="/admin/security/block-ips/new"
              className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 transition duration-150 ease-in-out"
            >
              مسدود کردن IP جدید
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}
