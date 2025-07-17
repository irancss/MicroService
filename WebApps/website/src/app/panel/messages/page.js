

"use client";
import AnimatedHr from "@components/Animated/Hr";
import React, { useState } from "react";
export default function MessagesPage() {
    const initialMessages = [
        {
            id: 1,
            sender: "کاربر 1",
            messageText: "هزینه کالا یا سفارشی که لغو شده را به اعتبار دیجی پی شما برگردوندیم.",
            time: "1403/03/01",
        },
        {
            id: 2,
            sender: "کاربر 2",
            messageText: "آیا این محصول موجود است؟",
            time: "1403/03/02",
        },
        {
            id: 3,
            sender: "کاربر 3",
            messageText: "ممنون از خدمات شما.",
            time: "1403/03/03",
        },
    ];

    const [messages] = useState(initialMessages);

    return (
        <div>
            <h2 className="font-bold text-2xl mb-3">پیام‌های کاربران</h2>
            <AnimatedHr className="mb-3" />
            <ul className="list-none p-0">
                {messages.map((message) => (
                    <li
                        key={message.id}
                        className="mb-6 p-5 rounded-xl bg-gray-100 shadow-md"
                    >
                        <h3 className="mb-2 text-gray-800 font-semibold">{message.sender}</h3>
                        <p className="mb-3 text-gray-600">{message.messageText}</p>
                        <span className="text-gray-500 text-sm">{message.time}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
}   