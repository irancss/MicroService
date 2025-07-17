

export default function NotificationsSettings() {
const inputs = [
    {
    id: "emailNotifications",
    label: "اعلان‌های ایمیلی",
    type: "checkbox",
    description: "فعال‌سازی یا غیرفعال‌سازی اعلان‌های ایمیلی",
    },
    {
    id: "pushNotifications",
    label: "اعلان‌های پوش نوتیفیکیشن",
    type: "checkbox",
    description: "فعال‌سازی یا غیرفعال‌سازی اعلان‌های پوش نوتیفیکیشن",
    },
    {
    id: "smsNotifications",
    label: "اعلان‌های پیامکی",
    type: "checkbox",
    description: "فعال‌سازی یا غیرفعال‌سازی اعلان‌های پیامکی",
    },
    {
    id: "newFollowerNotifications",
    label: "اعلان دنبال‌کننده جدید",
    type: "checkbox",
    description: "دریافت اعلان هنگام دنبال شدن توسط کاربر جدید",
    },
    {
    id: "commentNotifications",
    label: "اعلان دیدگاه جدید",
    type: "checkbox",
    description: "دریافت اعلان برای دیدگاه‌های جدید روی پست‌های شما",
    },
    {
    id: "mentionNotifications",
    label: "اعلان منشن شدن",
    type: "checkbox",
    description: "دریافت اعلان هنگامی که در یک دیدگاه یا پست منشن می‌شوید",
    },
    {
    id: "newsletterNotifications",
    label: "اعلان خبرنامه",
    type: "checkbox",
    description: "اشتراک در خبرنامه برای دریافت آخرین به‌روزرسانی‌ها",
    },
];
    
    return (
        <div className="p-4">
        <h1 className="text-2xl font-bold mb-4">تنظیمات اعلان‌ها</h1>
        <form>
            {inputs.map((input) => (
            <div key={input.id} className="mb-4 ">
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
