"use client";
import { useState } from "react";
import AnimatedHr from "@components/Animated/Hr";
import Swal from "sweetalert2";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faStar } from "@fortawesome/free-solid-svg-icons";
import { library } from "@fortawesome/fontawesome-svg-core";
library.add(faStar);

export default function ReviewsPage() {
  const initialReviews = [
    {
      id: 1,
      productName: "محصول 1",
      reviewText: "این محصول عالی است!",
      rating: 5,
      state: "منتظر تایید",
      supplier: "تامین کننده 1",
      color: "قرمز",
      time: "1403/03/01",
    },
    {
      id: 2,
      productName: "محصول 2",
      reviewText: "کیفیت خوبی دارد.",
      rating: 4,
      state: "رد شده",
      supplier: "تامین کننده 2",
      color: "آبی",
      time: "1403/03/02",
    },
    {
      id: 3,
      productName: "محصول 3",
      reviewText: "بهتر از انتظار بود.",
      rating: 5,
      state: "تایید شده",
      supplier: "تامین کننده 3",
      color: "سبز",
      time: "1403/03/03",
    },
  ];

  const [reviews, setReviews] = useState(initialReviews);

  const handleDelete = (id) => {
    Swal.fire({
      title: "حذف نظر",
      text: "آیا از حذف این نظر اطمینان دارید؟",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "بله، حذف کن",
      cancelButtonText: "انصراف",
      confirmButtonColor: "#d33",
      cancelButtonColor: "#aaa",
    }).then((result) => {
      if (result.isConfirmed) {
        setReviews(reviews.filter((r) => r.id !== id));
        Swal.fire({
          icon: "success",
          title: "حذف شد!",
          text: "نظر با موفقیت حذف شد.",
          confirmButtonText: "باشه",
          confirmButtonColor: "#d33",
        });
      }
    });
  };

  const handleEdit = (id, currentText) => {
    Swal.fire({
      title: "ویرایش نظر",
      input: "textarea",
      inputLabel: "متن جدید نظر را وارد کنید:",
      inputValue: currentText,
      showCancelButton: true,
      confirmButtonText: "ذخیره",
      cancelButtonText: "انصراف",
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#aaa",
      inputValidator: (value) => {
        if (!value) {
          return "متن نظر نمی‌تواند خالی باشد!";
        }
      },
    }).then((result) => {
      if (result.isConfirmed) {
        setReviews(
          reviews.map((r) =>
            r.id === id ? { ...r, reviewText: result.value } : r
          )
        );
        Swal.fire({
          icon: "success",
          title: "ویرایش شد!",
          text: "نظر با موفقیت ویرایش شد.",
          confirmButtonText: "باشه",
          confirmButtonColor: "#3085d6",
        });
      }
    });
  };

  return (
    <div className="mt-8">
      <h2 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
        <FontAwesomeIcon
          icon="star"
          className="text-yellow-400 animate-bounce"
        />
        لیست نظرات
      </h2>
      <AnimatedHr className="my-6" />
      <ul className="mt-6 space-y-6">
        {reviews.map((review) => (
          <li
            key={review.id}
            className="flex flex-col md:flex-row items-start md:items-center justify-between bg-gradient-to-r from-white via-gray-50 to-gray-100 rounded-2xl shadow-lg p-6 border border-gray-200 hover:shadow-2xl transition-all duration-300"
          >
            <div className="flex-1 w-full">
              <div className="flex items-center gap-3 mb-2">
                <span className="inline-block w-2 h-8 rounded bg-gradient-to-b from-yellow-400 to-yellow-200"></span>
                <strong className="text-lg text-gray-700">
                  {review.productName}
                </strong>
              </div>
              <p className="text-gray-700 mb-2">{review.reviewText}</p>
              <div className="flex items-center gap-1 mb-2">
                {[...Array(review.rating)].map((_, i) => (
                  <FontAwesomeIcon
                    key={i}
                    icon="star"
                    className="text-yellow-400 drop-shadow"
                  />
                ))}
                <span className="ml-2 text-sm text-gray-500">
                  {review.rating} / 5
                </span>
              </div>
              <div className="flex flex-wrap gap-4 mt-2 text-xs text-gray-500">
                <span className="flex items-center gap-1">
                  <FontAwesomeIcon icon="star" className="text-blue-400" />
                  تامین کننده: {review.supplier}
                </span>
                <span className="flex items-center gap-1">
                  <svg
                    width="14"
                    height="14"
                    fill="none"
                    viewBox="0 0 24 24"
                    className="inline-block text-green-400"
                  >
                    <circle
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="2"
                    />
                    <path
                      d="M8 12l2 2 4-4"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                  تاریخ: {review.time}
                </span>
                <span className="flex items-center gap-1">
                  <span
                    className="w-3 h-3 rounded-full border"
                    style={{ backgroundColor: review.color }}
                  ></span>
                  رنگ: {review.color}
                </span>
              </div>
            </div>
            <div className="flex flex-col md:flex-row items-center gap-3 mt-4 md:mt-0">
              <button
                className="px-4 py-1 rounded-full bg-blue-50 text-blue-600 hover:bg-blue-100 font-semibold shadow transition"
                onClick={() => handleEdit(review.id, review.reviewText)}
              >
                ویرایش
              </button>
              <button
                className="px-4 py-1 rounded-full bg-red-50 text-red-600 hover:bg-red-100 font-semibold shadow transition"
                onClick={() => handleDelete(review.id)}
              >
                حذف
              </button>
              <span
                className={`px-4 py-1 rounded-full font-semibold shadow transition cursor-default
                                ${
                                  review.state === "منتظر تایید"
                                    ? "bg-yellow-100 text-yellow-700 border border-yellow-300"
                                    : review.state === "رد شده"
                                    ? "bg-red-100 text-red-700 border border-red-300"
                                    : review.state === "تایید شده"
                                    ? "bg-green-100 text-green-700 border border-green-300"
                                    : "bg-gray-100 text-gray-500 border border-gray-200"
                                }
                            `}
              >
                {review.state}
              </span>
            </div>
          </li>
        ))}
        {reviews.length === 0 && (
          <li className="text-center text-gray-400 text-lg py-8 animate-pulse">
            نظری وجود ندارد.
          </li>
        )}
      </ul>
    </div>
  );
}
