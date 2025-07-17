import Input from "@components/General/Input";

export default function SecuritySettingsPage() {
    const inputs = [
        {
            id: "allowedIp",
            label: "آدرس IP مجاز",
            type: "text",
            placeholder: "مثال: 192.168.1.100 یا 192.168.1.0/24",
        },
        {
            id: "accountLockoutDuration",
            label: "مدت زمان قفل حساب (دقیقه)",
            type: "number",
            placeholder: "مدت زمان (دقیقه)",
        },
        {
            id: "accessTokenLifetime",
            label: "طول عمر توکن دسترسی (ثانیه)",
            type: "number",
            placeholder: "مثال: 3600 (1 ساعت)",
        },
        {
            id: "identityTokenLifetime",
            label: "طول عمر توکن شناسایی (ثانیه)",
            type: "number",
            placeholder: "مثال: 300 (5 دقیقه)",
        },
        {
            id: "refreshTokenLifetime",
            label: "طول عمر توکن تازه‌سازی (ثانیه) - مطلق",
            type: "number",
            placeholder: "مثال: 2592000 (30 روز)",
        },
        {
            id: "refreshTokenSlidingLifetime",
            label: "طول عمر توکن تازه‌سازی (ثانیه) - متحرک",
            type: "number",
            placeholder: "مثال: 1296000 (15 روز)",
        },
        {
            id: "maxFailedAccessAttempts",
            label: "حداکثر تلاش‌های ناموفق دسترسی",
            type: "number",
            placeholder: "حداکثر تلاش‌ها",
        },
        {
            id: "requireTwoFactor",
            label: "احراز هویت دو عاملی",
            type: "checkbox",
        },
        {
            id: "userSsoLifetime",
            label: "طول عمر نشست SSO کاربر (ثانیه)",
            type: "number",
            placeholder: "مثال: 86400 (1 روز)",
        },
        {
            id: "requireConsent",
            label: "نیاز به رضایت کاربر",
            type: "checkbox",
        },
        {
            id: "allowOfflineAccess",
            label: "اجازه دسترسی آفلاین (Refresh Token)",
            type: "checkbox",
        },
        {
            id: "updateAccessTokenClaimsOnRefresh",
            label: "به‌روزرسانی Claimهای توکن دسترسی هنگام Refresh",
            type: "checkbox",
        },
        {
            id: "alwaysIncludeUserClaimsInIdToken",
            label: "همیشه Claimهای کاربر در توکن شناسایی گنجانده شود",
            type: "checkbox",
        },
        {
            id: "cookieLifetime",
            label: "طول عمر کوکی احراز هویت (ثانیه)",
            type: "number",
            placeholder: "مثال: 2592000 (30 روز)",
        },
        {
            id: "cookieSlidingExpiration",
            label: "انقضای متحرک کوکی احراز هویت",
            type: "checkbox",
        },
        {
            id: "requirePkce",
            label: "الزامی بودن PKCE برای Code Flow",
            type: "checkbox",
        },
    ];

    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold mb-4">تنظیمات امنیتی</h1>
            <form>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    {/* Existing Fields */}
                    {inputs.map((input) => (
                        <div className="mb-4" key={input.id}>
                            <label
                                htmlFor={input.id}
                                className="block text-sm font-medium text-gray-700"
                            >
                                {input.label}
                            </label>
                            {input.type !== "checkbox" ? (
                                <Input
                                    type={input.type}
                                    id={input.id}
                                    placeholder={input.placeholder}
                                    className="mt-1 block w-full    p-2"
                                />
                            ) : (
                                <div className="mt-1 flex items-center">
                                    <input
                                        type="checkbox"
                                        id={input.id}
                                        className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                                    />
                                    <label
                                        htmlFor={input.id}
                                        className="ml-2 block text-sm text-gray-900"
                                    >
                                        {input.id === "requireTwoFactor"
                                            ? "الزامی بودن احراز هویت دو عاملی برای کاربران"
                                            : input.label}
                                    </label>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            </form>
        </div>
    );
}
