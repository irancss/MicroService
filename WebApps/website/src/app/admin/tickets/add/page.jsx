"use client";
import Input from "@components/General/Input";
import Select from "@components/General/Select";
import { useState } from "react";
export default function AddTicket() {
    // Search User and then set the userId in the formData
    const initialFormData = {
        userId: "",
        subject: "",
        description: "",
        priority: "normal", // Default priority
    };
    const [formData, setFormData] = useState(initialFormData);
    const [searchQuery, setSearchQuery] = useState("");
    const [userId, setUserId] = useState("");

    const handleSearch = (e) => {
        e.preventDefault();
        // Logic to search for the user and set the userId
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Logic to submit the form
    };

    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold mb-4">ایجاد تیکت جدید</h1>
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block mb-1">جستجوی کاربر</label>
                    <input
                        type="text"
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="border border-gray-300 p-2 rounded w-full"
                    />
                    <button
                        onClick={handleSearch}
                        className="bg-blue-500 text-white px-4 py-2 rounded mt-2"
                    >
                        جستجو
                    </button>
                </div>
              
                <Select
                    label="موضوع"
                    name="subject"
                    options={[
                        { value: "issue", label: "مشکل" },
                        { value: "question", label: "سوال" },
                        { value: "feedback", label: "بازخورد" },
                    ]} 
                    value={formData.subject}
                    onChange={handleInputChange}
                />
                <Input
                    label="توضیحات"
                    name="description"
                    value={formData.description}
                    onChange={handleInputChange}
                />
                <Select
                    label="اولویت"
                    name="priority"
                    options={[
                        { value: "low", label: "پایین" },
                        { value: "normal", label: "متوسط" },
                        { value: "high", label: "بالا" },
                    ]}
                    value={formData.priority}
                    onChange={(value) => setFormData((prev) => ({ ...prev, priority: value }))}
                />
                <button
                    type="submit"
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                >
                    ایجاد تیکت
                </button>
            </form>
        </div>
    );
}
