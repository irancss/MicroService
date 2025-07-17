
export default function PaymentSettings() {
  const inputs = [
    {
      id: "paymentGateway",
      label: "درگاه پرداخت",
      type: "select",
      options: ["زرین‌پال", "پی‌پال", "Stripe", "بانک ملت"],
      description: "انتخاب درگاه پرداخت مورد نظر برای تراکنش‌ها",
    },
    {
      id: "currency",
      label: "واحد پول",
      type: "text",
      description: "واحد پول مورد استفاده در سایت (مثال: IRR, USD)",
    },
    {
      id: "transactionFee",
      label: "هزینه تراکنش (%)",
      type: "number",
      description: "درصد هزینه تراکنش برای هر پرداخت انجام شده",
    },
    {
      id: "minPaymentAmount",
        label: "حداقل مبلغ پرداخت",
        type: "number",
        description: "حداقل مبلغ مجاز برای پرداخت در هر تراکنش",
    },
    {
      id: "maxPaymentAmount",
      label: "حداکثر مبلغ پرداخت",
      type: "number",
      description: "حداکثر مبلغ مجاز برای پرداخت در هر تراکنش",
    },
    {
      id: "refundPolicy",
      label: "سیاست بازپرداخت",
      type: "textarea",
      description:
        "توضیح سیاست بازپرداخت و شرایط لازم برای درخواست بازپرداخت",
    },
    {
      id: "paymentNotifications",
      label: "اعلان‌های پرداخت",
      type: "checkbox",
      description:
        "فعال‌سازی اعلان‌ها برای اطلاع‌رسانی به کاربران درباره وضعیت پرداخت‌ها",
    },
  ];

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">تنظیمات پرداخت</h1>
      <form>
        {inputs.map((input) => (
          <div key={input.id} className="mb-4">
            <label className="block text-sm font-medium mb-2">
              {input.label}
            </label>
            {input.type === 'select' ? (
              <select
                id={input.id}
                className="border rounded w-full p-2"
              >
                {input.options.map((option) => (
                  <option key={option} value={option}>
                    {option}
                  </option>
                ))}
              </select>
            ) : (
              <input
                id={input.id}
                type={input.type}
                className="border rounded w-full p-2"
              />
            )}
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
