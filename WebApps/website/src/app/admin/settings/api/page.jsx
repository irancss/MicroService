export default function ApiSettings() {
  const inputs = [
    {
      id: "apiKey",
      label: "کلید API",
      type: "text",
      description: "کلید API برای دسترسی به سرویس‌های مختلف",
    },
    {
      id: "apiSecret",
      label: "رمز API",
      type: "password",
      description: "رمز API برای امنیت بیشتر در دسترسی به سرویس‌ها",
    },
    {
      id: "rateLimit",
      label: "محدودیت نرخ درخواست‌ها",
      type: "number",
      description: "تعداد درخواست‌های مجاز در هر دقیقه",
    },
    {
      id: "enableApiAccess",
      label: "دسترسی به API را فعال کنید",
      type: "checkbox",
      description: "فعال‌سازی یا غیرفعال‌سازی دسترسی به API برای کاربران",
    },
    {
      id: "apiDocumentation",
      label: "مستندات API",
      type: "link",
      description: "لینک به مستندات API برای راهنمایی بیشتر",
      link: "/api-docs",
    },
  ];
  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">تنظیمات API</h1>
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
