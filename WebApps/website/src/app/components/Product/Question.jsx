import { faQuestionCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";

library.add(faQuestionCircle);

export default function Question({ children }) {
  const questions = [
    {
      question: "برای تلوزیون مناسب هست؟",
      answer: "بهترین گزینه برا تلویزیون هست",
    },
    {
      question: "گارانتی دار هست؟؟",
      answer: "ن نداره",
    },
  ];
  return (
    <div className="grid grid-cols-4 gap-4 p-4 text-justify border-2 border-gray-300 rounded-lg">
      <div className="col-span-1">
        <h2 className="text-xl font-bold">پرسش ها</h2>
        <hr className="border border-amber-300 mt-0 md:mb-5" />
        <span>شما هم درباره این کالا پرسش ثبت کنید</span>
        <button className=" bg-amber-100 border-amber-400 text-amber-700 w-full rounded-lg py-2 mt-3">
          ثبت پرسش
        </button>
      </div>
      <div className="col-span-3">
        <div className="flex justify-between">
          <div className="flex justify-center">
            <span className="text-gray-500 font-medium text-sm">
              مرتب سازی بر اساس:
            </span>
            <ul className="flex flex-wrap gap-3 md:gap-4 pb-3 px-3">
              {["جدیدترین", "بیشترین پاسخ"].map((item) => (
                <li
                  key={item}
                  className="cursor-pointer text-sm hover:text-primary-600 transition-colors"
                >
                  {item}
                </li>
              ))}
            </ul>
          </div>
          <div>
            <span className="text-sm text-gray-400">3 پرسش</span>
          </div>
        </div>
        <div>
          {questions.map((item) => (
            <div className="grid">
              <ul key={item.question}>
                <li>
                  <FontAwesomeIcon
                    icon={faQuestionCircle}
                    className="w-4 h-4 text-gray-400 hover:text-gray-700 cursor-pointer"
                  />
                  {item.question}
                </li>
                <li className="border-b ">
                  <span className="text-gray-400 text-sm me-2">پاسخ:</span>
                  {item.answer}
                  <br />
                  <span className="text-sm text-gray-400 me-3">بهروز ممدی</span>
                  <span className="bg-green-200 border border-green-500 text-green-700 rounded-lg px-1 py-1 text-sm ">
                    خریدار
                  </span>
                </li>
              </ul>
            </div>
          ))}
        </div>
      </div>

      {children}
    </div>
  );
}
