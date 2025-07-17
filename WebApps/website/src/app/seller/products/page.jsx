"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import Swal from "sweetalert2";

export default function ProductPage() {
  const handleDelete = (item) => {
    Swal.fire({
      title: "آیا مطمئن هستید؟",
      text: "شما قادر به بازگرداندن این محصول نخواهید بود!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "بله، حذف کن!",
      cancelButtonText: "لغو",
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire("حذف شد!", "محصول شما حذف گردید.", "success");
      }
    });
  };

  const columns = [
    {
      key: "name",
      label: "نام محصول",
    },
    {
      key: "image",
      label: "تصویر محصول",
      render: (item) => (
        <img
          src={item.image}
          alt={item.name}
          className="w-16 h-16 object-cover rounded"
        />
      ),
    },
    {
      key: "datePublished",
      label: "تاریخ انتشار",
    },
    {
      key: "items",
      label: "تنوع ها",
      render: (item) => (
        <ul>
          {item.items.map((variant) => (
            <li key={variant.id}>
              {variant.property} - {variant.price} تومان
            </li>
          ))}
        </ul>
      ),
    },
    {
      key: "actions",
      label: "عملیات",
      render: (item) => (
        <div className="flex gap-2">
          <Link
            href={`/seller/products/edit/${item.id}`}
            className="text-blue-500 hover:text-blue-700"
          >
            ویرایش
          </Link>
          <button
            className="text-red-500 hover:text-red-700"
            onClick={() => handleDelete(item)}
          >
            حذف
          </button>
        </div>
      ),
    },
  ];
  const data = [
    {
      id: 1,
      name: "محصول 1",
      image: "/product.png",
      datePublished: "2023-01-01",
      items: [
        { id: 1, property: "ویژگی 1", price: 800000 },
        { id: 2, property: "ویژگی 2", price: 900000 },
      ],
    },
    {
      id: 2,
      name: "محصول 2",
      image: "/product.png",
      datePublished: "2023-01-02",
      items: [
        { id: 1, property: "ویژگی 1", price: 700000 },
        { id: 2, property: "ویژگی 2", price: 600000 },
      ],
    },
  ];
  return (
    <div className="p-4 sm:p-6 lg:p-8  min-h-screen">
      <div className="max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-gray-800">مدیریت محصولات</h2>
          <Link
            href="/seller/products/add"
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg shadow hover:bg-indigo-700 transition-colors"
          >
            افزودن محصول جدید
          </Link>
        </div>
        <hr className="mb-6 " />
        <div className="bg-white  overflow-hidden">
          <Table data={data} columns={columns} />
        </div>
      </div>
    </div>
  );
}
