"use client";
import Table from "@components/General/Table";
import { useEffect, useState } from "react";
export default function ApproveProducts() {
    //try Fetch Data from API if not available use dummy data
    const dummyData = [
        {
            ID: 1,
            Name: "Product A",
            Price: "$10",
            Status: "Approved",
            render: () => (
                <div className="flex justify-center items-center space-x-2">
                    <button className="text-blue-500 hover:text-blue-700">ویرایش</button>
                    <button className="text-red-500 hover:text-red-700">حذف</button>
                </div>
            ),
        },
        {
            ID: 2,
            Name: "Product B",
            Price: "$20",
            Status: "Pending",
            render: () => (
                <div className="flex justify-center items-center space-x-2">
                    <button className="text-blue-500 hover:text-blue-700">ویرایش</button>
                    <button className="text-red-500 hover:text-red-700">حذف</button>
                </div>
            ),
        },
        {
            ID: 3,
            Name: "Product C",
            Price: "$30",
            Status: "Rejected",
            render: (row) => (
                <div className="flex gap-2 text-center">
                    <a
                        href={`/edit/${row.ID}`}
                        className="text-blue-500 hover:text-blue-700"
                    >
                        ویرایش
                    </a>
                    <button className="text-red-500 hover:text-red-700">حذف</button>
                </div>
            ),
        },
    ];

    const [data, setData] = useState(dummyData);

    useEffect(() => {
        fetch("/api/products")
            .then((res) => {
                if (!res.ok) throw new Error("Network response was not ok");
                return res.json();
            })
            .then((apiData) => {
                // Map API data to match table structure if needed
                setData(
                    apiData.map((item) => ({
                        ...item,
                        render: () => (
                            <div className="flex justify-center items-center space-x-2">
                                <button className="text-blue-500 hover:text-blue-700">
                                    ویرایش
                                </button>
                                <button className="text-red-500 hover:text-red-700">
                                    حذف
                                </button>
                            </div>
                        ),
                    }))
                );
            })
            .catch(() => {
                setData(dummyData);
            });
    }, []);

    const columns = [
        { key: "ID", label: "شناسه" },
        { key: "Name", label: "نام" },
        { key: "Price", label: "قیمت" },
        { key: "Status", label: "وضعیت" },
        { key: "Actions", label: "عملیات" },
    ];

    return (
        <>
            <div >
                <div className="flex items-center justify-between mb-6">
                    <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
                        محصولات ناموجود
                    </h2>
                </div>
                <Table data={data} columns={columns} />
            </div>
        </>
    );
}
