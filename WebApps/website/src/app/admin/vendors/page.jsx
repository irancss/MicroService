"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import toast from "react-hot-toast";
import Swal from "sweetalert2";

export default function Vendors() {
  // Dummy data for vendors
  const vendors = [
    {
      id: 1,
      name: "فروشنده اول",
      mobile: "09123456789",
      countProducts: 10,
      status: "فعال",
    },
    {
      id: 2,
      name: "فروشنده دوم",
      mobile: "09123456788",
      countProducts: 5,
      status: "غیرفعال",
    },
  ];

  // Add render function to each vendor based on status
  vendors.forEach((vendor) => {
    // Make sure to import SweetAlert2 at the top of your file:
    // import Swal from 'sweetalert2';

    vendor.render = () => {
      const handleStatusChange = (newStatus) => {
        Swal.fire({
          title:
            newStatus === "فعال" ? "فعال سازی فروشنده" : "غیرفعال سازی فروشنده",
          text: `آیا از ${
            newStatus === "فعال" ? "فعال" : "غیرفعال"
          } کردن این فروشنده اطمینان دارید؟`,
          icon: "warning",
          showCancelButton: true,
          confirmButtonColor: newStatus === "فعال" ? "#10B981" : "#EF4444",
          cancelButtonColor: "#6B7280",
          confirmButtonText: "بله، انجام شود",
          cancelButtonText: "انصراف",
        }).then((result) => {
          if (result.isConfirmed) {
            // Here you would make API call to update vendor status
            vendor.status = newStatus;
            toast.success(
              `فروشنده با موفقیت ${
                newStatus === "فعال" ? "فعال" : "غیرفعال"
              } شد.`
            );
          }
        });
      };

      return (
        <div className="flex justify-center items-center space-x-2">
          <Link
            href={`/admin/vendors/${vendor.id}`}
            className="text-blue-500 hover:text-blue-700"
          >
            مشاهده
          </Link>
          {vendor.status === "فعال" ? (
            <button
              className="text-red-500 hover:text-red-700"
              onClick={() => handleStatusChange("غیرفعال")}
            >
              غیر فعال کردن
            </button>
          ) : (
            <button
              className="text-green-500 hover:text-green-700"
              onClick={() => handleStatusChange("فعال")}
            >
              فعال کردن
            </button>
          )}
        </div>
      );
    };
  });

  const columns = [
    { key: "name", label: "نام فروشنده" },
    { key: "mobile", label: "شماره موبایل" },
    { key: "countProducts", label: "تعداد محصولات" },
    {
      key: "status",
      label: "وضعیت",
      render: (row) => (
        <span
          className={`px-2 py-1 rounded-full text-xs ${
            row.status === "فعال"
              ? "bg-green-500 text-white"
              : "bg-red-500 text-white"
          }`}
        >
          {row.status}
        </span>
      ),
    },
    { key: "actions", label: "عملیات", render: (row) => row.render() },
  ];

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">فروشندگان</h1>
      <Table data={vendors} columns={columns} />
    </div>
  );
}
