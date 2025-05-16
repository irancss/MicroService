"use client";
import AnimatedHr from "@/app/components/Animated/Hr";
import React, { useState } from "react";

export default function QuestionsPage() {
    const initialQuestions = [
        {
            id: 1,
            questionText: "سوال 1",
            answerText: "پاسخ 1",
            state: "منتظر تایید",
            time: "1403/03/01",
        },
        {
            id: 2,
            questionText: "سوال 2",
            answerText: "پاسخ 2",
            state: "رد شده",
            time: "1403/03/02",
        },
        {
            id: 3,
            questionText: "سوال 3",
            answerText: "پاسخ 3",
            state: "تایید شده",
            time: "1403/03/03",
        },
    ];

    const [questions] = useState(initialQuestions);

    const stateColors = {
        "منتظر تایید": "#ff9800",
        "رد شده": "#f44336",
        "تایید شده": "#4caf50",
    };

    return (
        <div>
            <h2 className="font-bold text-2xl mb-3">سوالات کاربران</h2>
            <AnimatedHr className="mb-3" />
            <ul className="list-none p-0">
                {questions.map((question) => (
                    <li
                        key={question.id}
                        className="mb-6 p-5 rounded-xl bg-gray-100 shadow-md"
                    >
                        <h3 className="mb-2 text-gray-800 font-semibold">{question.questionText}</h3>
                        <p className="mb-3 text-gray-600">{question.answerText}</p>
                        <div className="flex justify-between items-center">
                            <span
                                className={`
                                    px-3 py-1 rounded-lg font-bold text-sm
                                    ${question.state === "منتظر تایید" && "bg-orange-500 text-white"}
                                    ${question.state === "رد شده" && "bg-red-500 text-white"}
                                    ${question.state === "تایید شده" && "bg-green-600 text-white"}
                                `}
                            >
                                {question.state}
                            </span>
                            <span className="text-gray-500 text-sm">{question.time}</span>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    );
}