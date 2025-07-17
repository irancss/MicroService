"use client";
import React from "react";
import Table from "@components/General/Table";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";

export default function BlockUsersPage() {
  const columns = [
    { key: "username", label: "نام کاربری" },
    { key: "phoneNumber", label: "شماره تلفن" },
    { key: "blockReason", label: "دلیل مسدودسازی" },
    { key: "blockDate", label: "تاریخ مسدودسازی" },
    { key: "unblockDate", label: "تاریخ رفع مسدودیت" },
    { key: "status", label: "وضعیت" },
  ];
  const dummyData = [
    {
      key: "1",
      username: "user1",
      phoneNumber: "09123456789",
      blockReason: "نقض قوانین",
      blockDate: "1402/01/15",
      unblockDate: "1402/02/15",
      status: "مسدود شده",
    },
    {
      key: "2",
      username: "user2",
      phoneNumber: "09123456780",
      blockReason: "رفتار نامناسب",
      blockDate: "1402/01/20",
      unblockDate: "1402/02/20",
      status: "مسدود شده",
    },
  ];

  const filtersConfig = [
    {
      name: "username",
      label: "نام کاربری",
      type: "text",
      placeholder: "جستجوی نام کاربری",
    },
    {
      name: "phoneNumber",
      label: "شماره تلفن",
      type: "text",
      placeholder: "جستجوی شماره تلفن",
    },
    {
      name: "blockReason",
      label: "دلیل مسدودسازی",
      type: "text",
      placeholder: "دلیل مسدودسازی",
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
    {
      name: "status",
      label: "وضعیت",
      type: "select",
      options: [{ value: "مسدود شده", label: "مسدود شده" }],
    },
  ];
  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = "";
    return acc;
  }, {});

  const { filters, handleFilterChange, filteredData, resetFilters } =
    useTableFilter(initialFilters, dummyData);
  return (
    <div className="p-4">
        <div className="flex justify-between items-center mb-4">
             <h1 className="text-2xl font-bold mb-4">مدیریت کاربران مسدود شده</h1>

          <button
            onClick={resetFilters}
            className="text-blue-600 hover:text-blue-800 text-sm"
          >
            حذف فیلترها
          </button>
        </div>

        <TableFilter
          filtersConfig={filtersConfig}
          filters={filters}
          onFilterChange={handleFilterChange}
        />
      <Table columns={columns} data={filteredData} />
    </div>

  );
}
