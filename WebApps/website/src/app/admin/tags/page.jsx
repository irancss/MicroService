"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import { useEffect, useState } from "react";
export default function ApproveProducts() {
  //try Fetch Data from API if not available use dummy data
  const dummyData = [
    {
      id: 1,
      Name: "برچسب 1",
      Url: "https://example.com/tag/tag1",
      DateCreated: "1402/01/01",
      DateUpdated: "1402/01/02",
      render: () => (
        <div className="flex justify-center items-center space-x-2">
          <button className="text-blue-500 hover:text-blue-700">ویرایش</button>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
    {
      id: 2,
      Name: "برچسب 2",
      Url: "https://example.com/tag/tag2",
      DateCreated: "1402/01/03",
      DateUpdated: "1402/01/04",
      render: () => (
        <div className="flex justify-center items-center space-x-2">
          <button className="text-blue-500 hover:text-blue-700">ویرایش</button>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
    {
      id: 3,
      Name: "برچسب 3",
      Url: "https://example.com/tag/tag3",
      DateCreated: "1402/01/05",
      DateUpdated: "1402/01/06",
      render: (row) => (
        <div className="flex gap-2 text-center">
          <Link
            href={`/admin/tags/edit/${row.id}`}
            className="text-blue-500 hover:text-blue-700"
          >
            ویرایش
          </Link>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
  ];

  const [data, setData] = useState(dummyData);

  useEffect(() => {
    fetch("/api/products")
      .then((res) => {
        if (!res.ok) throw new Error("Network response was not ok");
        return res.json();
      })
      .then((apiData) => {
        // Map API data to match table structure if needed
        setData(
          apiData.map((item) => ({
            ...item,
            render: () => (
              <div className="flex justify-center items-center space-x-2">
                <button className="text-blue-500 hover:text-blue-700">
                  ویرایش
                </button>
                <button className="text-red-500 hover:text-red-700">حذف</button>
              </div>
            ),
          }))
        );
      })
      .catch(() => {
        setData(dummyData);
      });
  }, []);

  const columns = [
    { key: "Name", label: "نام" },
    { key: "Url", label: "آدرس" },
    { key: "count", label: "تعداد محصولات" },
    { key: "DateCreated", label: "تاریخ ایجاد" },
    { key: "DateUpdated", label: "تاریخ به روز رسانی" },
    {
      key: "Actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2 text-center">
          <Link
            href={`/admin/tags/edit/${row.id}`}
            className="text-blue-500 hover:text-blue-700"
          >
            ویرایش
          </Link>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
  ];

  return (
    <>
      <div >
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
            برچسب ها
          </h2>
          <Link
            href="/admin/tags/add"
            className="text-white bg-blue-500 hover:bg-blue-600 px-4 py-2 rounded-lg"
          >
            افزودن برچسب جدید
          </Link>
        </div>
        <Table data={data} columns={columns} />
      </div>
    </>
  );
}
