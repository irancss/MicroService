"use client";
import { motion } from "framer-motion";
import { Swiper, SwiperSlide } from "swiper/react";
import { Autoplay } from "swiper/modules";
import Image from "next/image";
import "swiper/css";
import "swiper/css/navigation";
import "swiper/css/pagination";
import "swiper/css/autoplay";
import FiltersProduct from "@components/Category/Filters";
import ProductCategory from "@components/Category/Products";

const categories = [
  { image: "/product.png", title: "دسته اول" },
  { image: "/product.png", title: "دسته دوم" },
  { image: "/product.png", title: "دسته سوم" },
  { image: "/product.png", title: "دسته چهارم" },
  { image: "/product.png", title: "دسته پنجم" },
  { image: "/product.png", title: "دسته ششم" },
  { image: "/product.png", title: "دسته هفتم" },
  { image: "/product.png", title: "دسته هشتم" },
  { image: "/product.png", title: "دسته نهم" },
  { image: "/product.png", title: "دسته دهم" },
];

export default function CategoryPage() {
  return (
    <div className="p-2 sm:p-5 min-h-screen">
      {/* Swiper اسلایدر با */}
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4 }}
        className="relative overflow-hidden px-0 sm:px-4"
      >
        <Swiper
          dir="rtl"
          slidesPerView={2}
          loop={true}
          spaceBetween={8}
          autoplay={{
            delay: 2500,
            disableOnInteraction: false,
            pauseOnMouseEnter: true,
          }}
          modules={[Autoplay]}
          className="!overflow-visible"
          breakpoints={{
            640: {
              slidesPerView: 2,
              spaceBetween: 16,
            },
            768: {
              slidesPerView: 3,
              spaceBetween: 20,
            },
            1024: {
              slidesPerView: 4,
              spaceBetween: 24,
            },
          }}
        >
          {categories.map((category, index) => (
            <SwiperSlide
              key={index}
              className="!w-[calc(50%-8px)] sm:!w-[calc(25%-16px)] lg:!w-[calc(25%-24px)] !h-36 sm:!h-48"
            >
              <div className="w-full h-full p-2 flex flex-col gap-2 rounded-xl border-2 border-gray-200 hover:bg-gray-100 transition-all duration-300 group">
                <div className="relative w-full h-20 sm:h-32 rounded-lg overflow-hidden">
                  <Image
                    src={category.image}
                    alt={category.title}
                    fill
                    className="object-cover transition-transform duration-300 group-hover:scale-105"
                    sizes="(max-width: 640px) 80vw, (max-width: 1024px) 33vw, 25vw"
                    priority={index < 4}
                  />
                </div>
                <span className="text-xs sm:text-sm font-bold text-gray-600 text-center truncate px-2">
                  {category.title}
                </span>
              </div>
            </SwiperSlide>
          ))}
        </Swiper>
      </motion.div>

      {/* بقیه المان‌ها */}
      <div className="grid grid-cols-1 lg:grid-cols-[250px_1fr] gap-4 sm:gap-6 mt-6 sm:mt-8">
        <motion.aside
          initial={{ opacity: 0, x: -50 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3, delay: 0.2 }}
          className="bg-white p-3 sm:p-5 rounded-xl shadow-sm mb-4 lg:mb-0"
        >
         <FiltersProduct />
        </motion.aside>

        <motion.main
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.3, delay: 0.4 }}
          className="bg-white rounded-xl"
        >
          <ProductCategory />
        </motion.main>
      </div>
    </div>
  );
}
