"use client";
import TableFilter from "@components/Admin/General/TableFilter";
import useTableFilter from "@hooks/useTableFilter";
import Table from "@components/General/Table";
import Link from "next/link";

export default function PaymentSettings() {
  const dummyData = [
    {
      id: 1,
      paymentGateway: "زرین‌پال",
      currency: "IRR",
      transactionFee: 2.5,
      minPaymentAmount: 1000,
      maxPaymentAmount: 1000000,
      refundPolicy: "بازپرداخت در صورت عدم رضایت مشتری",
      paymentNotifications: true,
    },
    {
      id: 2,
      paymentGateway: "پی‌پال",
      currency: "USD",
      transactionFee: 3.0,
      minPaymentAmount: 5,
      maxPaymentAmount: 5000,
      refundPolicy: "بازپرداخت در صورت خطای تراکنش",
      paymentNotifications: false,
    },
    // Example: To test the empty state, comment out the above entries or use an empty array.
    // const paymentSettings = [];
  ];

  const columns = [
    { key: "id", label: "شناسه" },
    { key: "paymentGateway", label: "درگاه پرداخت" },
    { key: "currency", label: "واحد پول" },
    { key: "transactionFee", label: "هزینه تراکنش (%)" },
    { key: "minPaymentAmount", label: "حداقل مبلغ پرداخت" },
    { key: "maxPaymentAmount", label: "حداکثر مبلغ پرداخت" },
    { key: "refundPolicy", label: "سیاست بازپرداخت" },
    { key: "paymentNotifications", label: "اعلان‌های پرداخت" },
    {
      key: "actions",
      label: "عملیات",
      render: (item) => (
        <Link
          href={`/admin/settings/payment/${item.id}`}
          className="text-blue-600 hover:underline"
        >
          ویرایش
        </Link>
      ),
    },
  ];

  const filtersConfig = [
    {
      name: "paymentGateway",
      label: "درگاه پرداخت",
      type: "text",
      placeholder: "جستجوی درگاه پرداخت",
    },
    {
      name: "currency",
      label: "واحد پول",
      type: "text",
    },
  ];
  const initialFilters = filtersConfig.reduce((acc, filter) => {
    acc[filter.name] = ""; // Initialize all filters to empty
    return acc;
  }, {});
  const { filters, handleFilterChange, filteredData, resetFilters } =
    useTableFilter(dummyData, filtersConfig);

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">تنظیمات پرداخت</h1>
      <TableFilter
        filtersConfig={filtersConfig}
        onFilterChange={handleFilterChange}
        initialFilters={initialFilters}
        filters={filters}
      />
      <Table columns={columns} data={filteredData} />
    </div>
  );
}
