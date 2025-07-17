"use client";

import AddSchema from "@components/Admin/Settings/Schema/Add";
import AddList from "@components/Admin/Settings/Schema/List";
import { useState } from "react";

export default function SchemaSettings() {
  const [activeTab, setActiveTab] = useState("add");

  return (
    <div className="p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-4">تنظیمات اسکیما</h2>
      <div className="mb-4 border-b border-gray-200">
        <nav className="flex" aria-label="Tabs">
          <button
            onClick={() => setActiveTab("add")}
            className={`mr-4 py-2 px-4 border-b-2 font-medium ${
              activeTab === "add"
                ? "border-blue-500 text-blue-600"
                : "border-transparent text-gray-500 hover:text-gray-700"
            }`}
          >
            افزودن اسکیما
          </button>
          <button
            onClick={() => setActiveTab("list")}
            className={`mr-4 py-2 px-4 border-b-2 font-medium ${
              activeTab === "list"
                ? "border-blue-500 text-blue-600"
                : "border-transparent text-gray-500 hover:text-gray-700"
            }`}
          >
            لیست اسکیماها
          </button>
        </nav>
      </div>

      <div className="mt-6">
        {activeTab === "add" ? <AddSchema /> : <AddList />}
      </div>
    </div>
  );
}
