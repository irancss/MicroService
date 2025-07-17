"use client";
import Link from "next/link";
import { footerData } from "@data/siteData/footer/footer";
import OlHorizontal from "../General/Listing/Text/OlHorizontal";
import IconGrid from "../General/Listing/FontAwesome/IconGrid";
import Image from "next/image";
import Newspaper from "../General/Newspaper";
import { motion } from "framer-motion";
const fadeIn = {
  hidden: { opacity: 0, y: 40 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.7, ease: "easeOut" } },
};
export default function PostFooter() {
  const classHeader =
    "font-bold text-gray-800 mb-3 text-lg px-2 rounded  border-s-6  border-solid border-s-amber-500 ";
  return (
    <div className="bg-gray-100 px-3 lg:px-0   pt-4">
      <motion.div
        className="container mx-auto mb-5 lg:mb-0 border-3 py-md-3 py-2 md:my-5 rounded-3xl border-gray-300 bg-gray-100 relative group"
        variants={fadeIn}
        initial="hidden"
        animate="visible"
      >
        {/* گرادیان گوشه‌ها */}
        <div
          className="absolute -right-[3px] -top-[3px] h-16 w-16 bg-gradient-to-bl from-[#FE9A00] to-gray-100 opacity-30 z-0 transition-all duration-500 
    rounded-tr-3xl rounded-bl-2xl 
    group-hover:opacity-50 group-hover:-right-[4px] group-hover:-top-[4px]"
        ></div>

        <div
          className="absolute -left-[3px] -bottom-[3px] h-16 w-16 bg-gradient-to-tr from-[#FE9A00] to-gray-100 opacity-30 z-0 transition-all duration-500 
    rounded-bl-3xl rounded-br-3xl 
    group-hover:opacity-50 group-hover:-left-[4px] group-hover:-bottom-[4px]"
        ></div>

        <div className="relative z-10">
          <div className="md:px-3 px-2 pt-6 pb-0">
            <div className="grid grid-cols-1 md:grid-cols-12 gap-5 mb-5">
              {/* محتوای شما */}
              <div className="md:col-span-4">
                {footerData.logo && (
                  <Link href="/" className="flex items-center mb-4">
                    <Image
                      src="/logo.png"
                      alt="Logo"
                      width={100}
                      height={100}
                      className="h-auto object-cover"
                      loading="lazy"
                    />
                  </Link>
                )}
                <h4 className={classHeader}>
                  {footerData.title || "عنوان بخش پایانی"}
                </h4>
                <p className="text-sm text-gray-500 text-justify">
                  {footerData.aboutUs || "متن توضیحات این بخش"}
                </p>
              </div>
              <div className="md:col-span-3">
                <h4 className={`md:mb-6 mb-7 ${classHeader}`}>لینک های مفید</h4>
                <OlHorizontal
                  items={[
                    {
                      text: "سوالات متداول",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                    {
                      text: "حریم خصوصی",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                    {
                      text: "رویه بازگرداند کالا",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                    {
                      text: "نحوه ثبت سفارش",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                    {
                      text: "رویه ارسال سفارش",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                    {
                      text: "شیوه پرداخت",
                      icon: "dot-circle",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                      href: "/contact-us",
                    },
                  ]}
                  className="border-0 shadow-red-50 mt-3"
                  bgColor="transparent"
                  numbered={false}
                />
              </div>
              <div className="md:col-span-3">
                <h4 className={classHeader}>ارتباط با ما</h4>
                <OlHorizontal
                  items={[
                    {
                      text: `${footerData.address.city}`,
                      icon: "location-dot",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                    },
                    {
                      text: `${footerData.phone}`,
                      icon: "location-dot",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                    },
                    {
                      text: `${footerData.mail}`,
                      icon: "location-dot",
                      iconColor: "text-amber-500",
                      textColor: "text-gray-500",
                    },
                  ]}
                  className="border-0 shadow-red-50"
                  bgColor="transparent"
                  numbered={false}
                />

                <div className="mx-auto mt-3">
                  <IconGrid
                    icons={[
                      { name: "instagram", color: "text-gray-600" },
                      { name: "telegram", color: "text-gray-600" },
                      { name: "whatsapp", color: "text-gray-600" },
                    ]}
                    direction="horizontal"
                    bgColor="transparent"
                    
                    size="md"
                  />
                </div>
                <Newspaper />
              </div>
              <div className="md:col-span-2">
                <h4 className={classHeader}>نشان‌های اعتماد</h4>
              </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <p className="text-sm text-gray-200 text-justify"></p>
            </div>
          </div>
        </div>
      </motion.div>
      <div className="bg-gray-400 px-3 py-2 md:mt-5 mt-3">
        <div className="grid text-center  grid-cols-1 md:grid-cols-1 gap-4 my-2">
          <p className="text-sm text-gray-300 ">
            برای استفاده از تمامی مطالب کینگ بادی ،داشتن «هدف غیرتجاری» و ذکر
            «منبع» کافیست.
          </p>
        </div>
      </div>
    </div>
  );
}
