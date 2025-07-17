"use client";
import Table from "@components/General/Table";
import Link from "next/link";
import Swal from "sweetalert2";

export default function ShowListTicketPage({}) {
  const dummyTickets = [
    {
      id: 1,
      subject: "مشکل در ورود به حساب کاربری",
      createdAt: "1400/01/01",
      category: "حساب کاربری",
      priority: "عالی",
      status: "باز",
    },
    {
      id: 2,
      subject: "سوال درباره محصول",
      createdAt: "1400/01/02",
      category: "محصولات",
      priority: "متوسط",
      status: "بسته شده",
    },
    {
      id: 3,
      subject: "درخواست بازگشت وجه",
      createdAt: "1400/01/03",
      category: "مالی",
      priority: "پایین",
      status: "در حال بررسی",
    },
  ];
  const columns = [
    { label: "شماره تیکت", key: "id" },
    { label: "موضوع", key: "subject" },
    { label: "دسته بندی", key: "category" },
    { label: "اولویت", key: "priority" },
    { label: "وضعیت", key: "status" },
    { label: "تاریخ ایجاد", key: "createdAt" },
    {
      label: "عملیات",
      key: "actions",
      render: (ticket) => (
        <div className="flex items-center text-center justify-center gap-3">
          <Link
            href={`/seller/tickets/${ticket.id}`}
            className="py-1 px-2 rounded bg-blue-200 text-blue-700 hover:bg-blue-200"
          >
            مشاهده
          </Link>
          <button
            className="ml-2 py-1 px-2 rounded bg-red-200 text-red-700 hover:bg-red-200"
            onClick={() =>
              Swal.fire({
                title: "آیا مطمئن هستید؟",
                text: `حذف تیکت ${ticket.id}`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "بله",
                cancelButtonText: "خیر",
              }).then((result) => {
                if (result.isConfirmed) {
                  handleCloseTicket(ticket.id);
                }
              })
            }
          >
            بستن تیکت
          </button>
        </div>
      ),
    },
  ];
  const data = dummyTickets.map((ticket) => ({
    id: ticket.id,
    subject: ticket.subject,
    createdAt: ticket.createdAt,
    category: ticket.category,
    priority: ticket.priority,
    status: ticket.status,
  }));
  const handleCloseTicket = (ticketId) => {
    // Logic to close the ticket
    console.log(`Closing ticket with ID: ${ticketId}`);
    // You can add API call here to close the ticket
  };
  return (
    <div className="container mx-auto p-4">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold ">تیکت‌ ها</h1>
        <Link
          href="/seller/tickets/create"
          className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
        >
          ایجاد تیکت جدید
        </Link>
      </div>
      <hr className="border-gray-300 mb-4" />
      <Table columns={columns} data={data} />
    </div>
  );
}
