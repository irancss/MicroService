"use client";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";
import Table from "@components/General/Table";
import Link from "next/link";

export default function SuspiciousActivityPage() {
  const columns = [
    { key: "id", label: "شناسه" },
    { key: "activityType", label: "نوع فعالیت" },
    { key: "user", label: "کاربر" },
    { key: "ipAddress", label: "آدرس IP" },
    { key: "timestamp", label: "زمان" },
    {
      key: "actions",
      label: "عملیات",
      render: (item) => (
        <Link
          href={`/admin/security/suspicious-activity/${item.id}`}
          className="text-blue-600 hover:underline"
        >
          جزئیات
        </Link>
      ),
    },
  ];

  const dummyData = [
    {
      id: 1,
      activityType: "ورود مشکوک",
      user: "user1",
      ipAddress: "192.168.1.1",
      timestamp: "2023-01-01",
    },
    {
      id: 2,
      activityType: "تلاش ورود ناموفق",
      user: "user2",
      ipAddress: "192.168.1.2",
      timestamp: "2023-01-02",
    },
    {
      id: 3,
      activityType: "تغییر رمز عبور",
      user: "user3",
      ipAddress: "192.168.1.3",
      timestamp: "2023-01-03",
    },
  ];

  const filtersConfig = [
    {
      name: "activityType",
      label: "نوع فعالیت",
      type: "text",
      placeholder: "جستجوی نوع فعالیت",
    },
    {
      name: "user",
      label: "کاربر",
      type: "text",
      placeholder: "جستجوی کاربر",
    },
    {
      name: "ipAddress",
      label: "آدرس IP",
      type: "text",
      placeholder: "جستجوی آدرس IP",
    },
    {
      name: "timestamp",
      label: "زمان",
      type: "date",
    },
  ];

  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = "";
    return acc;
  }, {});

  const { filters, handleFilterChange, filteredData, resetFilters } =
    useTableFilter(initialFilters, dummyData);

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">فعالیت‌های مشکوک</h1>

      <TableFilter
        filtersConfig={filtersConfig}
        filters={filters}
        onFilterChange={handleFilterChange}
      />

      {filteredData.length > 0 ? (
        <Table columns={columns} data={filteredData} />
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
            هیچ فعالیت مشکوکی وجود ندارد.
          </h3>
          <p className="mt-1 text-sm text-gray-500">
            با بررسی فعالیت‌های مشکوک شروع کنید.
          </p>
        </div>
      )}
    </div>
  );
}
