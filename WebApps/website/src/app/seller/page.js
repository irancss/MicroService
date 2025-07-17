"use client";
import { motion } from "framer-motion";
import  SalesRecords  from "@components/General/Seller/Dashboard/Sales-records";
import LastNotification from "@components/General/Seller/Dashboard/Last-Notification";
import DepotGoods from "@components/General/Seller/Dashboard/Depot-Goods";
import BestSellingProducts from "@components/General/Seller/Dashboard/Best-Selling-Products";

export default function SellerPage() {
  const sellVolume = [
    {
      title: "هفته جاری",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
    {
      title: "هفته گذشته",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
    {
      title: "ماه گذشته",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
  ];
  const sellCount = [
    {
      title: "هفته جاری",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
    {
      title: "هفته گذشته",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
    {
      title: "ماه گذشته",
      value: 0,
      items: [
        { title: "اقساطی", value: 0 },
        { title: "اعتباری", value: 0 },
        { title: "نقدی", value: 0 },
      ],
    },
  ];
  return (
    <div className="flex flex-col min-h-screen bg-white p-4 space-y-6">
      <div className="grid gap-4">
        <motion.div
          className="rounded-lg p-4 flex flex-col space-y-2 shadow"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
        >
          <h2 className="font-bold text-lg">حجم فروش</h2>
          <hr className="border-gray-300" />
          <div className="flex gap-4 justify-around text-sm">
            {sellVolume.map((period, idx) => (
              <div
                key={period.title}
                className="grid rounded-2xl px-3 py-3 border-2 w-full border-gray-200 gap-4 text-sm"
              >
                <div className="flex justify-between bg-gray-100 rounded-2xl px-3 py-3">
                  <span>{period.title}: </span>
                  <span className="font-bold">
                    {period.value.toLocaleString("fa-IR")}
                  </span>
                </div>
                {period.items.map((item) => (
                  <div className="flex justify-between" key={item.title}>
                    <span>{item.title}: </span>
                    <span className="font-bold">
                      {item.value.toLocaleString("fa-IR")}
                    </span>
                  </div>
                ))}
              </div>
            ))}
          </div>
        </motion.div>

        <motion.div
          className="rounded-lg p-4 flex flex-col space-y-2 shadow"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
        >
          <h2 className="font-bold text-lg">تعداد فروش</h2>
          <hr className="border-gray-300" />
          <div className="flex gap-4 justify-around text-sm">
            {sellCount.map((period, idx) => (
              <div
                key={period.title}
                className="grid rounded-2xl px-3 py-3 border-2 w-full border-gray-200 gap-4 text-sm"
              >
                <div className="flex justify-between bg-gray-100 rounded-2xl px-3 py-3">
                  <span>{period.title}: </span>
                  <span className="font-bold">
                    {period.value.toLocaleString("fa-IR")}
                  </span>
                </div>
                {period.items.map((item) => (
                  <div className="flex justify-between" key={item.title}>
                    <span>{item.title}: </span>
                    <span className="font-bold">
                      {item.value.toLocaleString("fa-IR")}
                    </span>
                  </div>
                ))}
              </div>
            ))}
          </div>
        </motion.div>
      </div>

      <div className="flex flex-row gap-5">
        <div className="w-2/3">
          <LastNotification />
        </div>
        <div className="w-1/3">
          <LastNotification />
        </div>
      </div>

      <div className="flex flex-row gap-5">
        <div className="w-1/3">
          <DepotGoods />
        </div>
        <div className="w-2/3">
          <LastNotification />
        </div>
      </div>


      {/* پرفروش‌ترین کالاها */}
      <motion.div
        className="bg-gray-50 rounded-lg p-4 shadow"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
      >
        <div className="flex space-x-2 mb-4 rtl:space-x-reverse">
          <button className="px-3 py-1 rounded bg-blue-100 text-blue-700">
            ماهانه
          </button>
          <button className="px-3 py-1 rounded hover:bg-blue-50">هفتگی</button>
          <button className="px-3 py-1 rounded hover:bg-blue-50">روزانه</button>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm text-right">
            <thead>
              <tr className="bg-gray-100">
                <th className="p-2">تصویر</th>
                <th className="p-2">عنوان</th>
                <th className="p-2">کد کالا</th>
                <th className="p-2">دسته‌بندی</th>
                <th className="p-2">برند</th>
                <th className="p-2">میزان فروش</th>
                <th className="p-2">تعداد فروش</th>
              </tr>
            </thead>
            <tbody>
              {[1, 2, 3].map((n) => (
                <tr key={n} className="border-b">
                  <td className="p-2">
                    <div className="w-10 h-10 bg-gray-200 rounded" />
                  </td>
                  <td className="p-2">کالا {n}</td>
                  <td className="p-2">#۰۰{n}</td>
                  <td className="p-2">دسته {n}</td>
                  <td className="p-2">برند {n}</td>
                  <td className="p-2">۰</td>
                  <td className="p-2">۰</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </motion.div>

      {/* چهار پست آخر مطالب آموزشی */}
      <motion.div
        className="bg-gray-50 rounded-lg p-4 shadow"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
      >
        <h2 className="font-bold mb-2">آخرین مطالب آموزشی</h2>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {[1, 2, 3, 4].map((n) => (
            <div
              key={n}
              className="bg-white rounded p-3 shadow flex flex-col items-center"
            >
              <div className="w-16 h-16 bg-gray-200 rounded mb-2" />
              <div className="font-semibold text-center">مقاله {n}</div>
              <button className="mt-2 text-blue-600 text-xs border px-2 py-1 rounded">
                مشاهده
              </button>
            </div>
          ))}
        </div>
      </motion.div>
    </div>
  );
}
