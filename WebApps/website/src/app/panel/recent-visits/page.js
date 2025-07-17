"use client";
import ProductCart from "@components/General/Product/Cart";
import { useState } from "react";
import AnimatedHr from "@components/Animated/Hr";
import AnimatedDiv from "@components/Animated/Div";
export default function RecentVisitsPage() {
  const initialVisits = [
    {
      id: 1,
      name: "محصول 1",
      image: "/product.png",
      datePublished: "2023-01-01",
      items: [
        {
          id: 1,
          price: 800000,
          specialOffer: 500000,
          property: "ویژگی 1",
          selected: false,
          recommended: true,
          description:
            "این ویژگی به شما این امکان را می‌دهد که با استفاده از آن، تجربه کاربری بهتری داشته باشید.",
          stock: 10,
        },
        {
          id: 2,
          price: 2,
          specialOffer: 800000,
          property: "ویژگی 2",
          selected: false,
          description:
            "این ویژگی به شما این امکان را می‌دهد که با استفاده از آن، تجربه کاربری بهتری داشته باشید.",
          stock: 5,
        },
        {
          id: 3,
          price: 3,
          specialOffer: 800000,
          property: "ویژگی 3",
          selected: false,
          description:
            "این ویژگی به شما ا3 که با استفاده از آن، تجربه کاربری بهتری داشته باشید.",
          stock: 0,
        },
      ],
    },
    {
      id: 2,
      name: "محصول 2",
      image: "/product.png",
      price: 2000000,
      datePublished: "2023-01-02",
      description:
        "این یک توضیحات تستی برای محصول 2 است. این محصول دارای ویژگی‌های خاصی است که آن را از سایر محصولات متمایز می‌کند.",
      stock: 20,
      recommended: true,
    },
    {
      id: 3,
      name: "محصول 3",
      image: "/product.png",
      price: 3000000,
      recommended: false,
      specialOffer: 2500000,
      datePublished: "2023-01-03",
      stock: 0,
    },
  ];

  const [visits] = useState(initialVisits);

  return (
    <div>
      <h2 className="font-bold text-2xl mb-3">بازدیدهای اخیر</h2>
      <AnimatedHr className="mb-3" />
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {initialVisits.map((product, idx) => (
          <AnimatedDiv
            key={product.id}
            initial={{ opacity: 0, y: 30 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: idx * 0.1, duration: 0.4 }}
            className="  flex flex-col justify-between   transition-shadow duration-200"
          >
            <ProductCart
              key={product.id}
              name={product.name}
              image={product.image}
              price={product.price}
              specialOffer={product.specialOffer}
              recommended={product.recommended}
              items={product.items}
              datePublished={product.datePublished}
              stock={product.stock}
              description={product.description}
            />
          </AnimatedDiv>
        ))}
      </div>
    </div>
  );
}
