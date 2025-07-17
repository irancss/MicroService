
export default function StoreSettings() {
    const inputs = [
        {
            id: "storeName",
            label: "نام فروشگاه",
            type: "text",
            description: "نام فروشگاه شما برای نمایش در سایت و برنامه‌ها",
        },
        {
            id: "storeDescription",
            label: "توضیحات فروشگاه",
            type: "textarea",
            description: "توضیحاتی درباره فروشگاه شما برای جذب مشتریان",
        },
        {
            id: "currency",
            label: "واحد پول",
            type: "text",
            description: "واحد پول مورد استفاده در فروشگاه (مثال: تومان)",
        },
        {
            id: "taxRate",
            label: "نرخ مالیات (%)",
            type: "number",
            description: "نرخ مالیات بر اساس درصد برای محاسبه مالیات بر فروش",
        },
        {
            id: "shippingOptions",
            label: "گزینه‌های حمل و نقل",
            type: "text",
            description: "لیست گزینه‌های حمل و نقل موجود (مثال: پست، تیپاکس)",
        },
    ];

    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold mb-4">تنظیمات فروشگاه</h1>
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
                            <p className="text-sm text-gray-500 mt-1">
                                {input.description}
                            </p>
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