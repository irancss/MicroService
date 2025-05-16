// Wallet Page Component (Glassmorphism Style)
"use client";
import AnimatedHr from "@/app/components/Animated/Hr";
import { convertNumberRialToLetters } from "@/lib/convert-iranian-money";
import { useState } from "react";
import clsx from "clsx";

export default function WalletPage() {
    const initialWallet = {
        balance: 10000000000,
        transactions: [
            {
                id: 1,
                type: "credit",
                amount: 5000,
                date: "1403/03/01",
                description: "شارژ کیف پول",
            },
            {
                id: 2,
                type: "debit",
                amount: 2000,
                date: "1403/03/02",
                description: "خرید از فروشگاه",
            },
            {
                id: 3,
                type: "credit",
                amount: 3000,
                date: "1403/03/03",
                description: "هدیه از دوست",
            },
        ],
    };
    const [wallet] = useState(initialWallet);
    const [amount, setAmount] = useState("");

    // Pagination state
    const [currentPage, setCurrentPage] = useState(1);
    const transactionsPerPage = 10;
    const totalPages = Math.ceil(wallet.transactions.length / transactionsPerPage);

    // Get current page transactions
    const indexOfLast = currentPage * transactionsPerPage;
    const indexOfFirst = indexOfLast - transactionsPerPage;
    const currentTransactions = wallet.transactions.slice(indexOfFirst, indexOfLast);

    // Handle page change
    const handlePageChange = (page) => {
        if (page >= 1 && page <= totalPages) setCurrentPage(page);
    };

    return (
        <div
            className="min-h-screen flex flex-col items-center justify-center py-8 px-4"
            style={{
                background:
                    "linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)",
            }}
        >
            <div className="w-full max-w-4xl mx-auto space-y-10">
                {/* Wallet Card */}
                <div className="rounded-3xl p-8 mb-10 shadow-2xl border border-white/30 bg-white/30 backdrop-blur-xl bg-gradient-to-br from-white/40 via-white/20 to-blue-200/30 relative overflow-hidden">
                    {/* Glass shine effect */}
                    <div className="absolute inset-0 pointer-events-none">
                        <div className="absolute -top-10 -left-10 w-1/2 h-1/2 bg-white/40 rounded-full blur-2xl opacity-40" />
                        <div className="absolute bottom-0 right-0 w-1/3 h-1/3 bg-pink-200/40 rounded-full blur-2xl opacity-30" />
                    </div>
                    <h3 className="text-xl font-bold text-blue-700 mb-6 flex items-center gap-2 drop-shadow">
                        کیف پول شما
                    </h3>
                    <AnimatedHr className="mb-8" />
                    <div className="flex flex-col md:flex-row gap-8 mb-12">
                        {/* Balance Card */}
                        <div className="flex-1 flex flex-col justify-center items-start p-8 rounded-2xl bg-white/30 backdrop-blur-lg shadow-lg border border-white/30 relative overflow-hidden">
                            <span className="text-gray-500 text-base mb-2">موجودی کیف پول:</span>
                            <span className="text-green-600 font-extrabold tracking-wider text-2xl mb-1 drop-shadow">
                                {wallet.balance.toLocaleString()}{" "}
                                <span className="text-base">ریال</span>
                            </span>
                            <span className="text-gray-500 text-sm mt-2">
                                معادل {convertNumberRialToLetters(wallet.balance)} تومان
                            </span>
                            {/* Shine */}
                            <div className="absolute top-0 right-0 w-1/3 h-1/3 bg-blue-200/40 rounded-full blur-xl opacity-30" />
                        </div>
                        {/* Increase Balance Form */}
                        <form
                            className="flex-1 flex flex-col gap-4 bg-white/30 backdrop-blur-lg border border-white/30 rounded-2xl px-8 py-6 shadow-lg relative overflow-hidden"
                            onSubmit={(e) => {
                                e.preventDefault();
                                const amount = Number(e.target.elements.amount.value);
                                if (!amount || amount <= 0) return;
                                alert(`درخواست افزایش ${amount.toLocaleString()} تومان ثبت شد.`);
                                setAmount("");
                                e.target.reset();
                            }}
                        >
                            <label
                                className="text-gray-700 text-base mb-1 font-semibold"
                                htmlFor="amount"
                            >
                                افزایش موجودی
                            </label>
                            <input
                                type="number"
                                name="amount"
                                id="amount"
                                min="1"
                                placeholder="مبلغ افزایش (تومان)"
                                className="border border-blue-200/40 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-200/60 text-lg transition bg-white/40 backdrop-blur"
                                required
                                onChange={(e) => setAmount(e.target.value)}
                                value={amount}
                            />
                            {amount && (
                                <span className="text-blue-600 text-sm">
                                    معادل {convertNumberRialToLetters(Number(amount) * 10)} تومان
                                </span>
                            )}
                            <button
                                type="submit"
                                className="bg-gradient-to-r from-pink-500/80 via-blue-500/80 to-cyan-400/80 text-white px-6 py-2 rounded-lg font-bold hover:from-pink-600 hover:to-cyan-500 transition mt-2 shadow-lg"
                            >
                                افزایش موجودی
                            </button>
                            {/* Shine */}
                            <div className="absolute -bottom-6 -left-6 w-1/4 h-1/4 bg-cyan-200/40 rounded-full blur-xl opacity-30" />
                        </form>
                    </div>
                    {/* Pagination (top) */}
                    {totalPages > 1 && (
                        <div className="flex justify-center items-center gap-2 mt-8">
                            <button
                                className="px-3 py-1 rounded-lg bg-white/40 text-blue-600 hover:bg-blue-200/60 transition disabled:opacity-50 backdrop-blur"
                                onClick={() => handlePageChange(currentPage - 1)}
                                disabled={currentPage === 1}
                            >
                                قبلی
                            </button>
                            {[...Array(totalPages)].map((_, idx) => (
                                <button
                                    key={idx}
                                    className={`px-3 py-1 rounded-lg ${
                                        currentPage === idx + 1
                                            ? "bg-gradient-to-r from-pink-400/80 to-blue-400/80 text-white shadow"
                                            : "bg-white/40 text-blue-600 hover:bg-blue-200/60"
                                    } transition backdrop-blur`}
                                    onClick={() => handlePageChange(idx + 1)}
                                >
                                    {idx + 1}
                                </button>
                            ))}
                            <button
                                className="px-3 py-1 rounded-lg bg-white/40 text-blue-600 hover:bg-blue-200/60 transition disabled:opacity-50 backdrop-blur"
                                onClick={() => handlePageChange(currentPage + 1)}
                                disabled={currentPage === totalPages}
                            >
                                بعدی
                            </button>
                        </div>
                    )}
                </div>
                {/* Transactions Table */}
                <div className="rounded-3xl p-8 shadow-2xl border border-white/30 bg-white/30 backdrop-blur-xl bg-gradient-to-br from-white/40 via-white/20 to-blue-200/30 relative overflow-hidden">
                    {/* Glass shine effect */}
                    <div className="absolute inset-0 pointer-events-none">
                        <div className="absolute -top-10 -left-10 w-1/2 h-1/2 bg-pink-200/40 rounded-full blur-2xl opacity-30" />
                        <div className="absolute bottom-0 right-0 w-1/3 h-1/3 bg-blue-200/40 rounded-full blur-2xl opacity-30" />
                    </div>
                    <h3 className="text-xl font-bold text-blue-700 mb-6 flex items-center gap-2 drop-shadow">
                        <svg width="22" height="22" fill="none" viewBox="0 0 24 24">
                            <path
                                d="M12 3v18m9-9H3"
                                stroke="#2563eb"
                                strokeWidth="2"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                            />
                        </svg>
                        تراکنش‌ها
                    </h3>
                    <AnimatedHr className="mb-6" />
                    <div className="overflow-x-auto rounded-xl">
                        <table className="min-w-full divide-y divide-white/40 text-center bg-white/20 backdrop-blur rounded-xl">
                            <thead className="bg-white/40">
                                <tr>
                                    <th className="px-6 py-3 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        تاریخ
                                    </th>
                                    <th className="px-6 py-3 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        مبلغ
                                    </th>
                                    <th className="px-6 py-3 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        وضعیت
                                    </th>
                                    <th className="px-6 py-3 text-xs font-semibold text-gray-600 uppercase tracking-wider">
                                        توضیحات
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white/10 divide-y divide-white/30">
                                {currentTransactions.length === 0 ? (
                                    <tr>
                                        <td colSpan={4} className="py-8 text-gray-400 text-lg">
                                            تراکنشی یافت نشد.
                                        </td>
                                    </tr>
                                ) : (
                                    currentTransactions.map((transaction) => (
                                        <tr key={transaction.id}>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                                                {transaction.date}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm font-bold text-gray-800">
                                                {transaction.amount.toLocaleString()}{" "}
                                                <span className="text-xs text-gray-500">تومان</span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm">
                                                <span
                                                    className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${
                                                        transaction.type === "credit"
                                                            ? "bg-green-200/60 text-green-700"
                                                            : "bg-pink-200/60 text-pink-700"
                                                    }`}
                                                >
                                                    {transaction.type === "credit" ? "شارژ" : "خرید"}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                                {transaction.description}
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                    {/* Pagination (bottom) */}
                    {totalPages > 1 && (
                        <div className="flex justify-center items-center gap-2 mt-8">
                            <button
                                className="px-3 py-1 rounded-lg bg-white/40 text-blue-600 hover:bg-blue-200/60 transition disabled:opacity-50 backdrop-blur"
                                onClick={() => handlePageChange(currentPage - 1)}
                                disabled={currentPage === 1}
                            >
                                قبلی
                            </button>
                            {[...Array(totalPages)].map((_, idx) => (
                                <button
                                    key={idx}
                                    className={`px-3 py-1 rounded-lg ${
                                        currentPage === idx + 1
                                            ? "bg-gradient-to-r from-pink-400/80 to-blue-400/80 text-white shadow"
                                            : "bg-white/40 text-blue-600 hover:bg-blue-200/60"
                                    } transition backdrop-blur`}
                                    onClick={() => handlePageChange(idx + 1)}
                                >
                                    {idx + 1}
                                </button>
                            ))}
                            <button
                                className="px-3 py-1 rounded-lg bg-white/40 text-blue-600 hover:bg-blue-200/60 transition disabled:opacity-50 backdrop-blur"
                                onClick={() => handlePageChange(currentPage + 1)}
                                disabled={currentPage === totalPages}
                            >
                                بعدی
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
