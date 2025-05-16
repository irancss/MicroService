'use client';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faCheckCircle, faChevronRight, faStar, faCircle, faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import PropTypes from 'prop-types';

// افزودن آیکون‌ها به کتابخانه
library.add(faCheckCircle, faChevronRight, faStar, faCircle, faInfoCircle);

/**
 * کامپوننت لیست عمودی پیشرفته با آیکون‌های سفارشی برای هر آیتم
 * 
 * @param {Array} items - آرایه‌ای از آیتم‌ها
 * @param {boolean} numbered - نمایش شماره‌گذاری
 * @param {string} bgColor - رنگ پس‌زمینه
 * @param {string} textColor - رنگ متن پیش‌فرض
 * @param {string} className - کلاس‌های اضافی
 */
export default function OlVertical({
  items = [],
  numbered = true,
  bgColor = 'bg-gray-50',
  textColor = 'text-gray-700',
  className = '',
}) {
  // تبدیل نام آیکون به فرمت قابل قبول
  const getIcon = (iconName) => {
    const iconsMap = {
      'check-circle': 'check-circle',
      'chevron-right': 'chevron-right',
      'star': 'star',
      'circle': 'circle',
      'info': 'info-circle',
    };
    return iconsMap[iconName] || 'chevron-right';
  };

  return (
    <div className={`flex flex-col w-full p-6 ${bgColor} rounded-lg shadow-sm ${className}`}>
      <ol className={`${numbered ? 'list-decimal' : 'list-none'} space-y-4`}>
        {items.map((item, index) => {
          const {
            text = typeof item === 'string' ? item : item.text || item.title || '',
            icon = item.icon || 'chevron-right',
            iconColor = item.iconColor || 'text-gray-600',
            itemTextColor = item.textColor || textColor,
            description = item.description || null
          } = typeof item === 'object' ? item : { text: item };

          return (
            <li key={index} className="flex flex-col gap-2">
              <div className="flex items-start gap-3">
                <span className={`${iconColor} mt-1 flex-shrink-0`}>
                  <FontAwesomeIcon 
                    icon={Array.isArray(icon) ? icon : getIcon(icon)} 
                    className="w-4 h-4"
                  />
                </span>
                <span className={`${itemTextColor} font-medium`}>{text}</span>
              </div>
              {description && (
                <p className={`text-sm opacity-80 pr-7 ${itemTextColor}`}>
                  {description}
                </p>
              )}
            </li>
          );
        })}
      </ol>
    </div>
  );
}

OlVertical.propTypes = {
  items: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.string),
    PropTypes.arrayOf(PropTypes.shape({
      text: PropTypes.string,
      title: PropTypes.string,
      description: PropTypes.string,
      icon: PropTypes.string,
      iconColor: PropTypes.string,
      textColor: PropTypes.string
    }))
  ]).isRequired,
  numbered: PropTypes.bool,
  bgColor: PropTypes.string,
  textColor: PropTypes.string,
  className: PropTypes.string
};

OlVertical.defaultProps = {
  items: [],
  numbered: true,
  bgColor: 'bg-gray-50',
  textColor: 'text-gray-700',
  className: ''
};