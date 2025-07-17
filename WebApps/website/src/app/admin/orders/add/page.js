import OrderItems from "@components/Admin/Order/Add/Items";
import DetailsOrder from "@components/Admin/Order/Add/Details";
import TrackingBox from "@components/Admin/Order/Add/Tracking-Box";
import Note from "@components/Admin/Order/Add/Note";
import Document from "@components/Admin/Order/Add/Documents";
export default function AddOrder() {
  return (
    <>
      <div >
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-bold text-blue-700 flex items-center gap-2">
            افزودن سفارش جدید
          </h2>
        </div>
        <hr className="border-b border-blue-200 mb-6" />
        <div className="flex gap-4">
          <DetailsOrder className="w-1/3" />
          <OrderItems className="w-2/3" />
        </div>
        <div className="flex items-center my-6">
          <hr className="flex-1 border-b border-blue-200" />
          <h4 className="text-xl font-bold text-blue-700 px-4">تنظیمات دیگر</h4>
          <hr className="flex-1 border-b border-blue-200" />
        </div>
        <div className="flex gap-4">
          <TrackingBox className="w-1/3" />
          <Document className="w-1/3" />
          <Note className="w-1/3" />
        </div>
      </div>
    </>
  );
}
