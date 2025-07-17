"use client";
import { useState } from "react";

export default function SupportPage() {
  const [openAnswers, setOpenAnswers] = useState([]);

  const toggleAnswer = (id) => {
    setOpenAnswers((prevOpenAnswers) =>
      prevOpenAnswers.includes(id)
        ? prevOpenAnswers.filter((answerId) => answerId !== id)
        : [...prevOpenAnswers, id]
    );
  };

  const questions = [
    {
      id: 1,
      question: "چگونه می‌توانم یک حساب کاربری ایجاد کنم؟",
      answer: "برای ایجاد حساب کاربری، به صفحه ثبت‌نام بروید و فرم را پر کنید.",
    },
    {
      id: 2,
      question: "چگونه می‌توانم رمز عبور خود را بازیابی کنم؟",
      answer:
        "برای بازیابی رمز عبور، به صفحه ورود بروید و روی لینک 'فراموشی رمز عبور' کلیک کنید.",
    },
    {
      id: 3,
      question: "چگونه می‌توانم با تیم پشتیبانی تماس بگیرم؟",
      answer:
        "برای تماس با تیم پشتیبانی، از طریق ایمیل یا شماره تلفن زیر اقدام کنید.",
    },
    {
      id: 4,
      question: "سوالات متداول چیست؟",
      answer:
        "سوالات متداول شامل پاسخ به سوالات رایج کاربران است که در این بخش قابل مشاهده است.",

    },
  ];
  return (
    <div>
      <h1>پشتیبانی</h1>
      <p>
        در این بخش می‌توانید با تیم پشتیبانی تماس بگیرید یا سوالات خود را مطرح
        کنید.
      </p>
      <h2>سوالات متداول</h2>
      <ul>
        {questions.map((q) => (
          <li key={q.id}>
            <strong>{q.question}</strong>
            <button onClick={() => toggleAnswer(q.id)}>
              {openAnswers.includes(q.id) ? "بستن" : "نمایش"} پاسخ
            </button>
            {openAnswers.includes(q.id) && <p>{q.answer}</p>}
          </li>
        ))}
      </ul>
    </div>
  );
}
