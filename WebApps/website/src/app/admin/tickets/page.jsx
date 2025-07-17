"use client";
import Table from "@components/General/Table";
import Link from "next/link";

export default function Tickets() {
  const tickets = [
    {
      id: 1,
      subject: "Issue with product",
      status: "Open",
      priority: "High",
      createdAt: "2023-09-01",
    },
    {
      id: 2,
      subject: "Billing question",
      status: "Closed",
      priority: "Medium",
      createdAt: "2023-08-15",
    },
    {
      id: 3,
      subject: "Feature request",
      status: "In Progress",
      priority: "Low",
      createdAt: "2023-08-20",
    },
  ];
  const handleCloseTicket = (id) => {
    // Logic to close the ticket
    // Here you would typically make an API call to update the ticket status
  };

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
      <h1 className="text-2xl font-bold mb-4">مدیریت تیکت‌ها</h1>
      <Table data={tickets} columns={columns} />
    </div>
  );
}
