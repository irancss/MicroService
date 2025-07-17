"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import { useState } from "react";
import toast from "react-hot-toast";
import Swal from "sweetalert2";
export default function VendorRequests() {
  // Dummy data for vendor requests
  const [vendorRequests, setVendorRequests] = useState([
    {
      id: 1,
      name: "Vendor A",
      mobile: "09123456789",
      status: "pending",
      render: function () {
        return (
          <div className="flex justify-center items-center space-x-2">
            {this.status === "pending" && (
              <>
                <Link href={`/admin/vendors/requests/${this.id}`}>
                  <button className="text-blue-500 hover:text-blue-700 ml-2">
                    جزئیات
                  </button>
                </Link>
                <button
                  className="text-blue-500 hover:text-blue-700 ml-2"
                  onClick={() => handleApprove(this.id)}
                >
                  تایید
                </button>
                <button
                  className="text-red-500 hover:text-red-700"
                  onClick={() => handleReject(this.id)}
                >
                  رد
                </button>
              </>
            )}
            {this.status === "approved" && (
              <span className="text-green-500">تایید شده</span>
            )}
            {this.status === "rejected" && (
              <span className="text-red-500">رد شده</span>
            )}
          </div>
        );
      },
    },
    {
      id: 2,
      name: "Vendor B",
      mobile: "09187654321",
      status: "approved",
      render: function () {
        return (
          this.status === "approved" && (
            <span className="text-green-500">تایید شده</span>
          )
        );
      },
    },
    {
      id: 3,
      name: "Vendor C",
      mobile: "09198765432",
      status: "rejected",
      render: function () {
        return (
          this.status === "rejected" && (
            <span className="text-red-500">رد شده</span>
          )
        );
      },
    },
  ]);

  const handleApprove = (id) => {
    Swal.fire({
      title: "تایید درخواست",
      text: "آیا از تایید این درخواست اطمینان دارید؟",
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "بله، تایید شود",
      cancelButtonText: "انصراف",
    }).then((result) => {
      if (result.isConfirmed) {
        setVendorRequests((prev) =>
          prev.map((vendor) =>
            vendor.id === id ? { ...vendor, status: "approved" } : vendor
          )
        );
        toast.success("درخواست با موفقیت تایید شد");
      }
    });
  };

  const handleReject = (id) => {
    Swal.fire({
      title: "رد درخواست",
      text: "آیا از رد این درخواست اطمینان دارید؟",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#3085d6",
      confirmButtonText: "بله، رد شود",
      cancelButtonText: "انصراف",
    }).then((result) => {
      if (result.isConfirmed) {
        setVendorRequests((prev) =>
          prev.map((vendor) =>
            vendor.id === id ? { ...vendor, status: "rejected" } : vendor
          )
        );
        toast.error("درخواست رد شد");
      }
    });
  };

  const columns = [
    { key: "name", label: "نام فروشنده" },
    { key: "mobile", label: "شماره موبایل" },
    { key: "status", label: "وضعیت" },
    { key: "actions", label: "عملیات", render: (row) => row.render() },
  ];

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">درخواست‌های فروشنده</h1>
      <Table data={vendorRequests} columns={columns} />
    </div>
  );
}
