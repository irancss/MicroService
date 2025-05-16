import Link from "next/link";

export default function SidebarProduct({ children }) {
  return (
    <>
      <div className="bg-gray-100 rounded-lg p-4 flex flex-col gap-4 border-2 border-gray-300">
        <div className="flex justify-between gap-2 align-middle">
          <span className="font-bold text-lg">فروشنده</span>
          <span className="text-gray-500 text-sm mt-2">1 فروشنده دیگر</span>
        </div>
        <hr className="border border-gray-300 mt-0" />
        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500">فروشنده: </span>
          <span className="text-sm font-bold">فروشگاه اینترنتی دیجی کالا</span>
        </div>
        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500">تاریخ انقضا: </span>
          <span className="text-sm font-bold">1402/12/30</span>
        </div>

        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500">گارانتی: </span>
          <span className="text-sm font-bold">ندارد</span>
        </div>
        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500">وضعیت موجودی: </span>
          <span className="text-sm font-bold">موجود در انبار</span>
        </div>

        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500 mt-1">قیمت: </span>
          <span className=" font-bold text-rose-600">1,200,000 تومان</span>
        </div>
        <div className="grid">
          <button className="bg-rose-600 text-white rounded-lg p-2 w-full hover:bg-rose-700 transition duration-300 ease-in-out">
            افزودن به سبد خرید
          </button>
        </div>
        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500 mt-1">
            امتیاز دریافتی با خرید این کالا:
          </span>
          <span className="text-sm font-bold">12 امتیاز</span>
        </div>
        <div className="flex justify-between gap-2 align-middle">
          <span className="text-sm text-gray-500">تاریخ بروزرسانی</span>
          <span className="text-sm font-bold">1402/12/30</span>
        </div>
      </div>
      <div className="bg-gray-100 rounded-lg p-4 flex flex-col gap-4 border-2 border-gray-300 mt-4">
        <span className="font-bold">برچسب ها</span>
        <div className="flex flex-wrap gap-2">
          <span className="bg-gray-300 text-gray-800 text-sm font-semibold px-2 py-1 rounded-full">
            <Link href="/product/1">برچسب 4</Link>
          </span>
          <span className="bg-gray-300 text-gray-800 text-sm font-semibold px-2 py-1 rounded-full">
            <Link href="/product/1">برچسب 4</Link>
          </span>
          <span className="bg-gray-300 text-gray-800 text-sm font-semibold px-2 py-1 rounded-full">
            <Link href="/product/1">برچسب 4</Link>
          </span>
          <span className="bg-gray-300 text-gray-800 text-sm font-semibold px-2 py-1 rounded-full">
            <Link href="/product/1">برچسب 4</Link>
          </span>
          <span className="bg-gray-300 text-gray-800 text-sm font-semibold px-2 py-1 rounded-full">
            <Link href="/product/1">برچسب 4</Link>
          </span>
        </div>
      </div>
    </>
  );
}
