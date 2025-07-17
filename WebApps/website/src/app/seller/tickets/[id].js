
export default function ShowTicketPage({}) {
    const tickets = [
    ];
    return (
        <div className="container mx-auto p-4">
            <h1 className="text-2xl font-bold mb-4">تیکت‌ها</h1>
            <table className="min-w-full bg-white border border-gray-300">
                <thead>
                    <tr>
                        <th className="px-4 py-2 border-b">شماره تیکت</th>
                        <th className="px-4 py-2 border-b">موضوع</th>
                        <th className="px-4 py-2 border-b">وضعیت</th>
                        <th className="px-4 py-2 border-b">عملیات</th>
                    </tr>
                </thead>
                <tbody>
                    {tickets.map((ticket) => (
                        <tr key={ticket.id}>
                            <td className="px-4 py-2 border-b">{ticket.id}</td>
                            <td className="px-4 py-2 border-b">{ticket.subject}</td>
                            <td className="px-4 py-2 border-b">{ticket.status}</td>
                            <td className="px-4 py-2 border-b">
                                <button className="text-blue-600 hover:underline">
                                    مشاهده
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );

}