"use client";
import { Swiper, SwiperSlide } from 'swiper/react';
import { FreeMode, Mousewheel } from 'swiper/modules';
import Image from 'next/image';
import { SimpleProductPage } from "@/data/siteData/product/simple";
import 'swiper/css';
import 'swiper/css/free-mode';

export default function OurFeature({ children }) {
  return (
    <div className="border-b-3 border-t border-gray-300 py-2 w-full">
      {/* Mobile Version (Swiper) */}
      <div className="lg:hidden px-4">
        <Swiper
          slidesPerView="auto"
          spaceBetween={16}
          freeMode
          mousewheel
          modules={[FreeMode, Mousewheel]}
          breakpoints={{
            320: {
              slidesPerView: 2.5,
            },
            480: {
              slidesPerView: 3.5,
            },
          }}
        >
          {SimpleProductPage.trustedIcon.map((item) => (
            <SwiperSlide key={item.id} className="!w-auto">
              <div className="flex items-center gap-2">
                <Image
                  src={item.icon}
                  alt={item.name}
                  width={40}
                  height={40}
                  className="rounded-lg"
                />
                <span className="text-sm text-gray-400 whitespace-nowrap">
                  {item.name}
                </span>
              </div>
            </SwiperSlide>
          ))}
        </Swiper>
      </div>

      {/* Desktop Version (Grid) */}
      <div className="hidden lg:flex max-w-7xl mx-auto justify-between px-8">
        {SimpleProductPage.trustedIcon.map((item) => (
          <div key={item.id} className="flex items-center gap-3">
            <Image
              src={item.icon}
              alt={item.name}
              width={50}
              height={50}
              className="rounded-lg"
            />
            <span className="text-sm text-gray-400">{item.name}</span>
          </div>
        ))}
      </div>
    </div>
  );
}