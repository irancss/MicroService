export default function Sellers({ children }) {
  return (
    <>
      <div className="w-full mt-3 md:px-5">
        <h3 className="text-md font-bold mb-4">فروشندگان این کالا</h3>
        <div className="bg-gray-100 flex mt-3 grid-cols-1 rounded-lg p-4 w-full justify-between items-center gap-4 border-2 border-gray-300">
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">فروشنده: </span>
            <span className="text-sm font-bold">
              فروشگاه اینترنتی دیجی کالا
            </span>
          </div>
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">امتیاز فروشنده</span>
            <span className="text-sm font-bold">4.5</span>
          </div>
          <div className="flex gap-2">
            <span className="text-sm text-gray-500">قیمت: </span>
            <span className=" font-bold h-3">1,200,000 تومان</span>
          </div>
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">قیمت: </span>
            <span className=" font-bold h-3">1,200,000 تومان</span>
          </div>
          <div className="flex gap-2 ">
            <button className="bg-rose-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition duration-200">
              <span className="text-sm font-bold">افزودن به سبد</span>
            </button>
          </div>
        </div>

        <div className="bg-gray-100 flex grid-cols-1 mt-3 rounded-lg p-4 w-full justify-between items-center gap-4 border-2 border-gray-300">
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">فروشنده: </span>
            <span className="text-sm font-bold">
              فروشگاه اینترنتی دیجی کالا
            </span>
          </div>
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">امتیاز فروشنده</span>
            <span className="text-sm font-bold">4.5</span>
          </div>
          <div className="flex gap-2">
            <span className="text-sm text-gray-500">قیمت: </span>
            <span className=" font-bold h-3">1,200,000 تومان</span>
          </div>
          <div className="flex gap-2 ">
            <span className="text-sm text-gray-500">قیمت: </span>
            <span className=" font-bold h-3">1,200,000 تومان</span>
          </div>
          <div className="flex gap-2 ">
            <button className="bg-rose-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition duration-200">
              <span className="text-sm font-bold">افزودن به سبد</span>
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
