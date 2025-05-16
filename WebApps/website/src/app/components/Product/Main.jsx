import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import Property from "./Property";
import ShortDescription from "./Short-Description";
import Link from "next/link";

import { faStar , faQuestion, faComment } from "@fortawesome/free-solid-svg-icons";

library.add(faStar , faQuestion , faComment);

export default function MainSectionProduct({ children }) {
  return (
    <>
      <div className="flex flex-col gap-4">
        <div className="flex flex-col gap-2">
          <h1 className="text-xl font-bold">عنوان محصول</h1>
          <hr />
        </div>
        <div className="flex flex-col-2 justify-between gap-2 align-middle">
          <div className="flex align-middle  gap-4">
            <span className="mt-1 text-sm">
            <FontAwesomeIcon
                icon={"fa-star"}
                className="w-3 h-3 text-amber-600 me-1"
              />
              4.5 امتیاز{" "}
              <span className="text-gray-400 text-sm">(از ۲۷۸ خریدار)</span>
            </span>
            <span className="py-1 px-3 text-sm rounded-2xl gap-2  bg-gray-200">
            <FontAwesomeIcon
                icon={"fa-comment"}
                className="w-3 h-3 text-gray-600 me-1"
              />
              5 دیدگاه
            </span>
            <span className="py-1 text-sm px-3 rounded-2xl gap-2 bg-gray-200">
              <FontAwesomeIcon
                icon={"fa-question"}
                className="w-3 h-3 text-gray-600 me-1"
              />
              5 پرسش
            </span>
          </div>
          <div>
            <span className="text-gray-400 text-sm">کد کالا: 12345</span>
          </div>
        </div>
        <div className="flex flex-row bg-gray-100 gap-2 mt-2 rounded-lg p-2 border-2 justify-evenly border-gray-300 text-gray-700">
          <span className="text-sm ">
            دسته بندی:{" "}
            <span className="text-sm font-bold">
              <Link href="/category/1">لپ تاپ</Link>
            </span>
          </span>
          <span className="text-sm ">
            برند:{" "}
            <span className="text-sm font-bold">
              <Link href="/brand/1">ایسر</Link>
            </span>
          </span>
        </div>
        <div className="flex flex-col gap-2">
          <h4 className="text-md font-bold">توضیحات کوتاه</h4>
          <ShortDescription />
        </div>
        <div>
          <h4 className="text-md font-bold">ویژگی‌های محصول</h4>

          <Property />
        </div>
      </div>
    </>
  );
}
