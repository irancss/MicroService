"use client";
import Table from "@components/General/Table";
import Link from "next/link";
export default function Wallets() {
  const walletsData = [
    { vendor: "فروشنده A", balance: 1500, transactions: 5 },
    { vendor: "فروشنده B", balance: 2500, transactions: 3 },
    { vendor: "فروشنده C", balance: 3000, transactions: 8 },
    { vendor: "فروشنده D", balance: 1200, transactions: 2 },
    { vendor: "فروشنده E", balance: 1800, transactions: 4 },
    { vendor: "فروشنده F", balance: 2200, transactions: 6 },
    { vendor: "فروشنده G", balance: 2700, transactions: 7 },
    { vendor: "فروشنده H", balance: 3200, transactions: 1 },
    { vendor: "فروشنده I", balance: 4000, transactions: 9 },
    { vendor: "فروشنده J", balance: 5000, transactions: 10 },
  ];

const columns = [
    { key: "vendor", label: "فروشنده" },
    { 
        key: "balance", 
        label: "موجودی", 
        render: (row) => `${row.balance.toLocaleString('fa-IR')} تومان` 
    },
    {
        key: "transactions",
        label: "تراکنش‌ها",
        render: (row) => `${row.transactions} بار`,
    },
    {
        key: "actions",
        label: "عملیات",
        render: (row) => (
            <div className="flex gap-2">
                <Link
                    className="text-blue-500 hover:text-blue-700"
                    href={`/admin/vendors/wallets/${row.vendor}`}
                >
                    جزئیات
                </Link>
                <button
                    className="text-red-500 hover:text-red-700"
                    onClick={() => alert(`Delete wallet for ${row.vendor}`)}
                >
                    تسویه
                </button>
            </div>
        ),
    },
];

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">کیف پول فروشندگان</h1>
      <Table data={walletsData} columns={columns} />
    </div>
  );
}
