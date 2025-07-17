import Table from "@components/General/Table";
import Link from "next/link";

export default function ShippingList() {
  const shippingMethods = [
    {
      id: 1,
      name: "روش ارسال استاندارد",
      cost: "20,000 تومان",
      regions: "تهران, البرز",
      cities: "تهران, کرج",
      deliveryTime: "3-5 روز کاری",
    },
    {
      id: 2,
      name: "روش ارسال سریع",
      cost: "50,000 تومان",
      regions: "تهران, اصفهان",
      cities: "تهران, اصفهان",
      deliveryTime: "1-2 روز کاری",
    },
    {
      id: 3,
      name: "روش ارسال رایگان",
      cost: "رایگان",
      regions: "تمام کشور",
      cities: "تمام شهرها",
      deliveryTime: "5-7 روز کاری",
    },

  ];
  const columns = [
    { key: "name", label: "نام روش ارسال" },
    { key: "cost", label: "هزینه ارسال" },
    { key: "regions", label: "استان ها" },
    { key: "cities", label: "شهرها" },
    { key: "deliveryTime", label: "زمان تحویل" },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2">
          <Link
            href={`/admin/settings/shipping/edit/${row.id}`}
            className="text-blue-600 hover:text-blue-800"
          >
            ویرایش
          </Link>
          <Link
            href={`/admin/settings/shipping/delete/${row.id}`}
            className="text-red-600 hover:text-red-800"
          >
            حذف
          </Link>
        </div>
      ),
    },
  ];
  return (
    <div className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت روش‌های ارسال
        </h2>
        <Link
          href="/admin/settings/shipping/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن روش ارسال جدید
        </Link>
      </div>
      <hr className="border-b border-blue-200 mb-6" />
      <Table columns={columns} data={shippingMethods} />
    </div>
  );
}
