import { useState } from "react";
import Swal from "sweetalert2";
import Image from "next/image";

export default function ProductCart({
  name,
  image,
  price,
  specialOffer,
  recommended,
  datePublished,
  items,
  stock,
  description,
}) {
  const [loading, setLoading] = useState(false);
  const [inCart, setInCart] = useState(false);
  const [quantity, setQuantity] = useState(1);
  const [selectedItem, setSelectedItem] = useState(
    items && Array.isArray(items) && items.length > 0
      ? items.find((i) => i.selected) || null
      : null
  );
  const [isFavorite, setIsFavorite] = useState(false);

  const handleAddToCart = async () => {
    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 1500));
    setLoading(false);
    setInCart(true);

    Swal.fire({
      icon: "success",
      title: "موفق!",
      text: "محصول با موفقیت به سبد خرید افزوده شد.",
      confirmButtonText: "باشه",
    });
  };

  const handleIncrease = () => setQuantity((q) => q + 1);

  const handleDecrease = () => {
    if (quantity > 1) {
      setQuantity((q) => q - 1);
    } else {
      setInCart(false);
      setQuantity(1);
      Swal.fire({
        icon: "info",
        title: "حذف شد",
        text: "محصول از سبد خرید حذف شد.",
        confirmButtonText: "باشه",
      });
    }
  };

  const formatPrice = (num) => {
    if (!num && num !== 0) return "";
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  };

  const showPrice = () => {
    if (items && Array.isArray(items) && items.length > 0) {
      if (!selectedItem) {
        const minItem = items.reduce((min, curr) => {
          const currPrice =
            curr.specialOffer && curr.specialOffer < curr.price
              ? curr.specialOffer
              : curr.price;
          const minPrice =
            min.specialOffer && min.specialOffer < min.price
              ? min.specialOffer
              : min.price;
          return currPrice < minPrice ? curr : min;
        }, items[0]);
        const hasOffer =
          minItem.specialOffer && minItem.specialOffer < minItem.price;
        return hasOffer ? (
          <div className="flex flex-col items-start">
            <span className="text-red-600 font-bold text-sm">
              {formatPrice(minItem.specialOffer)} تومان
            </span>
            <span className="text-gray-400 line-through text-sm">
              {formatPrice(minItem.price)} تومان
            </span>
          </div>
        ) : (
          <span className="text-gray-800 font-bold text-sm">
            {formatPrice(minItem.price)} تومان
          </span>
        );
      } else {
        const hasOffer =
          selectedItem.specialOffer &&
          selectedItem.specialOffer < selectedItem.price;
        return hasOffer ? (
          <div className="flex flex-col items-end">
            <span className="text-red-600 font-bold text-sm">
              {formatPrice(selectedItem.specialOffer)} تومان
            </span>
            <span className="text-gray-400 line-through text-sm">
              {formatPrice(selectedItem.price)} تومان
            </span>
          </div>
        ) : (
          <span className="text-gray-800 font-bold text-sm">
            {formatPrice(selectedItem.price)} تومان
          </span>
        );
      }
    }
    if (specialOffer && specialOffer < price) {
      return (
        <div className="flex flex-col items-end">
          <span className="text-red-600 font-bold text-sm">
            {formatPrice(specialOffer)} تومان
          </span>
          <span className="text-gray-400 line-through text-sm">
            {formatPrice(price)} تومان
          </span>
        </div>
      );
    }
    return (
      <span className="text-gray-800 font-bold text-sm">
        {formatPrice(price)} تومان
      </span>
    );
  };

  const handlePropertyClick = (clickedItem) => {
    if (!items) return;
    setSelectedItem(clickedItem);
    setQuantity(1);
    setInCart(false);
  };

  const handleFavorite = () => {
    setIsFavorite(true);
    Swal.fire({
      icon: "success",
      title: "افزوده شد!",
      text: "محصول به لیست علاقه‌مندی‌ها اضافه شد.",
      confirmButtonText: "باشه",
    });
  };

  // Determine border style based on specialOffer and recommended
  const borderClass =
    specialOffer && specialOffer < price && recommended
      ? "border-yellow-400 border-3 "
      : specialOffer && specialOffer < price
      ? "border-red-400 border-3 "
      : recommended
      ? "border-green-400 border-3 "
      : "";

  return (
    <>
      <div
        className={`bg-white mb-2 rounded-2xl w-full shadow-md  p-4 min-h-[430px] flex flex-col justify-between ${borderClass}`}
      >
        <div>
          <div className="relative">
            {image ? (
              <Image
                src={image}
                alt={name || "Product Image"}
                width={450}
                height={450}
                className="rounded-xl mx-auto"
              />
            ) : (
              <img
                src="/product.png"
                alt={name}
                className="rounded-xl mx-auto"
              />
            )}
            {/* Special Offer Ribbon */}
            {specialOffer && (
              <div className="absolute top-2 left-2 z-10">
                <span
                  className="bg-red-500 text-white px-3 py-1 rounded-full text-xs font-bold shadow-lg"
                  style={{ transform: "rotate(-45deg)" }}
                >
                  تخفیف ویژه
                </span>
              </div>
            )}
            <button
              className="absolute top-2 right-2 text-gray-500 hover:text-red-500"
              onClick={handleFavorite}
              disabled={isFavorite}
              aria-label="افزودن به علاقه‌مندی‌ها"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className={`h-6 w-6 fill-current ${
                  isFavorite ? "text-red-500" : ""
                }`}
                viewBox="0 0 24 24"
              >
                <path
                  d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 
                                                                                                                                                                                    5.42 4.42 3 7.5 3c1.74 0 3.41 0.81 4.5 
                                                                                                                                                                                    2.09C13.09 3.81 14.76 3 16.5 3 19.58 
                                                                                                                                                                                    3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 
                                                                                                                                                                                    11.54L12 21.35z"
                />
              </svg>
            </button>
            {/* پیشنهاد ما badge - پایین چپ روی تصویر */}
            {recommended && (
              <div className="absolute bottom-2 left-2 z-10">
                <span className="bg-yellow-400 text-gray-800 px-3 py-1 rounded-full text-xs font-bold shadow-lg ">
                  پیشنهاد ما
                </span>
              </div>
            )}
          </div>

          <h4 className="font-semibold">{name}</h4>
          {items && Array.isArray(items) && items.length > 0 ? (
            <div className="flex flex-row gap-2 mt-2 text-sm">
              {items.map((item, idx) => (
                <div
                  key={item.id || idx}
                  className="flex items-center justify-between bg-gray-100 px-2 py-1 rounded-full"
                >
                  <span
                    className={`cursor-pointer ${
                      item.selected
                        ? "text-orange-500 font-bold"
                        : "text-gray-800"
                    }`}
                    onClick={() => handlePropertyClick(item)}
                  >
                    {item.property}
                  </span>
                </div>
              ))}
            </div>
          ) : null}
        </div>
        <div className="flex justify-between items-center mt-4">
          {items && Array.isArray(items) && items.length > 0 ? (
            <span className="text-sm text-gray-400">
              {selectedItem && selectedItem.description
                ? selectedItem.description
                : description}
            </span>
          ) : (
            <span className="text-sm text-gray-500">
                {description}
                
                </span>
          )}
        </div>
        <div className="mt-4 grid items-center">
          {!inCart ? (
            <>
              <div className="flex items-center justify-between mb-2 bg-gray-100 px-2 py-1 rounded-xl">
                {showPrice()}
                <span className="text-gray-500 text-sm">
                  {items && Array.isArray(items) && items.length > 0
                    ? selectedItem && selectedItem.stock > 0
                      ? `در انبار: ${selectedItem.stock}`
                      : "ناموجود"
                    : stock > 0
                    ? `در انبار: ${stock} `
                    : "ناموجود"}
                </span>
              </div>
              <button
                className="bg-red-500 hover:bg-red-900 text-white text-sm font-medium px-2 py-2 rounded-xl  items-center text-center"
                onClick={handleAddToCart}
                disabled={loading}
              >
                {loading ? "در حال افزودن..." : "افزودن به سبد خرید"}
              </button>
            </>
          ) : (
            <div className="flex items-center justify-around ">
              <div className="flex items-center">
                <span className="text-gray-500 text-sm">تعداد در سبد شما</span>
              </div>
              <div className="flex items-center gap-2 border-2 border-gray-300 rounded-lg ">
                <button
                  className="bg-gray-200 px-2 py-1 rounded text-lg"
                  onClick={handleDecrease}
                >
                  -
                </button>
                <span className="text-center ">{quantity}</span>
                <button
                  className="bg-gray-200 px-2 py-1 rounded text-lg"
                  onClick={handleIncrease}
                >
                  +
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
