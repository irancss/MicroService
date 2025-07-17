"use client";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";
import Table from "@components/General/Table";
import Link from "next/link";

export default function UserLogsPage() {
  const columns = [
    { key: "id", label: "شناسه" },
    { key: "user", label: "کاربر" },
    { key: "action", label: "عملیات" },
    { key: "timestamp", label: "زمان" , // 2023-01-02T14:30:00Z
        render: (item) => {
          const date = new Date(item.timestamp);
          return <span>{date.toLocaleString("fa-IR")}</span>; 
        }
    },
    {
      key: "details",
      label: "جزئیات",
      render: (item) => (
        <Link
          href={`/admin/security/user-logs/${item.id}`}
          className="text-blue-600 hover:underline"
        >
          مشاهده
        </Link>
      ),
    },
  ];

  const dummyData = [
    {
      id: 1,
      user: "user1",
      action: "ورود به سیستم",
      timestamp: "2023-01-01T12:00:00Z",
    },
    {
      id: 2,
      user: "user2",
      action: "تغییر رمز عبور",
      timestamp: "2023-01-02T14:30:00Z",
    },
    {
      id: 3,
      user: "user3",
      action: "خروج از سیستم",
      timestamp: "2023-01-03T16:45:00Z",
    },
  ];

  const filtersConfig = [
    {
      name: "user",
      label: "کاربر",
      type: "text",
      placeholder: "جستجوی کاربر",
    },
    {
      name: "action",
      label: "عملیات",
      type: "text",
      placeholder: "جستجوی عملیات",
    },
    {
      name: "startDate",
      label: "از تاریخ",
      type: "date",
    },
    {
      name: "endDate",
      label: "تا تاریخ",
      type: "date",
    },
  ];



  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = ""; // Initialize all filters to empty
    return acc;
  }, {});

  const { filters, handleFilterChange, filteredData } = useTableFilter(
    initialFilters,
    dummyData
  );

  return (
    <div className="p-4">
        <h1 className="text-2xl font-bold mb-4">گزارشات لاگ کاربران</h1>
        <TableFilter
          filters={filters}
          onFilterChange={handleFilterChange}
          filtersConfig={filtersConfig}
          className="mb-4"
        />
      <Table columns={columns} data={filteredData} />
    </div>
  );
}
