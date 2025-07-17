import { useState } from "react";

const options = [
    {
        name: "sendSmsOnStockChange",
        label: "ارسال پیامک هنگام تغییر موجودی",
    },
    {
        name: "notifyOutOfStock",
        label: "اطلاع‌رسانی هنگام اتمام موجودی",
    },
    {
        name: "notifyInStock",
        label: "اطلاع‌رسانی هنگام شارژ مجدد موجودی",
    },
    {
        name: "notifyNewProduct",
        label: "اطلاع‌رسانی محصول جدید",
    },
];

export default function Notifications({ value = {}, onChange }) {
    const [notifications, setNotifications] = useState({
        sendSmsOnStockChange: value.sendSmsOnStockChange || false,
        notifyOutOfStock: value.notifyOutOfStock || false,
        notifyInStock: value.notifyInStock || false,
        notifyNewProduct: value.notifyNewProduct || false,
    });

    const handleChange = (e) => {
        const { name, checked } = e.target;
        const updated = { ...notifications, [name]: checked };
        setNotifications(updated);
        if (onChange) onChange(updated);
    };

    return (
     <div >
                  <h2 className="text-2xl font-bold text-blue-700 mb-6 flex items-center gap-2">
                <svg width="28" height="28" fill="none" viewBox="0 0 24 24"><path fill="#2563eb" d="M12 2a10 10 0 100 20 10 10 0 000-20zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/></svg>
                تنظیمات اطلاع‌ رسانی
            </h2>
            <div className="space-y-4">
                {options.map((option) => (
                    <label
                        key={option.name}
                        className="flex items-center cursor-pointer transition hover:bg-gray-50 rounded px-2 py-2 pe-2"
                    >
                        <input
                            type="checkbox"
                            name={option.name}
                            className="form-checkbox h-5 w-5 text-blue-600 transition duration-150 ml-3 mr-3"
                            checked={notifications[option.name]}
                            onChange={handleChange}
                        />
                        <span className="text-gray-800">{option.label}</span>
                    </label>
                ))}
            </div>
        </div>
    );
}
