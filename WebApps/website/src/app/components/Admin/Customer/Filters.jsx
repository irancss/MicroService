import Input from "@components/General/Input";
export default function Filters({ columns }) {
  const handleSearch = (event) => {
    const searchTerm = event.target.value;
  };
  return (
    <>
      <div className="mb-6 bg-white p-4 rounded-lg shadow-sm border border-blue-100">
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1">
            <label
              htmlFor="search"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              جستجو
            </label>
            <Input
              type="text"
              id="search"
              placeholder="جستجو بر اساس نام یا ایمیل..."
              className="w-full"
            />
          </div>
          <div className="w-full md:w-48 mt-2">
            <label
              htmlFor="sortBy"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              مرتب‌سازی بر اساس
            </label>
            <select
              id="sortBy"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {columns.map((column) => (
                <option key={column.key} value={column.key}>
                  {column.label}
                </option>
              ))}
            </select>
          </div>
          <div className="flex items-end">
            <button className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition">
              اعمال فیلتر
            </button>
          </div>
        </div>
        <div className="mt-4">
          <details className="group">
            <summary className="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center cursor-pointer list-none">
              <span>فیلتر های پیشرفته</span>
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-4 w-4 mr-1 group-open:rotate-180 transition-transform"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M19 9l-7 7-7-7"
                />
              </svg>
            </summary>

            <div className="mt-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <div>
                  <label
                    htmlFor="avgOrderAmount"
                    className="block text-sm font-medium text-gray-700 mb-1"
                  >
                    میانگین مبلغ سفارش
                  </label>
                  <div className="flex gap-2">
                    <Input
                      type="number"
                      id="minAvgOrder"
                      placeholder="حداقل"
                      className="w-full"
                    />
                    <Input
                      type="number"
                      id="maxAvgOrder"
                      placeholder="حداکثر"
                      className="w-full"
                    />
                  </div>
                </div>

                <div>
                  <label
                    htmlFor="orderCount"
                    className="block text-sm font-medium text-gray-700 mb-1"
                  >
                    تعداد سفارش
                  </label>
                  <div className="flex gap-2">
                    <Input
                      type="number"
                      id="minOrderCount"
                      placeholder="حداقل"
                      className="w-full"
                    />
                    <Input
                      type="number"
                      id="maxOrderCount"
                      placeholder="حداکثر"
                      className="w-full"
                    />
                  </div>
                </div>

                <div>
                  <label
                    htmlFor="orderAmount"
                    className="block text-sm font-medium text-gray-700 mb-1"
                  >
                    مبلغ سفارش
                  </label>
                  <div className="flex gap-2">
                    <Input
                      type="number"
                      id="minOrderAmount"
                      placeholder="حداقل"
                      className="w-full"
                    />
                    <Input
                      type="number"
                      id="maxOrderAmount"
                      placeholder="حداکثر"
                      className="w-full"
                    />
                  </div>
                </div>

                <div>
                  <label
                    htmlFor="paymentMethod"
                    className="block text-sm font-medium text-gray-700 mb-1"
                  >
                    روش های پرداخت
                  </label>
                  <select
                    id="paymentMethod"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    multiple
                  >
                    <option value="credit_card">کارت اعتباری</option>
                    <option value="cash">پرداخت نقدی</option>
                    <option value="online">پرداخت آنلاین</option>
                    <option value="bank_transfer">انتقال بانکی</option>
                  </select>
                </div>

                <div>
                  <label
                    htmlFor="shippingMethod"
                    className="block text-sm font-medium text-gray-700 mb-1"
                  >
                    روش های ارسال
                  </label>
                  <select
                    id="shippingMethod"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    multiple
                  >
                    <option value="express">ارسال سریع</option>
                    <option value="standard">ارسال استاندارد</option>
                    <option value="economic">ارسال اقتصادی</option>
                    <option value="in_person">تحویل حضوری</option>
                  </select>
                </div>
              </div>
            </div>
          </details>
        </div>
      </div>
    </>
  );
}
