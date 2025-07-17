"use client";
import { useState } from "react";
import { useRouter } from "next/compat/router";
export default function Document() {
  const [selectedDoc, setSelectedDoc] = useState(null);
  const router = useRouter();
  const { id } = router?.query || {};

  const buttons = [
    // برگه فاکتور - برگه انبارداری - برگه برچسب پستی - برگه برچسب محصول
    {
      id: 1,
      label: "برگه فاکتور",
      icon: "document-text",
      href: "/admin/orders/invoice/[id]",
    },
    {
      id: 2,
      label: "برگه انبارداری",
      icon: "archive-box",
      href: "/admin/orders/warehouse/[id]",
    },
    {
      id: 3,
      label: "برچسب پستی",
      icon: "tag",
      href: "/admin/orders/shipping-label/[id]",
    },
    {
      id: 4,
      label: "برچسب محصول",
      icon: "tag",
      href: "/admin/orders/product-label/[id]",
    },
  ];

  const handleDocumentClick = (button) => {
    setSelectedDoc(button);
    const url = button.href.replace("[id]", id);
    router.push(url);
  };

  return (
    <div className="p-4  rounded-lg  w-1/3">
      <h2 className="text-xl font-bold mb-4">
        اسناد سفارش {selectedDoc?.label && `- ${selectedDoc.label}`}
      </h2>
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {buttons.map((button) => (
          <button
            key={button.id}
            onClick={() => handleDocumentClick(button)}
            disabled={!id}
            className={`flex flex-col items-center justify-center p-4 border rounded-lg ${
              !id ? "opacity-50 cursor-not-allowed" : "hover:bg-gray-50"
            } transition-colors`}
          >
            <div className="w-12 h-12 flex items-center justify-center bg-blue-100 rounded-full mb-2">
              <i className={`bi bi-${button.icon} text-xl text-blue-600`}></i>
            </div>
            <span className="text-sm font-medium">{button.label}</span>
          </button>
        ))}
      </div>
    </div>
  );
}
