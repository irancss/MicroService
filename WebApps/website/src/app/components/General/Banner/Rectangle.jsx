import Image from "next/image";
import PropTypes from 'prop-types';

/**
 * کامپوننت RectangleBanner - بنر مستطیلی با قابلیت‌های پیشرفته
 * 
 * @param {string} src - مسیر تصویر (الزامی)
 * @param {string} alt - متن جایگزین تصویر (الزامی)
 * @param {string} [ratio="16:9"] - نسبت ابعاد بنر (پیش‌فرض 16:9)
 * @param {string} [] - کلاس‌های سفارشی Tailwind
 * @param {ReactNode} children - محتوای دلخواه روی بنر
 * @param {boolean} [overlay=false] - لایه overlay تیره
 * @param {string} [contentPosition="center"] - موقعیت محتوا (center/start/end)
 * @param {string} [contentAlign="center"] - تراز محتوا (center/left/right)
 */
export default function RectangleBanner({
  src,
  alt,
  ratio = "16:9",
  className = "",
  children,
  overlay = false,
  contentPosition = "center",
  contentAlign = "center",
}) {
  // محاسبه نسبت ابعاد
  const ratioClasses = {
    "16:9": "aspect-w-16 aspect-h-9",
    "4:3": "aspect-w-4 aspect-h-3",
    "3:2": "aspect-w-3 aspect-h-2",
    "1:1": "aspect-w-1 aspect-h-1",
  };

  // کلاس‌های موقعیت محتوا
  const positionClasses = {
    center: "items-center justify-center",
    start: "items-start justify-start",
    end: "items-end justify-end",
  };

  // کلاس‌های تراز متن
  const alignClasses = {
    center: "text-center",
    left: "text-right", // برای راست به چپ
    right: "text-left", // برای راست به چپ
  };

  return (
    <div className={`relative rounded-lg overflow-hidden shadow-xl ${ratioClasses[ratio]} ${className}`}>
      {/* تصویر بنر */}
      <Image
        src={src}
        alt={alt}
        fill
        className="object-cover"
        priority
      />

      {/* لایه overlay (اختیاری) */}
      {overlay && (
        <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
      )}

      {/* محتوای دلخواه روی بنر */}
      {children && (
        <div className={`absolute inset-0 flex p-6 md:p-12 ${positionClasses[contentPosition]}`}>
          <div className={`max-w-2xl p-4 ${alignClasses[contentAlign]}`}>
            {children}
          </div>
        </div>
      )}
    </div>
  );
}

RectangleBanner.propTypes = {
  src: PropTypes.string.isRequired,
  alt: PropTypes.string.isRequired,
  ratio: PropTypes.oneOf(["16:9", "4:3", "3:2", "1:1"]),
  className: PropTypes.string,
  children: PropTypes.node,
  overlay: PropTypes.bool,
  contentPosition: PropTypes.oneOf(["center", "start", "end"]),
  contentAlign: PropTypes.oneOf(["center", "left", "right"]),
};