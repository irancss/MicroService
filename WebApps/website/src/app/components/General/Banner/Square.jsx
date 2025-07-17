import Image from "next/image";
import PropTypes from 'prop-types';

/**
 * کامپوننت Banner - کاملاً قابل تنظیم با Tailwind
 * 
 * @param {string} src - مسیر تصویر (الزامی)
 * @param {string} alt - متن جایگزین تصویر (الزامی)
 * @param {number} [width=1200] - عرض بنر (پیکسل)
 * @param {number} [height=400] - ارتفاع بنر (پیکسل)
 * @param {string} [] - کلاس‌های سفارشی Tailwind
 * @param {ReactNode} children - محتوای دلخواه روی بنر
 * @param {boolean} [overlay=false] - آیا لایه overlay تیره اضافه شود؟
 * @param {string} [position="center"] - موقعیت محتوا (`center`، `start`، `end`)
 */
export default function Banner({
  src,
  alt,
  width = 1200,
  height = 400,
  className = "",
  children,
  overlay = false,
  position = "center"
}) {
  // تعیین موقعیت محتوا بر اساس پارامتر position
  const positionClasses = {
    center: "items-center justify-center",
    start: "items-center justify-start",
    end: "items-center justify-end"
  };

  return (
    <div className={`relative w-full rounded-xl overflow-hidden shadow-lg ${className}`}>
      {/* تصویر بنر */}
      <Image
        src={src}
        alt={alt}
        width={width}
        height={height}
        className="w-full h-auto object-cover"
        priority
      />

      {/* لایه overlay (اختیاری) */}
      {overlay && (
        <div className="absolute inset-0 bg-black/30"></div>
      )}

      {/* محتوای دلخواه روی بنر */}
      {children && (
        <div className={`absolute inset-0 flex p-8 ${positionClasses[position]}`}>
          <div className="text-white max-w-2xl">
            {children}
          </div>
        </div>
      )}
    </div>
  );
}

Banner.propTypes = {
  src: PropTypes.string.isRequired,
  alt: PropTypes.string.isRequired,
  width: PropTypes.number,
  height: PropTypes.number,
  className: PropTypes.string,
  children: PropTypes.node,
  overlay: PropTypes.bool,
  position: PropTypes.oneOf(["center", "start", "end"])
};