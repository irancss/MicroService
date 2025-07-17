import AnimatedDiv from "@components/Animated/Div";

export default function DepotGoods() {
  const datas = {
    notInSite: 0,
    inSite: 0,
    inDepot: 0,
    TLP: {
      total: 0,
      inSite: 0,
      notInSite: 0,
    },
  };

  return (
    <AnimatedDiv
      className="bg-white rounded-xl p-5 shadow-md max-w-md mx-auto"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
    >
      <h2 className="font-bold text-lg mb-4 text-gray-800 text-center">
        گزارش کالاهای دپو
      </h2>
      <div className="grid grid-cols-1 gap-4">
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد تنوع های انبارش شده ناموجود در سایت:
          </span>
          <span className="font-medium text-blue-600">{datas.notInSite}</span>
        </div>
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد تنوع های انبارش شده موجود در سایت:
          </span>
          <span className="font-medium text-blue-600">{datas.inSite}</span>
        </div>
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد تنوع های انبارش شده در انبار:
          </span>
          <span className="font-medium text-blue-600">{datas.inDepot}</span>
        </div>
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد TLP کالاهای انبارش شده:
          </span>
          <span className="font-medium text-blue-600">{datas.TLP.total}</span>
        </div>
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد TLP های انبارش شده موجود در سایت:
          </span>
          <span className="font-medium text-blue-600">{datas.TLP.inSite}</span>
        </div>
        <div className="flex  justify-between align-middle bg-gray-50 rounded-lg px-4 py-3">
          <span className="text-gray-700 text-xs">
            تعداد TLP های انبارش شده ناموجود در سایت:
          </span>
          <span className="font-medium text-blue-600">
            {datas.TLP.notInSite}
          </span>
        </div>
      </div>
    </AnimatedDiv>
  );
}
