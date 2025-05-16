"use client";

import { useState } from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import { FreeMode, Navigation, Thumbs } from "swiper/modules";
import Image from "next/image";
import "swiper/css";
import "swiper/css/free-mode";
import "swiper/css/navigation";
import "swiper/css/thumbs";

export default function SimilarProduct({ children }) {
  const arraySimilarProduct = [
    {
      id: 1,
      name: "محصول مشابه 1",
      image: "/product.png",
      price: "1000000",
    },
    {
      id: 2,
      name: "محصول مشابه 2",
      image: "/product.png",
      price: "2000000",
    },
    {
      id: 3,
      name: "محصول مشابه 3",
      image: "/product.png",
      price: "3000000",
    },
    {
      id: 4,
      name: "محصول مشابه 4",
      image: "/product.png",
      price: "4000000",
    },
    {
      id: 5,
      name: "محصول مشابه 5",
      image: "/product.png",
      price: "5000000",
    },
    {
      id: 6,
      name: "محصول مشابه 6",
      image: "/product.png",
      price: "6000000",
    },
    {
      id: 7,
      name: "محصول مشابه 7",
      image: "/product.png",
      price: "7000000",
    },
    {
      id: 8,
      name: "محصول مشابه 8",
      image: "/product.png",
      price: "8000000",
    },
    {
      id: 9,
      name: "محصول مشابه 9",
      image: "/product.png",
      price: "9000000",
    },
  ];

  const [thumbsSwiper, setThumbsSwiper] = useState(null);

  return (
    <div className="md:px-5 md:mt-5 mt-3 overflow-hidden">
      <h3 className="text-md font-bold mb-4">محصولات مشابه</h3>
      <div className="relative  ">
        <Swiper
          loop={true}
          spaceBetween={10}
          slidesPerView="2"
          autoplay="true"
          centeredSlides={false}
          modules={[Navigation]}
          className="!overflow-visible mb-4 rounded-lg"
          breakpoints={{
            640: { slidesPerView: 4 },
            768: { slidesPerView: 4 },
            1024: { slidesPerView: 7  },
          }}
          
          thumbs={{ swiper: thumbsSwiper }}
          navigation 
        >
          {arraySimilarProduct.map((product) => (
            <SwiperSlide
              key={product.id}
            >
               <div className="border-1 border-gray-300 rounded-lg p-2 bg-white  hover:shadow-lg  duration-200">
               <div className="relative aspect-square">
                  <Image
                    src={product.image}
                    alt={product.name}
                    fill
                    sizes="(max-width: 768px) 100vw, 80vw"
                    className="object-contain rounded-lg"
                    quality={60}
                  />
                </div>
                <div className="flex flex-col items-center mt-2">
                  <h5 className="text-sm font-bold">{product.name}</h5>
                  <span className="text-sm text-gray-500">
                    {product.price} تومان
                  </span>
                  <button className="bg-rose-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition duration-200 mt-2">
                    <span className="text-sm font-bold">افزودن به سبد</span>
                    </button>
                </div>
                </div>
            </SwiperSlide>
          ))}
        </Swiper>
      </div>

    </div>
  );
}
