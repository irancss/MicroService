"use client";
import { useState } from "react";
import Input from "@components/General/Input";
import toast from "react-hot-toast";
import QuillEditor from "@components/General/Quill";

export default function GeneralSettings() {
    const [isLoading, setIsLoading] = useState(false);
    const [formData, setFormData] = useState({
        siteName: "",
        siteDescription: "",
        supportEmail: "",
        supportPhone: "",
        siteLogo: null
    });

    const inputs = [
        {
            label: "نام سایت",
            name: "siteName",
            type: "text",
            placeholder: "نام سایت خود را وارد کنید",
            value: formData.siteName,
        },
        {
            label: "توضیحات سایت",
            name: "siteDescription",
            type: "textarea",
            placeholder: "توضیحات سایت خود را وارد کنید",
            value: formData.siteDescription,
        },
        {
            label: "ایمیل پشتیبانی",
            name: "supportEmail",
            type: "email",
            placeholder: "ایمیل پشتیبانی خود را وارد کنید",
            value: formData.supportEmail,
        },
        {
            label: "شماره تماس پشتیبانی",
            name: "supportPhone",
            type: "tel",
            placeholder: "شماره تماس پشتیبانی خود را وارد کنید",
            value: formData.supportPhone,
        },
        {
            label: "لوگو سایت",
            name: "siteLogo",
            type: "file",
            placeholder: "لوگوی سایت خود را آپلود کنید",
        },
        {
            label: "آیکون سایت (Favicon)",
            name: "siteFavicon",
            type: "file",
            placeholder: "آیکون سایت خود را آپلود کنید",
        },
    ];
    const checkBoxes = [
        {
            label: "کاربران باید نام‌نویسی کرده باشند و وارد شده باشند تا بتوانند دیدگاهشان را بنویسند",
            name: "requireLoginForComments",
            checked: false,
        },
        {
            label: "دیدگاهی نیازمند بررسی دارد",
            name: "commentsNeedReview",
            checked: false,
        },
        {
            label: "دیدگاه‌ها باید به صورت دستی تأیید شوند",
            name: "manualCommentApproval",
            checked: false,
        },
    ]   ;
    const textarea = [
        //Privacy
        {
            label: "قوانین و مقررات سایت",
            name: "termsAndConditions",
            placeholder: "قوانین و مقررات سایت خود را وارد کنید",
            value: "",
        },
        {
            label: "سیاست حفظ حریم خصوصی",
            name: "privacyPolicy",
            placeholder: "سیاست حفظ حریم خصوصی سایت خود را وارد کنید",
            value: "",
        },
    ]


    const handleInputChange = (e) => {
        const { name, value, files } = e.target;
        if (name === "siteLogo" && files?.length) {
            setFormData(prev => ({ ...prev, [name]: files[0] }));
        } else {
            setFormData(prev => ({ ...prev, [name]: value }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        
        try {
            // اینجا ارسال به API انجام می‌شود
            // const response = await fetch('/api/settings', {
            //   method: 'POST',
            //   body: JSON.stringify(formData)
            // });
            
            // ایجاد تاخیر برای شبیه‌سازی ارسال به سرور
            await new Promise(resolve => setTimeout(resolve, 1000));
            
            toast.success("تنظیمات با موفقیت ذخیره شد");
        } catch (error) {
            console.error("خطا در ذخیره تنظیمات:", error);
            toast.error("خطا در ذخیره تنظیمات");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <>
            <div className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto">
                <h2 className="text-2xl font-bold text-blue-700 mb-6">تنظیمات عمومی</h2>
                <hr className="border-b border-blue-200 mb-6" />
                <form className="space-y-6" onSubmit={handleSubmit}>
                    {inputs.map((input, index) => (
                        <Input
                            key={index}
                            label={input.label}
                            name={input.name}
                            type={input.type}
                            placeholder={input.placeholder}
                            value={input.value}
                            onChange={handleInputChange}
                        />
                    ))}

                    {checkBoxes.map((checkbox, index) => (
                        <div key={index} className="flex items-center space-x-2">
                            <input
                                type="checkbox"
                                name={checkbox.name}
                                id={checkbox.name}
                                checked={checkbox.checked}
                                onChange={(e) => {
                                    const { name, checked } = e.target;
                                    setFormData(prev => ({ ...prev, [name]: checked }));
                                }}
                                className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                            />
                            <label htmlFor={checkbox.name} className="text-gray-700">
                                {checkbox.label}
                            </label>
                        </div>
                    ))}

                    {textarea.map((text, index) => (
                        <div key={index} className="space-y-2">
                            <label className="block text-gray-700">{text.label}</label>
                            <QuillEditor
                                name={text.name}
                                placeholder={text.placeholder}
                                value={text.value}
                                onChange={(value) => {
                                    setFormData(prev => ({ ...prev, [text.name]: value }));
                                }}
                            />
                        </div>
                    ))}


                    <button
                        type="submit"
                        className={`bg-blue-600 text-white px-4 py-2 rounded-lg transition ${
                            isLoading ? "opacity-70 cursor-not-allowed" : "hover:bg-blue-700"
                        }`}
                        disabled={isLoading}
                    >
                        {isLoading ? "در حال ذخیره..." : "ذخیره تنظیمات"}
                    </button>
                </form>
            </div>
        </>
    );
}
