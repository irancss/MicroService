//
export default function Map({}) {
    return (
        <div className="w-full h-full flex flex-col gap-4 p-4">
            <h1 className="text-2xl font-bold">نقشه</h1>
            <div className="w-full h-[400px] bg-gray-200 rounded-lg"></div>
            <div className="flex justify-end">
                <button className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600 transition-colors">
                    ذخیره آدرس
                </button>
            </div>
        </div>
    );
}
