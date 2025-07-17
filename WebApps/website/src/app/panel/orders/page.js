"use client";
import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import { motion, AnimatePresence } from "framer-motion";
import {
  faHome,
  faShoppingCart,
  faHistory,
  faTicket,
  faWallet,
  faHeart,
  faQuestionCircle,
  faSignOutAlt,
  faEdit,
  faComment,
  faEnvelope,
} from "@fortawesome/free-solid-svg-icons";
import AnimatedHr from "@components/Animated/Hr";
library.add(
  faHome,
  faShoppingCart,
  faHistory,
  faTicket,
  faWallet,
  faHeart,
  faQuestionCircle,
  faSignOutAlt,
  faEdit,
  faComment,
  faEnvelope
);

const dummyOrders = {
  in_progress: [
    { id: "1001", date: "1403/03/01", status: "در حال پردازش" },
    { id: "1002", date: "1403/03/02", status: "در انتظار ارسال" },
  ],
  sent: [{ id: "1003", date: "1403/02/28", status: "ارسال شده" }],
  delivered: [{ id: "1004", date: "1403/02/25", status: "تحویل شده" }],
  returned: [{ id: "1005", date: "1403/02/20", status: "مرجوع شده" }],
  canceled: [{ id: "1006", date: "1403/02/15", status: "لغو شده" }],
};

export default function OrdersPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [activeTab, setActiveTab] = useState(
    searchParams.get("activeTab") || "in_progress"
  );
  const [orders, setOrders] = useState(null);

  useEffect(() => {
    setActiveTab(searchParams.get("activeTab") || "in_progress");
  }, [searchParams]);

  useEffect(() => {
    // فرض کن اینجا درخواست به بک‌اند می‌زنیم
    // اگر دیتا نیامد، داده dummy را ست کن
    const fetchOrders = async () => {
      try {
        // const res = await fetch(...);
        // const data = await res.json();
        // setOrders(data);
        // شبیه‌سازی خطا یا نبود دیتا:
        throw new Error("No data from backend");
      } catch (e) {
        setOrders(dummyOrders[activeTab] || []);
      }
    };
    fetchOrders();
  }, [activeTab]);

  const orderTabs = [
    { name: "جاری", value: "in_progress", icon: "" },
    { name: "ارسال شده", value: "sent", icon: "" },
    { name: "تحویل شده", value: "delivered", icon: "" },
    { name: "مرجوع شده", value: "returned", icon: "" },
    { name: "لغو شده", value: "canceled", icon: "" },
  ];

  const handleTabClick = (value) => {
    router.push(`?activeTab=${value}`, { scroll: false });
  };

  return (
    <div className="w-full min-h-screen flex flex-col items-center bg-gradient-to-br from-blue-50 to-white py-8">
      <div className="w-full max-w-5xl">
        <h3 className="text-2xl font-bold text-gray-800 mb-6 text-center">
          سفارشات
        </h3>
        <div className="flex justify-center gap-2 mb-6">
          {orderTabs.map((tab) => (
            <motion.button
              key={tab.name}
              onClick={() => handleTabClick(tab.value)}
              className={`px-6 py-2 text-base font-medium rounded-full transition-colors duration-200 shadow-sm focus:outline-none ${
                activeTab === tab.value
                  ? "bg-rose-600 text-white shadow-lg"
                  : "bg-gray-100 text-gray-700 hover:bg-blue-100"
              }`}
              whileTap={{ scale: 0.95 }}
              whileHover={{ scale: 1.05 }}
              layout
            >
              {tab.icon && <FontAwesomeIcon icon={tab.icon} className="mr-2" />}
              {tab.name}
            </motion.button>
          ))}
        </div>
        <AnimatedHr />
        <div className="w-full flex flex-col items-center">
          <AnimatePresence mode="wait">
            <motion.div
              key={activeTab}
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -30 }}
              transition={{ duration: 0.4 }}
              className="w-full"
            >
              <div className="mb-4 text-center text-blue-700 font-semibold">
                Tab: {activeTab}
              </div>
              <div className="overflow-x-auto">
                <table className="w-full bg-white border border-gray-200 rounded-xl shadow-lg">
                  <thead className="bg-blue-50 border-b border-gray-200">
                    <tr>
                      <th className="px-6 py-3 text-right text-gray-700 font-semibold">
                        شماره سفارش
                      </th>
                      <th className="px-6 py-3 text-right text-gray-700 font-semibold">
                        تاریخ
                      </th>
                      <th className="px-6 py-3 text-right text-gray-700 font-semibold">
                        وضعیت
                      </th>
                      <th className="px-6 py-3 text-right text-gray-700 font-semibold">
                        عملیات
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {orders && orders.length > 0 ? (
                      orders.map((order) => (
                        <tr key={order.id}>
                          <td className="px-6 py-4 text-right">{order.id}</td>
                          <td className="px-6 py-4 text-right">{order.date}</td>
                          <td className="px-6 py-4 text-right">
                            {order.status}
                          </td>
                          <td className="px-6 py-4 text-right">
                            <button className="text-blue-600 hover:underline">
                              مشاهده
                            </button>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td
                          colSpan={4}
                          className="text-center py-8 text-gray-400"
                        >
                          سفارشی یافت نشد.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </motion.div>
          </AnimatePresence>
        </div>
      </div>
    </div>
  );
}
