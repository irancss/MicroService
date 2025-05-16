import Image from "next/image";
import Application from "../General/application";

export default function Prefooter() {
  const items = [
    {
      icon: "/globe.svg",
      title: "ارتباط جهانی",
      desc: "دسترسی به خدمات ما در سراسر جهان",
    },
    {
      icon: "/globe.svg",
      title: "پشتیبانی ۲۴/۷",
      desc: "همیشه در کنار شما هستیم",
    },
    {
      icon: "/globe.svg",
      title: "امنیت بالا",
      desc: "اطلاعات شما نزد ما محفوظ است",
    },
    {
      icon: "/globe.svg",
      title: "تجربه کاربری عالی",
      desc: "ساده، سریع و کاربرپسند",
    },
  ];

  return (
    <>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6 mb-10 px-6 md:px-20">
        {items.map((item, i) => (
          <div
            key={i}
            className="flex flex-col items-center bg-white rounded-xl shadow-lg p-6 hover:scale-105 transition-transform duration-300"
          >
            <div className="bg-blue-100 rounded-full p-4 mb-4">
              <Image
                src={item.icon}
                alt={item.title}
                width={50}
                height={50}
                className="object-contain"
              />
            </div>
            <h3 className="text-lg font-bold text-blue-700 mb-2">{item.title}</h3>
            <p className="text-gray-600 text-center">{item.desc}</p>
          </div>
        ))}
      </div>
      <Application />
    </>
  );
}
