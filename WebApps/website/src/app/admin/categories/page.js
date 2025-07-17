import Table from "@components/General/Table";
import Link from "next/link";

export default function Categories() {
  const categories = [
    { id: 1, name: "دسته بندی 1", slug: "category-1", parent: "بدون والد" },
    { id: 2, name: "دسته بندی 2", slug: "category-2", parent: "بدون والد" },
    { id: 3, name: "دسته بندی 3", slug: "category-3", parent: "بدون والد" },
  ];

  const columns = [
    { key: "id", label: "ID" },
    { key: "name", label: "نام دسته بندی" },
    { key: "slug", label: "اسلاگ" },
    { key: "parent", label: "والد" },
  ];

return (
    <div >
        <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
                مدیریت دسته بندی ها
            </h2>
            <Link href="/admin/categories/add" className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition">
                افزودن دسته بندی جدید
            </Link>
        </div>
        <Table data={categories} columns={columns} />
    </div>
);
}
