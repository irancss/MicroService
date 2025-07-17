"use client";
import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import FontAwesomeIcon from "@components/General/FontAwesomeIcon";
import Link from "next/link";
export default function SidebarAdmin() {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [activeTab, setActiveTab] = useState("داشبورد");
  const [expandedMenus, setExpandedMenus] = useState({});
  const tabs = [
    { title: "داشبورد", icon: "chart-line", href: "/admin/dashboard" },
    {
      title: "مدیریت محصولات",
      icon: "box",
      href: "/admin/products",
      subMenu: [
        { title: "لیست محصولات", icon: "boxes", href: "/admin/products" },
        { title: "افزودن محصول", icon: "circle-plus", href: "/admin/products/add" },
        { title: "تایید محصولات جدید", icon: "check", href: "/admin/products/approve" },
        { title: "محصولات ناموجود", icon: "box-open", href: "/admin/products/out-of-stock" },
        { title: "تگ ها", icon: "tags", href: "/admin/tags" },
        { title: "دسته بندی محصولات", icon: "folder", href: "/admin/categories" },
        { title: "برندها", icon: "copyright", href: "/admin/brands" },
        { title: "ویژگی‌ها", icon: "list", href: "/admin/attributes" },
        { title: "نظرات محصولات", icon: "comment", href: "/admin/products/comments" },
      ],
    },
    {
      title: "مدیریت سفارشات",
      icon: "cart-shopping",
      href: "/admin/orders",
      subMenu: [
        { title: "لیست سفارشات", icon: "cart-shopping", href: "/admin/orders" },
        { title: "افزودن سفارش", icon: "cart-plus", href: "/admin/orders/add" },
        { title: "سفارشات در انتظار", icon: "clock", href: "/admin/orders/pending" },
        { title: "سفارشات در حال پردازش", icon: "gear", href: "/admin/orders/processing" },
        { title: "سفارشات ارسال شده", icon: "truck", href: "/admin/orders/shipped" },
        { title: "سفارشات تحویل شده", icon: "circle-check", href: "/admin/orders/delivered" },
        { title: "سفارشات لغو شده", icon: "ban", href: "/admin/orders/canceled" },
        { title: "پیگیری سفارش", icon: "location-dot", href: "/admin/orders/tracking" },
        { title: "گزارش‌های فروش", icon: "chart-bar", href: "/admin/orders/reports" },
      ],
    },
    {
      title: "مدیریت فروشندگان",
      icon: "store",
      href: "/admin/vendors",
      subMenu: [
        { title: "لیست فروشندگان", icon: "store", href: "/admin/vendors" },
        { title: "درخواست‌های همکاری", icon: "handshake", href: "/admin/vendors/requests" },
        { title: "سطوح دسترسی", icon: "lock", href: "/admin/vendors/access-levels" },
        { title: "بسته‌های فروشندگی", icon: "box-open", href: "/admin/vendors/packages" },
        { title: "مدیریت محصولات فروشندگان", icon: "boxes", href: "/admin/vendors/products" },
        { title: "پیام‌های فروشندگان", icon: "envelope", href: "/admin/vendors/messages" },
        { title: "کیف پول فروشندگان", icon: "wallet", href: "/admin/vendors/wallets" },
        { title: "گزارشات فروشندگان", icon: "file", href: "/admin/vendors/reports" },
      ],
    },
    {
      title: "مدیریت کاربران",
      icon: "users",
      href: "/admin/customers",
      subMenu: [
        { title: "لیست مشتریان", icon: "users", href: "/admin/customers" },
        { title: "افزودن مشتری", icon: "user-plus", href: "/admin/customers/add" },
        { title: "پیام‌های مشتریان", icon: "envelope", href: "/admin/customers/messages" },
        { title: "امتیازات و وفاداری", icon: "medal", href: "/admin/customers/loyalty" },
        { title: "گزارش فعالیت کاربران", icon: "user-clock", href: "/admin/customers/activity-report" },
        { title: "سبد خریدهای رها شده", icon: "cart-shopping", href: "/admin/customers/abandoned-carts" },
        { title: "لیست علاقه‌مندی‌ها", icon: "heart", href: "/admin/customers/wishlists" },
        { title: "نظرات و نقدهای کاربران", icon: "star", href: "/admin/customers/reviews" },
        { title: "سطح دسترسی و نقش‌ها", icon: "lock", href: "/admin/customers/access-levels" },
      ],
    },
    {
      title: "تخفیف‌ها و کدها",
      icon: "percent",
      href: "/admin/discounts",
      subMenu: [
        { title: "کدهای تخفیف", icon: "tags", href: "/admin/discounts/codes" },
        { title: "کمپین‌ها", icon: "bullhorn", href: "/admin/discounts/campaigns" },
        { title: "پیشنهادات ویژه", icon: "gift", href: "/admin/discounts/special-offers" },
        { title: "تخفیف گروهی محصولات", icon: "percentage", href: "/admin/discounts/bulk" },
      ],
    },
    {
      title: "تیکت‌ ها",
      icon: "ticket",
      href: "/admin/tickets",
      subMenu: [
        { title: "لیست تیکت‌ ها", icon: "ticket", href: "/admin/tickets" },
        { title: "افزودن تیکت", icon: "circle-plus", href: "/admin/tickets/add" },
        { title: "تیکت‌های باز", icon: "envelope-open", href: "/admin/tickets/open" },
        { title: "تیکت‌های در حال بررسی", icon: "spinner", href: "/admin/tickets/in-progress" },
        { title: "تیکت‌های بسته شده", icon: "envelope", href: "/admin/tickets/closed" },
        { title: "دسته‌بندی تیکت‌ ها", icon: "folder", href: "/admin/tickets/categories" },
      ],
    },
    {
      title: "مالی و تسویه حساب",
      icon: "money-bill",
      href: "/admin/finance",
      subMenu: [
        { title: "گزارش فروش", icon: "file-invoice-dollar", href: "/admin/finance/sales-report" },
        { title: "کیف پول", icon: "wallet", href: "/admin/finance/wallet" },
        { title: "درآمد فروشندگان", icon: "hand-holding-dollar", href: "/admin/finance/vendor-income" },
        { title: "کمیسیون‌ها", icon: "percentage", href: "/admin/finance/commissions" },
        { title: "تسویه حساب فروشندگان", icon: "money-check", href: "/admin/finance/vendor-settlements" },
        { title: "درگاه‌های پرداخت", icon: "credit-card", href: "/admin/finance/payment-gateways" },
        { title: "تراکنش‌ ها", icon: "right-left", href: "/admin/finance/transactions" },
        { title: "درخواست‌های تسویه", icon: "money-bill-transfer", href: "/admin/finance/settlements" },
        { title: "گزارش مالی", icon: "file", href: "/admin/finance/report" },
        { title: "مالیات و عوارض", icon: "percent", href: "/admin/finance/tax" },
      ],
    },
    {
      title: "مدیریت محتوا",
      icon: "file",
      href: "/admin/content",
      subMenu: [
        { title: "مقالات", icon: "newspaper", href: "/admin/content/articles" },
        { title: "دسته‌بندی مقالات", icon: "folder", href: "/admin/content/categories" },
        { title: "بنرها", icon: "image", href: "/admin/content/banners" },
        { title: "اسلایدرها", icon: "sliders", href: "/admin/content/sliders" },
        { title: "صفحات", icon: "file", href: "/admin/content/pages" },
        { title: "منوها", icon: "bars", href: "/admin/content/menus" },
        { title: "پاپ‌آپ‌ها", icon: "window-restore", href: "/admin/content/popups" },
        { title: "نظرات مقالات", icon: "comments", href: "/admin/content/comments" },
      ],
    },
    {
      title: "مارکتینگ",
      icon: "bullhorn",
      href: "/admin/marketing",
      subMenu: [
        { title: "کمپین‌ها", icon: "bullhorn", href: "/admin/marketing/campaigns" },
        { title: "تحلیل‌ها", icon: "chart-line", href: "/admin/marketing/analytics" },
        { title: "ایمیل مارکتینگ", icon: "envelope", href: "/admin/marketing/email" },
        { title: "پیامک‌های تبلیغاتی", icon: "message", href: "/admin/marketing/sms" },
        { title: "خبرنامه", icon: "newspaper", href: "/admin/marketing/newsletter" },
        { title: "SEO", icon: "search", href: "/admin/marketing/seo" },
      ],
    },
    {
      title: "آنالیتیکس",
      icon: "chart-simple",
      href: "/admin/analytics",
      subMenu: [
        { title: "داشبورد آمار", icon: "gauge", href: "/admin/analytics/dashboard" },
        { title: "گزارش بازدید", icon: "eye", href: "/admin/analytics/visits" },
        { title: "گزارش فروش", icon: "chart-line", href: "/admin/analytics/sales" },
        { title: "رفتار کاربران", icon: "user-tag", href: "/admin/analytics/user-behavior" },
        { title: "منابع ترافیک", icon: "shuffle", href: "/admin/analytics/traffic-sources" },
      ],
    },
    {
      title: "تنظیمات",
      icon: "gear",
      href: "/admin/settings",
      subMenu: [
        { title: "عمومی", icon: "sliders", href: "/admin/settings/general" },
        { title: "درگاه پرداخت", icon: "credit-card", href: "/admin/settings/payment" },
        { title: "کش", icon: "database", href: "/admin/settings/caching" },
        { title: "اسکیما", icon: "sitemap", href: "/admin/settings/schema" },
        { title: "ارسال و حمل و نقل", icon: "truck", href: "/admin/settings/shipping" },
        { title: "پشتیبان‌گیری و بازیابی", icon: "floppy-disk", href: "/admin/settings/backup" },
        { title: "تنظیمات چندفروشندگی", icon: "store-alt", href: "/admin/settings/multivendor" },
        { title: "تنظیمات فروشگاه", icon: "bag-shopping", href: "/admin/settings/store" },
        { title: "کاربران و دسترسی‌ها", icon: "users-gear", href: "/admin/settings/users" },
        { title: "تنظیمات API", icon: "plug", href: "/admin/settings/api" },
        { title: "نوتیفیکیشن‌ها", icon: "bell", href: "/admin/settings/notifications" },
      ],
    },
    {
      title: "امنیت و لاگ‌ها",
      icon: "shield-halved",
      href: "/admin/security",
      subMenu: [
        { title: "بلاک کردن کاربران", icon: "user-slash", href: "/admin/security/block-users" },
        { title: "بلاک کردن IPها", icon: "ban", href: "/admin/security/block-ips" },
        { title: "بررسی فعالیت‌های مشکوک", icon: "triangle-exclamation", href: "/admin/security/suspicious-activity" },
        { title: "لاگ ورود/خروج کاربران", icon: "clock-rotate-left", href: "/admin/security/user-logs" },
        { title: "لاگ ورود/خروج مدیران", icon: "clipboard-list", href: "/admin/security/admin-logs" },
        { title: "تنظیمات امنیتی", icon: "shield-halved", href: "/admin/security/settings" },
      ],
    },
    {
      title: "گزارشات",
      icon: "chart-pie",
      href: "/admin/reports",
      subMenu: [
        { title: "گزارش کلی فروش", icon: "chart-simple", href: "/admin/reports/sales" },
        { title: "گزارش محصولات", icon: "box", href: "/admin/reports/products" },
        { title: "گزارش فروشندگان", icon: "store", href: "/admin/reports/vendors" },
        { title: "گزارش مشتریان", icon: "users", href: "/admin/reports/customers" },
        { title: "گزارش موجودی", icon: "warehouse", href: "/admin/reports/inventory" },
      ],
    },
    {
      title: "راهنما و پشتیبانی",
      icon: "circle-question",
      href: "/admin/help",
      subMenu: [
        { title: "راهنمای استفاده", icon: "book", href: "/admin/help/guide" },
        { title: "تماس با پشتیبانی", icon: "headset", href: "/admin/help/support" },
        { title: "آموزش‌ها", icon: "video", href: "/admin/help/tutorials" },
      ],
    },
    { title: "خروج", icon: "right-from-bracket", href: "/logout" },
  ];

  const toggleSubmenu = (e, title) => {
    e.stopPropagation();
    setExpandedMenus((prev) => ({
      ...prev,
      [title]: !prev[title],
    }));
  };

  return (
    <motion.div
      className={`${
        isCollapsed ? "w-16" : "w-64"
      } rounded-xl bg-gradient-to-b from-gray-900 via-black to-gray-800 shadow-lg text-white h-screen transition-width duration-300 ease-in-out overflow-y-auto`}
      animate={{ width: isCollapsed ? "4rem" : "16rem" }}
      initial={false}
    >
      <div className="flex justify-between items-center p-4">
        {!isCollapsed && <h1 className="text-xl font-bold">پنل مدیریت</h1>}
        <button
          onClick={() => setIsCollapsed(!isCollapsed)}
          className="text-white hover:text-gray-300"
        >
          {isCollapsed ? "→" : "←"}
        </button>
      </div>
      <ul className="mt-4">
        {tabs.map((tab) => (
          <li key={tab.title}>
            <motion.div
              className={`flex items-center justify-between p-2 rounded-lg hover:bg-blue-900/60 transition-all duration-200 cursor-pointer ${
                activeTab === tab.title
                  ? "bg-gradient-to-l from-blue-700 via-blue-800 to-blue-900 border-r-4 border-blue-500 shadow-lg"
                  : ""
              }`}
              onClick={(e) => {
                setActiveTab(tab.title);
                if (tab.subMenu) toggleSubmenu(e, tab.title);
              }}
              whileTap={{ scale: 0.98 }}
            >
              <div className="flex items-center">
                <FontAwesomeIcon
                  icon={tab.icon}
                  className={`m-2 text-white ${isCollapsed ? "" : "mr-2"}`}
                />
                {!isCollapsed && <span>{tab.title}</span>}
              </div>
              {!isCollapsed && tab.subMenu && (
                <span className="ml-2">
                  <FontAwesomeIcon
                    icon={
                      expandedMenus[tab.title]
                        ? "chevron-down"
                        : "chevron-right"
                    }
                  />
                </span>
              )}
            </motion.div>
            {!isCollapsed && tab.subMenu && (
              <AnimatePresence>
                {expandedMenus[tab.title] && (
                  <motion.ul
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: "auto", opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    transition={{ duration: 0.3 }}
                    className="bg-gray-950/80 rounded-lg my-1 overflow-hidden"
                  >
                    {tab.subMenu.map((subItem) => (
                      <Link key={subItem.title} href={subItem.href}>
                        <motion.li
                          key={subItem.title}
                          className="flex items-center p-2 pl-6 hover:bg-blue-900/60 transition-all duration-200 cursor-pointer"
                          whileHover={{ scale: 1.01 }}
                          whileTap={{ scale: 0.99 }}
                          onClick={() => setActiveTab(subItem.title)}
                        >
                          <FontAwesomeIcon
                            icon={subItem.icon}
                            className="m-2 text-white"
                          />
                          <span>{subItem.title}</span>
                        </motion.li>
                      </Link>
                    ))}
                  </motion.ul>
                )}
              </AnimatePresence>
            )}
          </li>
        ))}
      </ul>
    </motion.div>
  );
}
