export default function MultivendorSettings() {
  const inputs = [
    {
      id: "vendorRegistration",
      label: "ثبت‌نام فروشنده",
      type: "checkbox",
      description: "فعال‌سازی یا غیرفعال‌سازی ثبت‌نام فروشندگان جدید",
    },
    {
      id: "commissionRate",
      label: "نرخ کمیسیون (%)",
      type: "number",
      description: "نرخ کمیسیون برای هر فروش انجام شده توسط فروشندگان",
    },
    {
      id: "vendorApproval",
      label: "تأیید فروشنده",
      type: "checkbox",
      description: "نیاز به تأیید مدیر برای فروشندگان جدید قبل از فعال‌سازی",
    },
    {
      id: "vendorDashboard",
      label: "داشبورد فروشنده",
      type: "checkbox",
      description:
        "فعال‌سازی داشبورد برای فروشندگان جهت مدیریت محصولات و سفارشات",
    },
    {
      id: "productApproval",
      label: "تأیید محصول",
      type: "checkbox",
      description: "نیاز به تأیید مدیر برای محصولات جدید قبل از نمایش در سایت",
    },
    {
      id: "vendorSupport",
      label: "پشتیبانی فروشنده",
      type: "checkbox",
      description:
        "فعال‌سازی پشتیبانی برای فروشندگان جهت پاسخ به سوالات و مشکلات",
    },
    {
      id: "vendorReports",
      label: "گزارش‌های فروشنده",
      type: "checkbox",
      description:
        "فعال‌سازی گزارش‌های فروش برای فروشندگان جهت مشاهده عملکرد خود",
    },
    {
      id: "vendorNotifications",
      label: "اعلان‌های فروشنده",
      type: "checkbox",
      description:
        "فعال‌سازی اعلان‌ها برای فروشندگان جهت اطلاع از سفارشات و پیام‌ها",
    },
  ];
  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">تنظیمات چندفروشنده</h1>
      <form>
        {inputs.map((input) => (
          <div key={input.id} className="mb-4">
            <label className="block text-sm font-medium mb-2">
              {input.label}
            </label>
            <input
              id={input.id}
              type={input.type}
              className="border rounded w-full p-2"
            />
            {input.description && (
              <p className="text-sm text-gray-500 mt-1">{input.description}</p>
            )}
          </div>
        ))}
        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          ذخیره تنظیمات
        </button>
      </form>
    </div>
  );
}
