"use client";
import Table from "@components/General/Table";
import Link from "next/link";

export default function OpenTickets() {
    const tickets = [
        {
        id: 1,
        subject: "مشکل در محصول",
        status: "باز",
        priority: "بالا",
        createdAt: "2023-09-01",
        },
        {
        id: 2,
        subject: "سوال در مورد صورتحساب",
       status: "باز",
        priority: "متوسط",
        createdAt: "2023-08-15",
        },
        {
        id: 3,
        subject: "درخواست ویژگی جدید",
         status: "باز",
        priority: "پایین",
        createdAt: "2023-08-20",
        },
    ];
    
    const handleCloseTicket = (id) => {
        // Logic to close the ticket
        // Here you would typically make an API call to update the ticket status
    }
    const columns = [
        { key: "subject", label: "موضوع" },
        { key: "status", label: "وضعیت" },
        { key: "priority", label: "اولویت" },
        { key: "createdAt", label: "تاریخ ایجاد" },
        {
        key: "actions",
        label: "عملیات",
        render: (ticket) => (
            <div>
            <Link href={`/tickets/${ticket.id}`} className="text-blue-500">
                مشاهده
            </Link>
            <button
                onClick={() => handleCloseTicket(ticket.id)}
                className="text-red-500"
            >
                بستن
            </button>
            </div>
        ),
        },
    ];
    
    return (
        <div className="p-4">
        <h1 className="text-xl font-bold mb-4">تیکت‌های باز</h1>
        <Table data={tickets} columns={columns} />
        </div>
    );
    }