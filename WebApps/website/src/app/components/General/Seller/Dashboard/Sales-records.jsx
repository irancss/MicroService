"use client";
import { useState } from "react";
import AnimatedDiv from "@components/Animated/Div";

const SalesRecords = () => {
  const [chartData, setChartData] = useState({
    labels: ["January", "February", "March", "April", "May", "June", "July"],
    datasets: [
      {
        label: "Sales",
        data: [65, 59, 80, 81, 56, 55, 40],
        fill: false,
        backgroundColor: "#42A5F5",
        borderColor: "#42A5F5",
      },
    ],
  });

  // Animation and filter buttons

  const FilteredChart = () => (
    <AnimatedDiv
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
      <div className="h-40 flex items-center justify-center text-gray-400">
        {/* <SalesRecords /> */}
      </div>
    </AnimatedDiv>
  );

  const [chartOptions, setChartOptions] = useState({
    responsive: true,
    plugins: {
      legend: {
        position: "top",
      },
      title: {
        display: true,
        text: "Sales Records",
      },
    },
  });

  return (
    <div className="w-full">
      <h1 className="text-2xl font-bold mb-4">Sales Records</h1>
      {/* <Chart chartData={chartData} chartOptions={chartOptions} /> */}
    </div>
  );
};

export default SalesRecords;
