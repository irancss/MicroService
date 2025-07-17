export default function Table({
  data = [],
  columns = [], // حالته آبجکت‌های با {key, label, render?}
  pagination = true,
  rowsPerPage = 10,
}) {
  return (
    <div className="overflow-x-auto p-4  rounded-lg ">
      <div className="overflow-x-auto bg-white rounded-lg shadow">
        <table className="min-w-full text-center border border-gray-200">
          <thead className="bg-gray-100">
            <tr>
              {columns.map((column) => (
                <th
                  key={column.key}
                  className="px-6 py-3 text-gray-600 font-semibold text-sm uppercase tracking-wider text-center border-b border-gray-200"
                >
                  {column.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {data.length > 0 ? (
              data.map((row, rowIndex) => (
                <tr
                  key={rowIndex}
                  className="hover:bg-gray-50 transition-colors duration-150"
                >
                  {columns.map((column) => (
                    <td
                      key={column.key}
                      className="px-6 py-4 whitespace-nowrap text-sm text-gray-700 border-b border-gray-200"
                    >
                      {column.render ? column.render(row) : row[column.key]}
                    </td>
                  ))}
                </tr>
              ))
            ) : (
              <tr>
                <td
                  colSpan={columns.length}
                  className="text-center py-6 text-gray-500"
                >
                  هیچ داده‌ای برای نمایش وجود ندارد
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      {pagination && data.length > 0 && (
        <div className="flex justify-between items-center mt-6 px-2">
          <span className="text-sm text-gray-600">
            نمایش {Math.min(rowsPerPage, data.length)} از {data.length} رکورد
          </span>
          {/* Pagination controls can be added here */}
          {/* Example:
          <div className="flex space-x-1">
            <button className="px-3 py-1 text-sm text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50">
              قبلی
            </button>
            <button className="px-3 py-1 text-sm text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50">
              بعدی
            </button>
          </div>
          */}
        </div>
      )}
    </div>
  );
}
