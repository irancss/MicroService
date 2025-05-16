import Image from "next/image";
import Link from "next/link";

export default function BriefSidebar({children}) {
  return (
    <>
      <div className="bg-gray-100 rounded-lg p-4 flex flex-col gap-4 border-2 border-gray-300">
        <div className="flex justify-start align-middle  gap-2 ">
          <Image 
          src="/product.png"
          width={50}
          height={50}
          />
          <h2 className="font-bold text-lg">
            عنوان محصول
          </h2>
        </div>
        <hr className="border border-gray-300 mt-0" />
        <div className="flex gap-2 justify-between align-middle">
          <span className="text-sm text-gray-500">فروشنده: </span>
          <span className="text-sm font-bold">فروشگاه اینترنتی دیجی کالا</span>
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
      </div>
    </>
  );
}
