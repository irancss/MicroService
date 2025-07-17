"use client";
import { motion } from "framer-motion";
import ProductCart from "@components/General/Product/Cart";
import { useEffect, useState } from "react";

export default function Wishlist() {
  const [productData, setProductData] = useState([]);

  const dummyProducts = [
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

  useEffect(() => {
    async function fetchWishlist() {
      try {
        const wishlistIds = [1, 2, 3];
        const products = await Promise.all(
          wishlistIds.map(async (id) => {
            const res = await fetch(`/products/${id}`);
            if (!res.ok) throw new Error("API Error");
            return res.json();
          })
        );
        setProductData(products);
      } catch (error) {
        setProductData(dummyProducts);
      }
    }
    fetchWishlist();
  }, []);

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <div className="mb-8">
        <h3 className="text-2xl font-bold text-gray-800 mb-2 border-b-2 border-indigo-500 pb-2 w-fit">
          علاقه‌مندی‌ها
        </h3>
        <p className="text-gray-600 mb-2">
          در این قسمت می‌توانید محصولات مورد علاقه خود را مشاهده کنید.
          <br />
          برای مشاهده جزئیات هر محصول، روی نام محصول کلیک کنید.
          <br />
          برای خرید محصولات، روی دکمه خرید کلیک کنید.
        </p>
        <hr className="my-4 border-gray-300" />
        <p className="text-lg font-semibold text-indigo-600 mb-4">
          محصولات شما:
        </p>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {productData.map((product, idx) => (
          <motion.div
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
          </motion.div>
        ))}
      </div>
    </div>
  );
}
