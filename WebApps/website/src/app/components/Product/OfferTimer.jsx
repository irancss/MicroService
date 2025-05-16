"use client";

import { useState, useEffect } from "react";

export default function OfferTimer({ initialHours = 24, children }) {
  const [timeLeft, setTimeLeft] = useState({
    hours: initialHours,
    minutes: 0,
    seconds: 0,
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // شبیه‌سازی دریافت زمان از بک‌اند
    const fetchTimerFromBackend = async () => {
      try {
        // در اینجا می‌توانید درخواست واقعی به بک‌اند داشته باشید
        // const response = await fetch('/api/timer');
        // const data = await response.json();

        // شبیه‌سازی پاسخ بک‌اند
        const backendTime = {
          hours: 12, // مقدار نمونه از بک‌اند
          minutes: 30,
          seconds: 0,
        };

        setTimeLeft(backendTime);
      } catch (error) {
        console.error("Error fetching timer:", error);
        // اگر خطا رخ داد، از زمان پیش‌فرض استفاده می‌کند
        setTimeLeft({
          hours: initialHours,
          minutes: 0,
          seconds: 0,
        });
      } finally {
        setIsLoading(false);
      }
    };

    fetchTimerFromBackend();

    // تایمر برای شمارش معکوس
    const timer = setInterval(() => {
      setTimeLeft((prevTime) => {
        const { hours, minutes, seconds } = prevTime;

        if (hours === 0 && minutes === 0 && seconds === 0) {
          clearInterval(timer);
          return prevTime;
        }

        if (seconds === 0) {
          if (minutes === 0) {
            return {
              hours: hours - 1,
              minutes: 59,
              seconds: 59,
            };
          }
          return {
            hours,
            minutes: minutes - 1,
            seconds: 59,
          };
        }

        return {
          hours,
          minutes,
          seconds: seconds - 1,
        };
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [initialHours]);

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center bg-rose-100 p-4 rounded-lg shadow-md">
        <p className="text-rose-700 mb-4">در حال بارگذاری پیشنهاد ویژه...</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 items-center justify-center py-2  mb-3 shadow shadow-amber-500 bg-amber-100 px-3  rounded-2xl">
      <div className="text-amber-700  justify-start items-end">
        فقط برای مدت محدود!
      </div>
      <div className="flex  space-x-4 justify-end items-end ">
        <div className=" p-2  ">
          <span className="text-amber-700 font-bold text-xl">
            {String(timeLeft.hours).padStart(2, "0")}:
            {String(timeLeft.minutes).padStart(2, "0")}:
            {String(timeLeft.seconds).padStart(2, "0")}
          </span>
        </div>
      </div>
    </div>
  );
}
