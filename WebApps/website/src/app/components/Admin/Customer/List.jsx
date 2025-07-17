import Table from "@components/General/Table";

export default function CustomersList({
  customers,
  columns,
  onSearch,
  onFilter,
  onSort,
  onPageChange,
  currentPage,
  totalPages,
}) 
{
  return (
    <div className="mt-6 bg-gradient-to-br from-blue-50 via-white to-purple-50 p-8 rounded-2xl shadow-xl border border-blue-100 mx-auto">
      <Table
        data={customers}
        columns={columns}
        onSearch={onSearch}
        onFilter={onFilter}
        onSort={onSort}
        onPageChange={onPageChange}
        currentPage={currentPage}
        totalPages={totalPages}
      />
    </div>
  );
}
