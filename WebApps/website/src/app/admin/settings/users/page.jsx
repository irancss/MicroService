
export default function UsersSettings() {
    const inputs = [
        {
        id: "username",
        label: "نام کاربری",
        type: "text",
        description: "نام کاربری منحصر به فرد برای ورود به سیستم",
        },
        {
        id: "email",
        label: "ایمیل",
        type: "email",
        description: "ایمیل معتبر برای دریافت اعلان‌ها و بازیابی رمز عبور",
        },
        {
        id: "password",
        label: "رمز عبور",
        type: "password",
        description: "رمز عبور قوی برای امنیت حساب کاربری",
        },
        {
        id: "profilePicture",
        label: "تصویر پروفایل",
        type: "file",
        description: "آپلود تصویر پروفایل برای نمایه کاربری شما",
        },
        {
        id: "bio",
        label: "بیوگرافی",
        type: "textarea",
        description: "توضیح کوتاه درباره خودتان (اختیاری)",
        },
    ];
    
    return (
        <div className="p-4">
        <h1 className="text-2xl font-bold mb-4">تنظیمات کاربران</h1>
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