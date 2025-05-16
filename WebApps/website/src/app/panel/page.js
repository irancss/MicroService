"use client";
import { motion } from "framer-motion";

const suggestedProducts = [
  { id: 1, name: "محصول پیشنهادی ۱", price: "۱۰۰٬۰۰۰ تومان" },
  { id: 2, name: "محصول پیشنهادی ۲", price: "۲۰۰٬۰۰۰ تومان" },
];

const lastSeenProducts = [
  { id: 1, name: "محصول دیده شده ۱" },
  { id: 2, name: "محصول دیده شده ۲" },
  { id: 3, name: "محصول دیده شده ۳" },
  { id: 4, name: "محصول دیده شده ۴" },
  { id: 5, name: "محصول دیده شده ۵" },
];

const lastOrders = [
  { id: 101, date: "۱۴۰۳/۰۴/۰۱", status: "تحویل شده", amount: "۳۰۰٬۰۰۰ تومان" },
  {
    id: 102,
    date: "۱۴۰۳/۰۳/۲۹",
    status: "در حال پردازش",
    amount: "۱۵۰٬۰۰۰ تومان",
  },
  { id: 103, date: "۱۴۰۳/۰۳/۲۷", status: "لغو شده", amount: "۵۰٬۰۰۰ تومان" },
  { id: 104, date: "۱۴۰۳/۰۳/۲۵", status: "تحویل شده", amount: "۴۰۰٬۰۰۰ تومان" },
  { id: 105, date: "۱۴۰۳/۰۳/۲۳", status: "تحویل شده", amount: "۲۵۰٬۰۰۰ تومان" },
];

export default function PanelPage() {
  return (
    <motion.div
      className="flex flex-col items-center justify-center min-h-screen py-8 bg-gradient-to-br from-blue-50 to-purple-100"
      initial={{ opacity: 0, y: 40 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.6 }}
    >
      <motion.div
        className="w-full max-w-3xl bg-white/80 rounded-2xl shadow-xl p-6 sm:p-8 mb-8 border border-gray-200"
        initial={{ scale: 0.95, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        transition={{ delay: 0.1 }}
      >
        <motion.h1
          className="text-3xl sm:text-4xl font-bold mb-3 sm:mb-4 text-right text-indigo-700"
          initial={{ scale: 0.8 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2 }}
        >
          پنل کاربری شما
        </motion.h1>
      </motion.div>

      {/* Last Orders Table */}
      <motion.div
        className="w-full max-w-3xl bg-white rounded-2xl shadow-lg p-4 sm:p-6 border-t-4 border-pink-400 mb-4"
        initial={{ y: 40, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        transition={{ delay: 0.7 }}
      >
        <h2 className="text-lg sm:text-xl font-semibold mb-3 sm:mb-4 text-right text-pink-600">
          ۵ سفارش آخر شما
        </h2>
        <div className="overflow-x-auto rounded-xl ">
          <table className="min-w-full bg-white rounded-xl shadow text-right text-sm sm:text-base">
            <thead>
              <tr>
                <th className="py-2 px-2 sm:px-4 border-b text-right whitespace-nowrap">
                  کد سفارش
                </th>
                <th className="py-2 px-2 sm:px-4 border-b text-right whitespace-nowrap">
                  تاریخ
                </th>
                <th className="py-2 px-2 sm:px-4 border-b text-right whitespace-nowrap">
                  وضعیت
                </th>
                <th className="py-2 px-2 sm:px-4 border-b text-right whitespace-nowrap">
                  مبلغ
                </th>
              </tr>
            </thead>
            <tbody>
              {lastOrders.map((order) => (
                <tr key={order.id} className="hover:bg-pink-50 transition">
                  <td className="py-2 px-2 sm:px-4 border-b text-right">
                    {order.id}
                  </td>
                  <td className="py-2 px-2 sm:px-4 border-b text-right">
                    {order.date}
                  </td>
                  <td className="py-2 px-2 sm:px-4 border-b text-right">
                    {order.status}
                  </td>
                  <td className="py-2 px-2 sm:px-4 border-b text-right">
                    {order.amount}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </motion.div>

      {/* Suggested Products */}
      <motion.div
        className="w-full max-w-3xl mb-8 bg-white rounded-2xl shadow-lg p-4 sm:p-6 border-t-4 border-indigo-400"
        initial={{ x: 100, opacity: 0 }}
        animate={{ x: 0, opacity: 1 }}
        transition={{ delay: 0.5 }}
      >
        <h2 className="text-lg sm:text-xl font-semibold mb-3 sm:mb-4 text-right text-indigo-600">
          پیشنهاد محصول
        </h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 sm:gap-6">
          {suggestedProducts.map((p) => (
            <motion.div
              key={p.id}
              className="bg-indigo-50 rounded-xl shadow p-3 sm:p-4 flex flex-col items-end border border-indigo-100 hover:shadow-md transition"
              whileHover={{ scale: 1.05 }}
            >
              <span className="font-medium text-right text-indigo-800">
                {p.name}
              </span>
              <span className="text-xs sm:text-sm text-indigo-500 mt-2">
                {p.price}
              </span>
            </motion.div>
          ))}
        </div>
      </motion.div>

      {/* Last Seen Products */}
      <motion.div
        className="w-full max-w-3xl mb-8 bg-white rounded-2xl shadow-lg p-4 sm:p-6 border-t-4 border-purple-400"
        initial={{ x: -100, opacity: 0 }}
        animate={{ x: 0, opacity: 1 }}
        transition={{ delay: 0.6 }}
      >
        <h2 className="text-lg sm:text-xl font-semibold mb-3 sm:mb-4 text-right text-purple-600">
          ۵ محصول آخری که دیدید
        </h2>
        <ul className="flex gap-2 sm:gap-3 flex-wrap justify-end">
          {lastSeenProducts.map((p) => (
            <motion.li
              key={p.id}
              className="bg-purple-50 rounded-xl shadow px-3 py-2 sm:px-4 text-right border border-purple-100 hover:shadow-md transition text-sm sm:text-base"
              whileHover={{ scale: 1.05 }}
            >
              {p.name}
            </motion.li>
          ))}
        </ul>
      </motion.div>
    </motion.div>
  );
}
