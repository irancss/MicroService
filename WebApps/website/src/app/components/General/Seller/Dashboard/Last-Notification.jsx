import AnimatedDiv from "@components/Animated/Div";

const notifications = [
    { title: "عنوان اعلان ۱", date: "۱۴۰۳/۰۴/۰۱" },
    { title: "عنوان اعلان ۲", date: "۱۴۰۳/۰۴/۰۲" },
    { title: "عنوان اعلان ۳", date: "۱۴۰۳/۰۴/۰۳" },
];

const LastNotification = () => (
    <AnimatedDiv
        className="bg-white rounded-xl p-5 shadow-md max-w-md mx-auto"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
    >
        <h2 className="font-bold text-lg mb-4 text-gray-800 text-center">
            آخرین اعلان‌ها
        </h2>
        <div className="space-y-3">
            {notifications.map((item, idx) => (
                <div
                    key={idx}
                    className="flex items-center justify-between bg-gray-50 rounded-lg px-4 py-3 hover:shadow transition"
                >
                    <div>
                        <div className="font-medium text-gray-700">{item.title}</div>
                        <div className="text-xs text-gray-400 mt-1">{item.date}</div>
                    </div>
                    <button className="text-blue-500 text-xs border border-blue-100 px-3 py-1 rounded-lg hover:bg-blue-50 transition">
                        مشاهده
                    </button>
                </div>
            ))}
        </div>
    </AnimatedDiv>
);

export default LastNotification;
