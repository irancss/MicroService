"use client";
import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import FontAwesomeIcon from "../General/FontAwesomeIcon";

export default function SidebarSeller() {
  const options = [
    { name: "داشبورد", icon: "gauge", href: "/seller" },
    {
      name: "محصولات",
      icon: "box",
      items: [
        { name: "افزودن محصول", icon: "plus", href: "/seller/products/add" },
        {
          name: "مدیریت محصولات",
          icon: "cogs",
          href: "/seller/products",
        },
      ],
    },
    {
      name: "سفارش",
      icon: "shopping-cart",
      items: [
        {
          name: "سفارش های تامین شده",
          icon: "cogs",
          href: "/seller/orders/supplied",
        },
        {
          name: "سفارش در انتظار تامین",
          icon: "history",
          href: "/seller/orders/pending",
        },
        {
          name: "سفارش های ارسال فروشنده",
          icon: "ban",
          href: "/seller/orders/sent",
        },
      ],
    },
    {
      name: "محموله",
      icon: "chart-line",
      items: [
        {
          name: "ایجاد محموله",
          icon: "truck",
          href: "/seller/shipments/create",
        },
        {
          name: "لیست محموله ها",
          icon: "check",
          href: "/seller/shipments/list",
        },
        { name: "آدرس های من", icon: "check", href: "/seller/addresses" },
      ],
    },
    {
      name: "لوجستیک",
      icon: "cog",
      items: [
        {
          name: "روش های ارسال",
          icon: "check",
          href: "/seller/logistics/methods",
        },
        {
          name: "لیست بسته بندی",
          icon: "check",
          href: "/seller/logistics/packages",
        },
      ],
    },
    {
      name: "گزارشات",
      icon: "chart-bar",
      items: [
        { name: "گزارش کالا", icon: "check", href: "/seller/reports/products" },
        {
          name: "گزارش موجودی و قیمت",
          icon: "check",
          href: "/seller/reports/inventory",
        },
        {
          name: "گزارش مرجوعی",
          icon: "check",
          href: "/seller/reports/returns",
        },
        { name: "گزارش فروش", icon: "check", href: "/seller/reports/sales" },
        {
          name: "گزارش موجودی انبارها",
          icon: "check",
          href: "/seller/reports/warehouses",
        },
      ],
    },
    {
      name: "تیکت",
      icon: "comment",
      items: [
        { name: "تیکت های من", icon: "check", href: "/seller/tickets" },
        { name: "ایجاد تیکت", icon: "check", href: "/seller/tickets/create" },
      ],
    },
    {
      name: "اعلانات",
      icon: "bell",
      items: [
        { name: "اعلانات من", icon: "check", href: "/seller/notifications" },
      ],
    },
  ];

  const [openMenus, setOpenMenus] = useState({});

  const toggleMenu = (name) => {
    setOpenMenus((prev) => ({
      ...prev,
      [name]: !prev[name],
    }));
  };

  const renderOption = (option) => {
    const isOpen = openMenus[option.name];

    if (!option.items) {
      return (
        <li key={option.name}>
          <a
            href={option.href}
            className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-gray-100 transition text-gray-700 text-base font-medium"
          >
            <FontAwesomeIcon
              icon={option.icon}
              className="text-primary-500 text-lg"
            />
            <span>{option.name}</span>
          </a>
        </li>
      );
    }

    return (
      <li key={option.name}>
        <button
          className="flex items-center gap-3 w-full px-3 py-2 rounded-lg hover:bg-gray-100 transition text-gray-700 text-base font-medium focus:outline-none"
          onClick={() => toggleMenu(option.name)}
          type="button"
        >
          <FontAwesomeIcon
            icon={option.icon}
            className="text-primary-500 text-lg"
          />
          <span>{option.name}</span>
          <FontAwesomeIcon
            icon={isOpen ? "fa-chevron-up" : "fa-chevron-down"}
            className="text-gray-400 text-xs"
          />
        </button>
        <AnimatePresence>
          {isOpen && (
            <motion.ul
              className="pl-6 mt-1 border-l-2 border-gray-100"
              initial={{ height: 0, opacity: 0 }}
              animate={{ height: "auto", opacity: 1 }}
              exit={{ height: 0, opacity: 0 }}
              transition={{ duration: 0.2 }}
            >
              {option.items.map((item) => (
                <motion.li
                  key={item.name}
                  initial={{ opacity: 0, x: -10 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -10 }}
                  transition={{ duration: 0.15 }}
                >
                  <button
                    onClick={() => (window.location.href = item.href)}
                    className="flex items-center gap-2 px-2 py-1 rounded hover:bg-gray-50 transition text-gray-500 text-sm w-full text-right"
                    type="button"
                  >
                    <FontAwesomeIcon icon={item.icon} className="text-xs" />
                    <span>{item.name}</span>
                  </button>
                </motion.li>
              ))}
            </motion.ul>
          )}
        </AnimatePresence>
      </li>
    );
  };

  // Sample store name, replace with dynamic value as needed
  const storeName = "فروشگاه من";

  return (
    <aside
      className="min-h-screen bg-white border-r border-gray-100 shadow-sm flex flex-col transition-all duration-300"
      style={{
        width: openMenus.sidebar === false ? 80 : 256, // 80px (w-20) or 256px (w-64)
        transition: "width 0.3s cubic-bezier(0.4,0,0.2,1)",
        overflow: "hidden",
      }}
    >
      <div className="p-6 border-b border-gray-100 flex items-center justify-between">
        <h2
          className={`text-xl font-bold text-gray-800 text-center flex-1 transition-opacity duration-200 ${
            openMenus.sidebar === false
              ? "opacity-0 pointer-events-none"
              : "opacity-100"
          }`}
        >
          {storeName}
        </h2>
        <button
          className="ml-2 p-2 rounded hover:bg-gray-100 transition"
          onClick={() =>
            setOpenMenus((prev) => ({ ...prev, sidebar: !prev.sidebar }))
          }
          type="button"
        >
          <FontAwesomeIcon
            icon={openMenus.sidebar ? "fa-chevron-right" : "fa-chevron-left"}
            className="text-gray-500"
          />
        </button>
      </div>
      {openMenus.sidebar !== false && (
        <nav className="flex-1 overflow-y-auto">
          <ul className="p-4 space-y-1">
            {options.map((option) => renderOption(option))}
          </ul>
        </nav>
      )}
      {openMenus.sidebar === false && (
        <nav className="flex-1 overflow-y-auto">
          <ul className="p-4 space-y-1">
            {options.map((option) =>
              !option.items ? (
                <li key={option.name} className="flex justify-center">
                  <a
                    href={option.href}
                    className="flex items-center justify-center px-3 py-2 rounded-lg hover:bg-gray-100 transition text-gray-700 text-base font-medium"
                    title={option.name}
                  >
                    <FontAwesomeIcon
                      icon={option.icon}
                      className="text-primary-500 text-lg"
                    />
                  </a>
                </li>
              ) : (
                <li key={option.name} className="flex justify-center">
                  <button
                    className="flex items-center justify-center w-full px-3 py-2 rounded-lg hover:bg-gray-100 transition text-gray-700 text-base font-medium focus:outline-none"
                    type="button"
                    title={option.name}
                  >
                    <FontAwesomeIcon
                      icon={option.icon}
                      className="text-primary-500 text-lg"
                    />
                  </button>
                </li>
              )
            )}
          </ul>
        </nav>
      )}
    </aside>
  );
}
