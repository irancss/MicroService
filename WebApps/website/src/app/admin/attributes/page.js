import Table from "@components/General/Table";
import Link from "next/link";
import { set } from "ol/transform";
export default function AttributesPage() {
  const attributes = [
    {
      name: "رنگ",
      url: "/attributes/color",
      count: 120,
      subAttributes: [
        "قرمز",
        "آبی",
        "سبز",
        "زرد",
        "مشکی",
        "سفید",
        "نارنجی",
        "بنفش",
      ],
    },
    {
      name: "اندازه",
      url: "/attributes/size",
      count: 80,
      subAttributes: ["کوچک", "متوسط", "بزرگ"],
    },
    {
      name: "جنس",
      url: "/attributes/material",
      count: 50,
      subAttributes: ["پنبه", "پلی استر", "چرم"],
    },
  ];
  const columns = [
    { key: "name", label: "نام ویژگی" },
    { key: "url", label: "آدرس" },
    { key: "count", label: "تعداد محصولات" },
    { key: "subAttributes", label: "زیر مجموعه" },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2">
          <button className="text-blue-500 hover:text-blue-700">ویرایش</button>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
  ];
  return (
    <div >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت ویژگی ها
        </h2>
        <Link
          href="/admin/attributes/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن ویژگی جدید
        </Link>
      </div>
      <Table columns={columns} data={attributes} />
    </div>
  );
}
