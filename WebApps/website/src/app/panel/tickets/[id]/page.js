"use client";
import AnimatedHr from "@/app/components/Animated/Hr";
import { useState } from "react";

export default function TicketPage() {
    const initialTicket = {
        id: 1,
        questionText: "این یک سوال نمونه است.",
        answerText: "این پاسخ ادمین به سوال شماست.",
        state: "منتظر تایید",
        time: "1403/03/01",
        isRead: false,
    };
    const [ticket] = useState(initialTicket);

    return (
        <div className="max-w-xl mx-auto mt-10">
            <AnimatedHr className="mb-6" />
            <div className="bg-white/80 backdrop-blur-md mb-8 p-7 rounded-2xl shadow-xl border border-gray-100">
                <div className="flex justify-between items-center mb-4">
                    <span
                        className={`
                            px-4 py-1 rounded-full font-bold text-sm shadow
                            ${
                                ticket.state === "منتظر تایید"
                                    ? "bg-orange-500 text-white"
                                    : ticket.state === "رد شده"
                                    ? "bg-red-500 text-white"
                                    : "bg-green-600 text-white"
                            }
                        `}
                    >
                        {ticket.state}
                    </span>
                    <span className="text-gray-400 text-xs">{ticket.time}</span>
                    <span
                        className={`text-xs font-semibold ${
                            ticket.isRead ? "text-green-600" : "text-red-500"
                        }`}
                    >
                        {ticket.isRead ? "خوانده شده" : "خوانده نشده"}
                    </span>
                </div>
                <div className="flex flex-col gap-8">
                    <div className="text-right flex flex-col items-end">
                        <span className="block bg-gradient-to-l from-blue-100 to-white rounded-xl p-4 shadow text-gray-800 text-base font-medium border border-blue-100">
                            {ticket.questionText}
                        </span>
                        <span className="block text-xs text-gray-400 mt-2">کاربر</span>
                    </div>
                    <div className="text-left flex flex-col items-start">
                        <span className="block bg-gradient-to-r from-green-100 to-white rounded-xl p-4 shadow text-gray-800 text-base font-medium border border-green-100">
                            {ticket.answerText}
                        </span>
                        <span className="block text-xs text-gray-400 mt-2">ادمین</span>
                    </div>
                </div>
            </div>
        </div>
    );
}
