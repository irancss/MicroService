import {
  faGear,
  faStar,
  faThumbsDown,
  faThumbsUp,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";

library.add(faStar, faGear, faThumbsUp, faThumbsDown);

export default function Review({ children }) {
  return (
    <div className="flex flex-col gap-4 py-3 px-2 text-justify" dir="rtl">
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {/* سایدبار امتیازات */}
        <div className="col-span-1 md:px-3 space-y-3">
          <div className="flex items-baseline gap-2">
            <span className="font-bold text-gray-800 text-2xl">4.5</span>
            <span className="text-gray-500 font-medium text-sm">از 5</span>
          </div>
          <p className="text-gray-500 font-medium text-sm">از 1000 نظر</p>
          <p className="text-gray-500 font-light text-sm">
            شما هم درباره این کالا دیدگاه ثبت کنید
          </p>

          <button className="w-full bg-amber-100 border-2 border-amber-300 text-amber-700 px-4 py-2 rounded-md hover:bg-amber-200 transition-colors">
            ثبت نظر
          </button>

          <p className="text-gray-500 text-sm font-light">
            با ثبت دیدگاه خود، به دیگران کمک کنید و 5 امتیاز دریافت کنید
          </p>
        </div>

        {/* بخش اصلی نظرات */}
        <div className="col-span-3 flex flex-col gap-4 md:mt-3  ">
          {/* فیلترهای مرتب سازی */}
          <div className="space-y-2">
            <span className="text-gray-500 font-medium text-sm md:text-base">
              مرتب سازی بر اساس:
            </span>
            <ul className="flex flex-wrap gap-3 md:gap-4 border-b border-gray-300 pb-3 mt-2">
              {[
                "جدیدترین",
                "قدیمی‌ترین",
                "بیشترین امتیاز",
                "کمترین امتیاز",
              ].map((item) => (
                <li
                  key={item}
                  className="cursor-pointer text-sm hover:text-primary-600 transition-colors"
                >
                  {item}
                </li>
              ))}
            </ul>
          </div>

          {/* کارت نظر */}
          <div className="w-full space-y-4">
            {/* هدر نظر */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <span className="font-medium">کاربر دیجی‌کالا</span>
                <span className="bg-green-200 border border-green-500 text-green-700 rounded-lg px-2 py-1 text-sm">
                  خریدار
                </span>
                <span className="text-gray-500 text-sm">17 شهریور</span>
              </div>
              <FontAwesomeIcon
                icon={faGear}
                className="w-4 h-4 text-gray-500 hover:text-gray-700 cursor-pointer"
              />
            </div>

            {/* بدنه نظر */}
            <div className="space-y-3">
              <div className="flex gap-1">
                {Array(5)
                  .fill()
                  .map((_, i) => (
                    <FontAwesomeIcon
                      key={i}
                      icon={faStar}
                      className="w-4 h-4 text-amber-400"
                    />
                  ))}
              </div>
              <p className="text-gray-600 leading-relaxed">
                Lorem ipsum dolor sit amet consectetur adipisicing elit.
                Dignissimos facilis, facere, cumque fugit aliquam quidem harum
                aperiam minus consectetur perferendis voluptas expedita neque?
                Deleniti, ipsa perferendis quis ut magni voluptatibus!
              </p>
            </div>

            {/* فوتر نظر */}
            <div className="flex items-center justify-between  pt-3">
              <div className="flex items-center gap-3">
                <span className="text-sm text-gray-500">بازرگانی تست</span>
                <div className="w-px h-4 bg-gray-300" />
                <span className="text-sm text-gray-500">زرد</span>
              </div>

              <div className="flex items-center gap-3">
                <div className="flex items-center gap-1">
                  <FontAwesomeIcon
                    icon={faThumbsDown}
                    className="w-4 h-4 text-gray-400 hover:text-red-500 cursor-pointer scale-x-[-1]"
                  />
                  <span className="text-sm mt-1 text-gray-500">0</span>
                </div>
                <div className="flex items-center gap-1">
                  <FontAwesomeIcon
                    icon={faThumbsUp}
                    className="w-4 h-4 text-gray-400 hover:text-green-500 cursor-pointer"
                  />
                  <span className="text-sm mt-1 text-gray-500">5</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
