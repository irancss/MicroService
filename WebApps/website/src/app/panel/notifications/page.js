"use client";
import { useState } from "react";
import AnimatedHr from "@components/Animated/Hr";
import Link from "next/link";
import Swal from "sweetalert2";

export default function NotificationsPage() {
  const initialNotifications = [
    { id: 1, message: "تخفیف ویژه برای محصولات جدید!", link: "/product/1" },
    {
      id: 2,
      message: "محصولات شما به زودی موجود خواهند شد.",
      link: "/product/2",
    },
    { id: 3, message: "تخفیف 20% برای خرید اول!", link: "/product/3" },
  ];
  const [notifications, setNotifications] = useState(initialNotifications);

  const handleDelete = (id) => {
    Swal.fire({
      title: "حذف اعلان",
      text: "آیا از حذف این کالا از لیست اطلاع‌رسانی‌ها اطمینان دارید؟",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "بله، حذف کن",
      cancelButtonText: "انصراف",
      confirmButtonColor: "#d33",
      cancelButtonColor: "#aaa",
    }).then((result) => {
      if (result.isConfirmed) {
        setNotifications(notifications.filter((n) => n.id !== id));
        Swal.fire({
          icon: "success",
          title: "حذف شد!",
          text: "اعلان  محصول با موفقیت حذف شد.",
          confirmButtonText: "باشه",
          confirmButtonColor: "#d33",
        });
      }
    });
  };

  return (
    <div className="mt-8">
      <h2 className="text-xl font-semibold text-gray-800">
        لیست اطلاع رسانی‌ها
      </h2>
      <AnimatedHr className="my-4" />
      <ul className="mt-4 space-y-4">
        {notifications.map((notification) => (
          <li
            key={notification.id}
            className="flex items-center justify-between bg-gray-100 rounded-lg shadow p-4 hover:bg-gray-200 transition duration-200"
          >
            <Link href={notification.link} className="flex-1">
              {notification.message}
            </Link>
            <button
              className="ml-4 text-red-500 hover:text-red-700"
              onClick={() => handleDelete(notification.id)}
            >
              حذف
            </button>
          </li>
        ))}
        {notifications.length === 0 && (
          <li className="text-center text-gray-500">اعلانی وجود ندارد.</li>
        )}
      </ul>
    </div>
  );
}
