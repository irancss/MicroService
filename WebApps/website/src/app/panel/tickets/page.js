"use client";
import React from "react";
import AnimatedHr from "@components/Animated/Hr";
import Link from "next/link";

const initialTickets = [
    {
        id: 1,
        title: "مشکل در پرداخت",
        description: "در هنگام پرداخت، خطا دریافت می‌کنم.",
        status: "در حال بررسی",
        link: "/panel/tickets/1",
        date: "1403/03/01",
    },
    {
        id: 2,
        title: "سفارش من ارسال نشده است",
        description: "سفارش من هنوز به دستم نرسیده است.",
        status: "پاسخ داده شده",
        link: "/panel/tickets/2",
        date: "1403/03/02",
    },
    {
        id: 3,
        title: "مشکل در ورود به حساب کاربری",
        description: "نمی‌توانم وارد حساب کاربری خود شوم.",
        status: "بسته شده",
        link: "/panel/tickets/3",
        date: "1403/03/03",
    },
];

function getStatusClass(status) {
    switch (status) {
        case "در حال بررسی":
            return "bg-orange-500 text-white";
        case "پاسخ داده شده":
            return "bg-green-600 text-white";
        case "بسته شده":
            return "bg-red-500 text-white";
        default:
            return "";
    }
}

export default function TicketsPage() {
    return (
        <div>
            <h2 className="font-bold text-2xl mb-3">تیکت‌های من</h2>
            <AnimatedHr className="mb-3" />
            <ul className="list-none p-0">
                {initialTickets.map((ticket) => (
                    <li
                        key={ticket.id}
                        className="mb-6 p-5 rounded-xl bg-gray-100 shadow-md"
                    >
                        <h3 className="mb-2 text-gray-800 font-semibold">{ticket.title}</h3>
                        <p className="mb-3 text-gray-600">{ticket.description}</p>
                        <div className="flex justify-between items-center">
                            <span
                                className={`px-3 py-1 rounded-lg font-bold text-sm ${getStatusClass(ticket.status)}`}
                            >
                                {ticket.status}
                            </span>
                            <span className="text-gray-500 text-sm">{ticket.date}</span>
                            <Link href={ticket.link} className="text-blue-500 hover:underline">
                                مشاهده
                            </Link>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    );
}
