import Table from "@components/General/Table";
import Link from "next/link";
import { useState } from "react";

export default function SchemaList() {
  const [schemas, setSchemas] = useState([
    {
      id: 1,
      name: "اسکیما محصول",
      type: "Product",
      location: "صفحه محصول",
      date: "1402/01/01",
    },
    {
      id: 2,
      name: "اسکیما مقاله",
      type: "Article",
      location: "صفحه مقاله",
      date: "1402/02/15",
    },
    {
      id: 3,
      name: "اسکیما دسته بندی",
      type: "Category",
      location: "صفحه دسته بندی",
      date: "1402/03/10",
    },
  ]);

  const columns = [
    { key: "name", label: "نام" },
    { key: "type", label: "نوع" },
    { key: "location", label: "محل موردنظر" },
    { key: "date", label: "تاریخ ایجاد" },
  ];

  return (
    <div className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت اسکیماها
        </h2>
        <Link
          href="/admin/settings/schema/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن اسکیما جدید
        </Link>
      </div>
      <Table columns={columns} data={schemas} />
    </div>
  );
}
