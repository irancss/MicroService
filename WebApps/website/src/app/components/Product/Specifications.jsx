export default function Specifications({ children }) {
  const arrayProperty = [
    { id: 1, name: "رنگ", value: "قرمز" },
    { id: 2, name: "وزن", value: "1.5 کیلوگرم" },
    { id: 3, name: "ابعاد", value: "20x30x40 سانتی‌متر" },
    { id: 4, name: "جنس", value: "پلاستیک" },
    { id: 5, name: "گارانتی", value: "یک سال" },
    { id: 6, name: "کشور سازنده", value: "ایران" },
    { id: 7, name: "مدل", value: "مدل A" },
    { id: 8, name: "نوع", value: "الکترونیکی" },
    { id: 9, name: "تاریخ تولید", value: "1402/01/01" },
    { id: 10, name: "تاریخ انقضا", value: "1403/01/01" },
    { id: 11, name: "شرکت سازنده", value: "شرکت الف" },
    { id: 12, name: "نوع بسته بندی", value: "جعبه‌ای" },
    { id: 13, name: "تعداد در بسته", value: "1 عدد" },
    { id: 14, name: "نوع مصرف", value: "خانگی" },
    { id: 15, name: "قابلیت شستشو", value: "دارد" },
    { id: 16, name: "سایر ویژگی‌ها", value: "ضد آب" },
    { id: 17, name: "توضیحات بیشتر", value: "این محصول دارای کیفیت بالا و قیمت مناسب است." },
    { id: 18, name: "نکات مهم", value: "لطفا قبل از استفاده، دفترچه راهنما را مطالعه کنید." },
  ];
  const groupedProperties = [];
  for (let i = 0; i < arrayProperty.length; i += 2) {
    groupedProperties.push(arrayProperty.slice(i, i + 2));
  }

  return (
    <div className="flex flex-col gap-4 p-4 text-justify border-2 border-gray-300 rounded-lg">
    <h2 className="text-xl font-bold">مشخصات محصول</h2>
    <hr className="border border-gray-300 mt-0" />
    
    {/* استفاده از Grid با تغییر حالت در صفحه‌های کوچک */}
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      {arrayProperty.map((element) => (
        <div 
          key={element.id}
          className="flex flex-col md:flex-row md:items-center justify-between gap-2 p-3 border-b border-gray-200"
        >
          <span className="text-gray-500 font-medium text-sm md:text-base">
            {element.name}
          </span>
          <span className="font-bold text-gray-800 text-sm md:text-base">
            {element.value}
          </span>
        </div>
      ))}
    </div>
  </div>
  );
}
