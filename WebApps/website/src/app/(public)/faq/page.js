"use client";
import { useState } from "react";

const faqSections = [
  {
    title: "سوالات ارسال",
    questions: [
      {
        question: "آیا ارسال بین‌المللی دارید؟",
        answer: "بله، به بسیاری از کشورها ارسال داریم.",
      },
      {
        question: "چگونه می‌توانم سفارش خود را پیگیری کنم؟",
        answer: "پس از ثبت سفارش، لینک پیگیری برای شما ایمیل می‌شود.",
      },
    ],
  },
  {
    title: "سوالات خریداران",
    questions: [
      {
        question: "سیاست بازگشت کالا چیست؟",
        answer: "تا ۳۰ روز پس از خرید امکان بازگشت وجود دارد.",
      },
      {
        question: "آیا محصولات گارانتی دارند؟",
        answer: "بله، تمامی محصولات دارای گارانتی معتبر هستند.",
      },
    ],
  },
  {
    title: "سوالات فروشندگان",
    questions: [
      {
        question: "چگونه می‌توانم فروشنده شوم؟",
        answer: "از طریق فرم ثبت‌نام فروشندگان اقدام کنید.",
      },
      {
        question: "تسویه حساب چگونه انجام می‌شود؟",
        answer: "تسویه حساب به صورت هفتگی انجام می‌شود.",
      },
    ],
  },
];

export default function Faq() {
  const [search, setSearch] = useState("");

  return (
    <div className="max-w-3xl mx-auto my-10 p-8 bg-white rounded-2xl shadow-lg font-sans">
      <h1 className="text-3xl font-bold mb-2 text-gray-900">سوالات متداول</h1>
      <p className="text-gray-500 mb-6">
        در این بخش پاسخ برخی از سوالات رایج را مشاهده می‌کنید.
      </p>
      <input
        type="text"
        placeholder="جستجوی سوال..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="mb-8 p-3 w-full border border-gray-200 rounded-lg text-base outline-none bg-gray-50"
      />
      {faqSections.map((section, idx) => {
        const filtered = section.questions.filter((q) =>
          q.question.toLowerCase().includes(search.toLowerCase())
        );
        if (filtered.length === 0) return null;
        return (
          <div key={idx} className="mb-10">
            <h2 className="text-xl font-semibold mb-4 text-blue-700">
              {section.title}
            </h2>
            <ul className="grid grid-cols-1 md:grid-cols-2 gap-5">
              {filtered.map((q, i) => (
                <li
                  key={i}
                  className="mb-0 p-5 rounded-xl bg-gray-100 shadow-sm"
                >
                  <div className="font-semibold text-gray-800 mb-2">
                    {q.question}
                  </div>
                  <div className="text-gray-600 text-sm">{q.answer}</div>
                </li>
              ))}
            </ul>
          </div>
        );
      })}
      {faqSections.every(
        (section) =>
          section.questions.filter((q) =>
            q.question.toLowerCase().includes(search.toLowerCase())
          ).length === 0
      ) && (
        <div className="text-gray-400 text-center py-10">سوالی یافت نشد.</div>
      )}
    </div>
  );
}
