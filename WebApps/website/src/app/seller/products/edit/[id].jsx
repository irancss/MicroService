
export default function EditProductPage({}) {
    return (
        <div className="p-4">
        <h1 className="text-2xl font-bold mb-4">ویرایش محصول</h1>
        <form className="space-y-4">
            <div>
            <label className="block mb-2">نام محصول</label>
            <input
                type="text"
                className="w-full p-2 border rounded"
                placeholder="نام محصول را وارد کنید"
            />
            </div>
            <div>
            <label className="block mb-2">قیمت</label>
            <input
                type="number"
                className="w-full p-2 border rounded"
                placeholder="قیمت را وارد کنید"
            />
            </div>
            <div>
            <label className="block mb-2">توضیحات</label>
            <textarea
                className="w-full p-2 border rounded"
                placeholder="توضیحات محصول را وارد کنید"
            ></textarea>
            </div>
            <button
            type="submit"
            className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
            >
            ذخیره تغییرات
            </button>
        </form>
        </div>
    );
}
