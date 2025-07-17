import Filters from "@components/Admin/Customer/Filters";
import List from "@components/Admin/Customer/List";
import Link from "next/link";
import Table from "@components/General/Table";

export default function Customers() {
  const customers = [
    {
      id: 1,
      name: "علی رضایی",
      mobile: "09123456789",
      address: "تهران، خیابان ولیعصر",
      city: "تهران",
      createdAt: "2023-10-01",
      status: "فعال",
      lastLogin: "2023-10-05",
      ordersCount: 5,
      loyaltyPoints: 100,
    },
    {
      id: 2,
      name: "مریم حسینی",
      mobile: "09123456780",
      address: "اصفهان، خیابان چهارباغ",
      city: "اصفهان",
      createdAt: "2023-09-15",
      status: "غیرفعال",
      lastLogin: "2023-09-20",
      ordersCount: 2,
      loyaltyPoints: 50,
    },
  ];
  const columns = [
    { key: "name", label: "نام مشتری" },
    { key: "mobile", label: "شماره موبایل" },
    { key: "address", label: "آدرس" },
    { key: "city", label: "شهر" },
    { key: "status", label: "وضعیت" },
    {
      key: "createdAt",
      label: "تاریخ ایجاد",
      render: (customer) => {
        return new Date(customer.createdAt).toLocaleDateString("fa-IR");
      },
    },

    {
      key: "lastLogin",
      label: "آخرین ورود",
      render: (customer) => {
        return new Date(customer.lastLogin).toLocaleDateString("fa-IR");
      },
    },
    { key: "ordersCount", label: "تعداد سفارشات" },
    { key: "loyaltyPoints", label: "امتیاز" },
    {
      key: "actions",
      label: "عملیات",
      render: (customer) => (
        <div className="flex gap-2">
          <Link
            href={`/admin/customers/${customer.id}`}
            className="text-blue-600 hover:text-blue-800"
          >
            ویرایش
          </Link>
          <button className="text-red-600 hover:text-red-800">حذف</button>
        </div>
      ),
    },
  ];
  const currentPage = 1;
  const totalPages = 5;
  const handleSearch = (searchTerm) => {
   
  };
  const handleFilter = (filter) => {
    
  };
  const handleSort = (sort) => {
  };
  const handlePageChange = (page) => {
  };

  return (
    <div >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
          مدیریت مشتریان
        </h2>
        <Link
          href="/admin/customers/add"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          افزودن مشتری جدید
        </Link>
      </div>
      <hr className="border-b border-blue-200 mb-6" />
      <Filters columns={columns} />
      <div className="mt-6">
        <Table
          data={customers}
          columns={columns}
          onSearch={handleSearch}
          onFilter={handleFilter}
          onSort={handleSort}
          onPageChange={handlePageChange}
          currentPage={currentPage}
          totalPages={totalPages}
        />
      </div>
    </div>
  );
}
