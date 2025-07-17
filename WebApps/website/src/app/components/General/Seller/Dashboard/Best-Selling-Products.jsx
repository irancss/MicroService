"use client";
import AnimatedDiv from "@components/Animated/Div";
import Table from "@components/General/Table"; // فرض می‌کنیم مسیر درست است
import { useState } from "react";

export default function BestSellingProducts() {
  const periods = [
    { label: "ماهانه", key: "monthly" },
    { label: "هفتگی", key: "weekly" },
    { label: "روزانه", key: "daily" },
  ];

  const dataByPeriod = {
    monthly: [
      { id: 1, name: "محصول ۱", price: 100000, sold: 50 },
      { id: 2, name: "محصول ۲", price: 200000, sold: 30 },
      { id: 3, name: "محصول ۳", price: 150000, sold: 20 },
      { id: 4, name: "محصول ۴", price: 250000, sold: 10 },
      { id: 5, name: "محصول ۵", price: 300000, sold: 5 },
    ],
    weekly: [
      { id: 1, name: "محصول ۱", price: 100000, sold: 10 },
      { id: 2, name: "محصول ۲", price: 200000, sold: 8 },
      { id: 3, name: "محصول ۳", price: 150000, sold: 6 },
      { id: 4, name: "محصول ۴", price: 250000, sold: 4 },
      { id: 5, name: "محصول ۵", price: 300000, sold: 2 },
    ],
    daily: [
      { id: 1, name: "محصول ۱", price: 100000, sold: 2 },
      { id: 2, name: "محصول ۲", price: 200000, sold: 1 },
      { id: 3, name: "محصول ۳", price: 150000, sold: 1 },
      { id: 4, name: "محصول ۴", price: 250000, sold: 0 },
      { id: 5, name: "محصول ۵", price: 300000, sold: 0 },
    ],
  };

  const [selectedPeriod, setSelectedPeriod] = useState("monthly");

  return (
    <AnimatedDiv
      className="bg-gray-50 rounded-lg p-4 shadow"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
    >
      <div className="flex space-x-2 mb-4 rtl:space-x-reverse">
        {periods.map((period) => (
          <button
            key={period.key}
            className={`px-3 py-1 rounded border transition-colors duration-150 ease-in-out
              ${
                selectedPeriod === period.key
                  ? "bg-blue-500 text-white border-blue-600 font-semibold" // استایل دکمه فعال
                  : "bg-white text-gray-700 border-gray-300 hover:bg-gray-100 hover:border-gray-400" // استایل دکمه غیرفعال
              }`}
            onClick={() => setSelectedPeriod(period.key)}
          >
            {period.label}
          </button>
        ))}
      </div>
      <Table
        key={selectedPeriod} // این key برای re-render شدن Table هنگام تغییر دوره کافی است
        title={`محصولات پرفروش - ${
          periods.find((p) => p.key === selectedPeriod)?.label
        }`}
        columns={["نام محصول", "قیمت", "فروش"]}
        data={dataByPeriod[selectedPeriod].map((item) => ({
          "نام محصول": item.name,
          قیمت: item.price.toLocaleString("fa-IR") + " تومان",
          فروش: item.sold.toString(),
        }))}
        // refreshKey={selectedPeriod} // اگر key اصلی کار می‌کند، این احتمالاً لازم نیست
      />
    </AnimatedDiv>
  );
}