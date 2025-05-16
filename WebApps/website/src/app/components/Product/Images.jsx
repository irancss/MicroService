"use client";

import { useState } from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import { FreeMode, Navigation, Thumbs } from "swiper/modules";
import Image from "next/image";
import "swiper/css";
import "swiper/css/free-mode";
import "swiper/css/navigation";
import "swiper/css/thumbs";

export default function ProductGallery({ images }) {
  if (!images || images.length === 0) {
    images = ["/product.png", "/product.png", "/product.png","/product.png","/product.png","/product.png","/product.png","/product.png", "/product.png"];
  }
  const [thumbsSwiper, setThumbsSwiper] = useState(null);

  return (
    <div className="w-full max-w-3xl mx-auto px-4 sm:px-6 lg:px-8">
      {/* Main Swiper */}
      <div className="relative overflow-hidden rounded-lg  mb-4">
        <Swiper
          loop={images.length > 1}
          navigation
          thumbs={{ swiper: thumbsSwiper }}
          modules={[FreeMode, Navigation, Thumbs]}
          style={{
            "--swiper-navigation-color": "#fff",
            "--swiper-navigation-size": "24px",
          }}
          className="h-64 sm:h-80 md:h-96"
        >
          {images.map((img, index) => (
            <SwiperSlide key={index}>
              <div className="relative h-full w-full flex items-center justify-center">
                <Image
                  src={img}
                  alt={`Product view ${index + 1}`}
                  width={600}
                  height={600}
                  className="object-contain w-full h-full"
                  quality={90}
                  priority={index === 0}
                />
              </div>
            </SwiperSlide>
          ))}
        </Swiper>
      </div>

      {/* Thumbnail Swiper */}
      <div className="px-4 sm:px-8">
        <Swiper
          onSwiper={setThumbsSwiper}
          loop={images.length > 4}
          spaceBetween={10}
          slidesPerView={4}
          freeMode
          watchSlidesProgress
          navigation
          modules={[FreeMode, Navigation, Thumbs]}
          breakpoints={{
            480: {
              slidesPerView: 5,
              spaceBetween: 12,
            },
            640: {
              slidesPerView: 6,
              spaceBetween: 14,
            },
            1024: {
              slidesPerView: 4,
              spaceBetween: 16,
            },
          }}
        >
          {images.map((img, index) => (
            <SwiperSlide key={index}>
              <div className="relative aspect-square rounded-lg cursor-pointer border-2 border-gray-200 hover:border-amber-500 transition-all overflow-hidden">
                <Image
                  src={img}
                  alt={`Thumbnail ${index + 1}`}
                  width={100}
                  height={100}
                  className="object-cover opacity-90 hover:opacity-100"
                  quality={70}
                />
              </div>
            </SwiperSlide>
          ))}
        </Swiper>
      </div>
    </div>
  );
}
