export default function EmailInput() {
    return (
      <div className="max-w-md mx-auto mt-3 rounded-lg">
        <div className="mb-4 text-right">
          <label
            htmlFor="email"
            className="block text-gray-500 text-sm font-bold mb-2"
          >
            عضویت در خبرنامه
          </label>
          <div className="flex gap-2"> {/* اضافه کردن flex برای چیدمان افقی */}
           
          <button
              type="submit"
              className="whitespace-nowrap font-assom bg-amber-500 hover:bg-amber-600 text-white font-bold py-2 px-4 rounded-lg transition duration-200"
            >
              ارسال
            </button>
            <div className="relative flex-grow">
              <div className="absolute right-3 top-2 text-gray-400">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-6 w-6"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                  />
                </svg>
              </div>
              <input
                type="email"
                id="email"
                className="w-full px-4 py-2 border-2 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent text-gray-700"
                placeholder="example@domain.com"
                dir="ltr"
              />
            </div>
        
          </div>
        </div>
      </div>
    );
  }