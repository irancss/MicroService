import Table from "@components/General/Table";
import Link from "next/link";
import Image from "next/image";

export default function Brands() {
  const brands = [
    {
      id: 1,
      name: "برند 1",
      image: "/product.png",
      createdAt: "2023-01-01",
      status: "فعال",
      productCount: 10,
    },
    {
      id: 2,
      name: "برند 2",
      image: "/product.png",
      createdAt: "2023-02-01",
      status: "غیرفعال",
      productCount: 5,
    },
    {
      id: 3,
      name: "برند 3",
      image: "/product.png",
      createdAt: "2023-03-01",
      status: "فعال",
      productCount: 20,
    },
  ];

  const columns = [
    {  key: "image",
       label: "تصویر" ,
       render: (row) => (
        <div className="flex items-center text-center mx-auto gap-2">
          <Image
            src={row.image}
            alt={row.name}
            width={40}
            height={40}
            className="rounded-full"
          />
        </div>
      ),
     },
    {
      key: "name",
      label: "نام برند",
    },
    { key: "createdAt", label: "تاریخ ایجاد" },
    { key: "status", label: "وضعیت" },
    { key: "productCount", label: "تعداد محصولات" },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2">
          <Link
            href={`/admin/brands/edit/${row.id}`}
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
    <div >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت برندها
        </h2>
        <Link
          href="/admin/brands/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن برند جدید
        </Link>
      </div>
      <Table data={brands} columns={columns} />
    </div>
  );
}
