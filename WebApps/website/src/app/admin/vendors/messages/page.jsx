"use client";
import Table from "@components/General/Table";
import { time } from "framer-motion";
import Swal from "sweetalert2";

export default function VendorMessages() {
  // Dummy data for vendor messages
  const vendorMessages = [
    {
      id: 1,
      name: "Vendor A",
      mobile: "09123456789",
      message: [
        {
          question: "How can I update my profile?",
          time: "2023-10-01 10:00",
          answer: "You can update your profile in the settings section.",
          answerTime: "2023-10-01 10:05",
        },
      ],
      status: "خوانده نشده",
      render: function () {
        return (
          <div className="flex justify-between items-center">
            <span>{this.message[0].question}</span>
            <span className="text-gray-500">{this.status}</span>
          </div>
        );
      },
    },
    {
      id: 2,
      name: "Vendor B",
      mobile: "09187654321",
      message: [
        {
          question: "Can you help me with my account?",
          time: "2023-10-01 10:00",
          answer: "Sure, I can help you with that.",
          answerTime: "2023-10-01 10:05",
        },
      ],
      status: "read",
      render: function () {
        return (
          <div className="flex justify-between items-center">
            <span>{this.message[0].question}</span>
            <span className="text-gray-500">{this.status}</span>
          </div>
        );
      },
    },
  ];

const columns = [
    { key: "name", label: "نام فروشنده" },
    { key: "mobile", label: "شماره موبایل" },
    {
        key: "message",
        label: "پیام",
        render: (row) => row.render(),
    },
    {
        key: "status",
        label: "وضعیت",
        render: (row) => (
            <span
                className={`px-2 py-1 rounded-full text-xs ${
                    row.status === "خوانده نشده"
                        ? "bg-yellow-500 text-white"
                        : "bg-green-500 text-white"
                }`}
            >
                {row.status}
            </span>
        ),
    },
    {
        key: "actions",
        label: "عملیات",
        render: (row) => (
            <div className="flex">
                {/* Close ticket button with sweet alert confirmation */}
                <button
                    className="text-red-500 hover:text-red-700"
                    onClick={() => {
                        Swal.fire({
                            title: "آیا مطمئن هستید؟",
                            text: "این پیام بسته خواهد شد.",
                            icon: "warning",
                            showCancelButton: true,
                            confirmButtonText: "بله، بسته شود",
                            cancelButtonText: "لغو",
                        }).then((result) => {
                            if (result.isConfirmed) {
                                Swal.fire("بسته شد!", "پیام با موفقیت بسته شد.", "success");
                                // Here you would update the state to remove or mark the message as closed
                            }
                        });
                    }}
                >
                        بستن پیام
                </button>
                {/* Combined view/answer message details button */}
                <button
                    className="ml-2 text-blue-500 hover:text-blue-700"
                    onClick={() => {
                        // Mark message as read if it's unread
                        if (row.status === "خوانده نشده") {
                            row.status = "read";
                            // In a real app, you would update state here
                            // setVendorMessages([...vendorMessages])
                        }
                        
                        // Check if there's already an answer
                        if (row.message[0].answer) {
                            // Just show details if there's an answer
                            const messagesHtml = row.message.map((msg, index) => `
                                <div class="message-item ${index > 0 ? 'mt-4 pt-4 border-t' : ''}">
                                    <strong>سوال:</strong> ${msg.question}<br>
                                    <strong>زمان:</strong> ${msg.time}<br>
                                    <strong>پاسخ:</strong> ${msg.answer}<br>
                                    <strong>زمان پاسخ:</strong> ${msg.answerTime}
                                </div>
                                <textarea class="swal2-textarea" placeholder="پاسخ خود را اینجا بنویسید..."></textarea>
                            `).join('');

                            Swal.fire({
                                title: "جزئیات پیام",
                                html: `<div class="message-container">${messagesHtml}</div>`,
                                confirmButtonText: "بستن",
                                width: '600px'
                            });
                        } else {
                            // Show form to provide an answer
                            Swal.fire({
                                title: "پاسخ به پیام",
                                html: `
                                    <div>
                                        <strong>سوال:</strong> ${row.message[0].question}<br>
                                        <strong>زمان:</strong> ${row.message[0].time}
                                    </div>
                                    <textarea id="answer" class="swal2-textarea" placeholder="پاسخ خود را اینجا بنویسید..."></textarea>
                                `,
                                showCancelButton: true,
                                confirmButtonText: "ارسال پاسخ",
                                cancelButtonText: "لغو",
                                preConfirm: () => {
                                    const answer = document.getElementById("answer").value;
                                    if (!answer) {
                                        Swal.showValidationMessage("لطفاً پاسخ خود را وارد کنید.");
                                    }
                                    return answer;
                                },
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    // Save the answer
                                    row.message[0].answer = result.value;
                                    row.message[0].answerTime = new Date().toISOString().slice(0, 16).replace('T', ' ');
                                    // In a real app, update state here
                                    // setVendorMessages([...vendorMessages])
                                    Swal.fire("پاسخ ارسال شد!", "پاسخ شما با موفقیت ارسال شد.", "success");
                                }
                            });
                        }
                    }}
                >
                    مشاهده جزئیات
                </button>
            </div>
        ),
    },
];

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">پیام‌های فروشندگان</h1>
      <Table data={vendorMessages} columns={columns} />
    </div>
  );
}
