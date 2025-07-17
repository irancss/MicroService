"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import { useState } from "react";

export default function ProductsPage() {
  const DummyData = [
    {
      image: "https://via.placeholder.com/150",
      name: "محصول ۱",
      label: "برچسب ۱",
      category: "دسته بندی ۱",
      price: 100000,
      specialPrice: 90000,
      stock: 50,
      brand: "برند ۱",
      createdAt: "2023-01-01",
      updatedAt: "2023-01-02",
    },
    {
      image: "https://via.placeholder.com/150",
      name: "محصول ۲",
      label: "برچسب ۲",
      category: "دسته بندی ۲",
      price: 200000,
      specialPrice: 180000,
      stock: 30,
      brand: "برند ۲",
      createdAt: "2023-01-03",
      updatedAt: "2023-01-04",
    },
  ];
  const columns = [
    {
      key: "image",
      label: "تصویر",
      render: (row) => (
        <img src={row.image} alt={row.name} width={50} height={50} />
      ),
    },
    { key: "name", label: "نام محصول" },
    { key: "label", label: "برچسب" },
    { key: "category", label: "دسته بندی" },
    { key: "price", label: "قیمت" },
    { key: "specialPrice", label: "قیمت ویژه" },
    { key: "stock", label: "موجودی" },
    { key: "brand", label: "برند" },
    { key: "createdAt", label: "تاریخ ایجاد" },
    { key: "updatedAt", label: "تاریخ بروزرسانی" },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2">
          <Link
            href={`/admin/products/edit/${row.name}`}
            className="text-blue-500 hover:text-blue-700"
          >
            ویرایش
          </Link>
          <button className="text-red-500 hover:text-red-700">حذف</button>
        </div>
      ),
    },
  ];

  const [stockFilter, setStockFilter] = useState("all");

  const filteredData =
    stockFilter === "all"
      ? DummyData
      : stockFilter === "inStock"
      ? DummyData.filter((item) => item.stock > 0)
      : DummyData.filter((item) => item.stock === 0);

  const getFilterButtonStyle = (filterType) => ({
    padding: "0.5rem 1rem",
    marginRight: "0.5rem",
    borderRadius: "0.375rem",
    border: "1px solid #d1d5db",
    cursor: "pointer",
    backgroundColor: stockFilter === filterType ? "#3b82f6" : "white",
    color: stockFilter === filterType ? "white" : "#374151",
  });

  return (
    <div style={{ padding: "1.5rem" }}>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: "1.5rem",
        }}
      >
        <h1
          style={{
            fontSize: "1.5rem",
            fontWeight: "bold",
            color: "#1f2937",
          }}
        >
          مدیریت محصولات
        </h1>
        <Link
          href="/admin/products/add"
          style={{
            backgroundColor: "#3b82f6",
            color: "white",
            padding: "0.5rem 1rem",
            borderRadius: "0.375rem",
            border: "none",
            cursor: "pointer",
          }}
        >
          افزودن محصول جدید
        </Link>
      </div>

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
          gap: "1rem",
          marginBottom: "1.5rem",
        }}
      >
        <div
          style={{
            backgroundColor: stockFilter === "all" ? "#3b82f6" : "white",
            color: stockFilter === "all" ? "white" : "inherit",
            padding: "1rem",
            borderRadius: "0.5rem",
            boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            border: "1px solid #e5e7eb",
            cursor: "pointer",
          }}
          onClick={() => setStockFilter("all")}
        >
          <h3
            style={{
              color: stockFilter === "all" ? "white" : "#6b7280",
              marginBottom: "0.25rem",
            }}
          >
            تعداد کل محصولات
          </h3>
          <p
            style={{
              fontSize: "1.5rem",
              fontWeight: "bold",
              color: stockFilter === "all" ? "white" : "inherit",
            }}
          >
            {DummyData.length}
          </p>
        </div>
        <div
          style={{
            backgroundColor: stockFilter === "inStock" ? "#3b82f6" : "white",
            color: stockFilter === "inStock" ? "white" : "inherit",
            padding: "1rem",
            borderRadius: "0.5rem",
            boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            border: "1px solid #e5e7eb",
            cursor: "pointer",
          }}
          onClick={() => setStockFilter("inStock")}
        >
          <h3
            style={{
              color: stockFilter === "inStock" ? "white" : "#6b7280",
              marginBottom: "0.25rem",
            }}
          >
            محصولات موجود
          </h3>
          <p
            style={{
              fontSize: "1.5rem",
              fontWeight: "bold",
              color: stockFilter === "all" ? "green" : "white",
            }}
          >
            {DummyData.filter((item) => item.stock > 0).length}
          </p>
        </div>
        <div
          style={{
            backgroundColor: stockFilter === "outOfStock" ? "#3b82f6" : "white",
            color: stockFilter === "outOfStock" ? "white" : "inherit",
            padding: "1rem",
            borderRadius: "0.5rem",
            boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            border: "1px solid #e5e7eb",
            cursor: "pointer",
          }}
          onClick={() => setStockFilter("outOfStock")}
        >
          <h3
            style={{
              color: stockFilter === "outOfStock" ? "white" : "#6b7280",
              marginBottom: "0.25rem",
            }}
          >
            محصولات ناموجود
          </h3>
          <p
            style={{
              fontSize: "1.5rem",
              fontWeight: "bold",
              color: stockFilter === "outOfStock" ? "white" : "#ef4444",
            }}
          >
            {DummyData.filter((item) => item.stock === 0).length}
          </p>
        </div>
      </div>

      <div
        style={{
          backgroundColor: "white",
          borderRadius: "0.5rem",
          boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
          padding: "1.5rem",
        }}
      >
        <div style={{ marginBottom: "1rem" }}>
          <input
            type="text"
            placeholder="جستجو در محصولات..."
            style={{
              width: "100%",
              padding: "0.5rem 1rem",
              border: "1px solid #d1d5db",
              borderRadius: "0.375rem",
              textAlign: "right",
            }}
          />
        </div>
        <Table data={filteredData} columns={columns} />
      </div>
    </div>
  );
}
