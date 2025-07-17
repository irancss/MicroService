import Table from "@components/General/Table";
export default function Admin() {
  // 5 تا کاربر آخر
  // 5 تا سفارش آخر
  // 5 تا درخواست آخر درخواست همکاری
  // 5 تا تیکت آخر
  // 5 تا درخواست تسویه آخر
  // Column configurations for each table
  const userColumns = [
    { key: "ID", label: "id" },
    { key: "Username", label: "username" },
    { key: "Phone Number", label: "phoneNumber" },
  ];

  const orderColumns = [
    { key: "ID", label: "id" },
    { key: "Customer", label: "customer" },
    { key: "Amount", label: "amount" },
    { key: "Status", label: "status" },
  ];

  const partnershipColumns = [
    { key: "ID", label: "id" },
    { key: "Name", label: "name" },
    { key: "Company", label: "company" },
    { key: "Status", label: "status" },
  ];

  const ticketColumns = [
    { key: "ID", label: "id" },
    { key: "Subject", label: "subject" },
    { key: "User", label: "user" },
    { key: "Status", label: "status" },
  ];

  const withdrawalColumns = [
    { key: "ID", label: "id" },
    { key: "User", label: "user" },
    { key: "Amount", label: "amount" },
    { key: "Status", label: "status" },
  ];

  // Dummy data for demonstration - replace with actual data fetching
  const dummyLast5Users = [
    { id: 1, username: "user1", phoneNumber: "09123456789" },
    { id: 2, username: "user2", phoneNumber: "09123456780" },
    { id: 3, username: "user3", phoneNumber: "09123456781" },
    { id: 4, username: "user4", phoneNumber: "09123456782" },
  ];
  const dummyLast5Orders = [
    { id: 1, customer: "customer1", amount: 1000, status: "Completed" },
    { id: 2, customer: "customer2", amount: 2000, status: "Pending" },
    { id: 3, customer: "customer3", amount: 1500, status: "Cancelled" },
    { id: 4, customer: "customer4", amount: 3000, status: "Completed" },
    { id: 5, customer: "customer5", amount: 2500, status: "Pending" },
  ];
  const dummyLast5Partnerships = [
    { id: 1, name: "Partnership 1", company: "Company 1", status: "Active" },
    { id: 2, name: "Partnership 2", company: "Company 2", status: "Pending" },
    { id: 3, name: "Partnership 3", company: "Company 3", status: "Active" },
    { id: 4, name: "Partnership 4", company: "Company 4", status: "Inactive" },
    { id: 5, name: "Partnership 5", company: "Company 5", status: "Active" },
  ];
  const dummyLast5Tickets = [
    { id: 1, subject: "Ticket 1", user: "user1", status: "Open" },
    { id: 2, subject: "Ticket 2", user: "user2", status: "Closed" },
    { id: 3, subject: "Ticket 3", user: "user3", status: "Open" },
    { id: 4, subject: "Ticket 4", user: "user4", status: "Pending" },
    { id: 5, subject: "Ticket 5", user: "user5", status: "Closed" },
  ];
  const dummyLast5Withdrawals = [
    { id: 1, user: "user1", amount: 100, status: "Completed" },
    { id: 2, user: "user2", amount: 200, status: "Pending" },
    { id: 3, user: "user3", amount: 150, status: "Cancelled" },
    { id: 4, user: "user4", amount: 300, status: "Completed" },
    { id: 5, user: "user5", amount: 250, status: "Pending" },
  ];

  return (
    <div>
      <h1>Admin</h1>
      <p>Admin page content goes here.</p>
      <h2>Last 5 Users</h2>
      <Table columns={userColumns} data={dummyLast5Users} />
      <h2>Last 5 Orders</h2>
      <Table columns={orderColumns} data={dummyLast5Orders} />
      <h2>Last 5 Partnership Requests</h2>
      <Table columns={partnershipColumns} data={dummyLast5Partnerships} />
      <h2>Last 5 Support Tickets</h2>
      <Table columns={ticketColumns} data={dummyLast5Tickets} />
      <h2>Last 5 Withdrawal Requests</h2>
      <Table columns={withdrawalColumns} data={dummyLast5Withdrawals} />
    </div>
  );
}
