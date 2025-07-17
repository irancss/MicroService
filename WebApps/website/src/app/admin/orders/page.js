import Table from "@components/General/Table";
import Link from "next/link";
export default function OrdersPage() {
  const DummyOrders = [
    {
      id: 1,
      name: "علی",
      date: "1402/01/01",
      status: "در حال پردازش",
      total: 100000,
      referral: "تبلیغات",
      barcode: "1234567890123",
    },
    {
      id: 2,
      name: "مریم",
      date: "1402/01/02",
      status: "تحویل داده شده",
      total: 200000,
      referral: "دوست",
      barcode: "1234567890124",
    },
    {
      id: 3,
      name: "حسین",
      date: "1402/01/03",
      status: "لغو شده",
      total: 150000,
      referral: "تبلیغات",
      barcode: "1234567890125",
    },
  ];
  const columns = [
    { key: "checkbox", label: "#", render: (row) => <input type="checkbox" /> },
    { key: "id", label: "شناسه" },
    { key: "name", label: "نام مشتری" },
    { key: "date", label: "تاریخ" },
    {
      key: "status",
      label: "وضعیت",
      render: (row) => (
        <span
          className={
            row.status === "لغو شده"
              ? "text-red-500 bg-red-100 px-3 py-2 rounded-xl"
              : row.status === "تحویل داده شده"
              ? "text-green-500 bg-green-100 px-3 py-2 rounded-xl"
              : row.status === "در حال پردازش"
              ? "text-yellow-500 bg-yellow-100 px-3 py-2 rounded-xl"
              : row.status === "در انتظار تایید"
              ? "text-blue-500 bg-blue-100 px-3 py-2 rounded-xl"
              : row.status === "در انتظار پرداخت"
              ? "text-purple-500 bg-purple-100 px-3 py-2 rounded-xl"
              : row.status === "در انتظار ارسال"
              ? "text-indigo-500 bg-indigo-100 px-3 py-2 rounded-xl"
              : ""
          }
        >
          {row.status}
        </span>
      ),
    },
    {
      key: "total",
      label: "مجموع",
      render: (row) => row.total.toLocaleString() + " تومان",
    },
    {
      key: "referral",
      label: "مبدا",
    },
    {
      key: "barcode",
      label: "بارکد",
    },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2 text-center">
          <Link
            href={`/admin/orders/edit/${row.id}`}
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
          مدیریت سفارشات
        </h2>
        <Link
          href="/admin/orders/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن سفارش جدید
        </Link>
      </div>
      <div className="flex items-center justify-start gap-4 mb-4">
        <input
          type="text"
          placeholder="جستجو..."
          className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <select className="border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option value="">همه وضعیت ها</option>
          <option value="در حال پردازش">در حال پردازش</option>
          <option value="تحویل داده شده">تحویل داده شده</option>
          <option value="لغو شده">لغو شده</option>
        </select>
      </div>
      <Table data={DummyOrders} columns={columns} />
    </div>
  );
}
