import Link from 'next/link';
import PropTypes from 'prop-types';

/**
 * کامپوننت Breadcrumb بهینه شده برای سئو
 * @param {Object} props - ویژگی‌های کامپوننت
 * @param {Array} props.items - آیتم‌های مسیر
 * @param {string|React.ReactNode} [props.separator='/'] - جداکننده آیتم‌ها
 * @param {string} [props.className=''] - کلاس‌های اضافی کانتینر
 * @param {string} [props.itemClassName=''] - کلاس‌های اضافی آیتم‌ها
 * @param {string} [props.activeClassName='text-amber-500 font-medium'] - کلاس آیتم فعال
 */
export default function Breadcrumb({
  items = [],
  separator = '/',
  className = '',
  itemClassName = '',
  activeClassName = 'text-amber-500 font-medium'
}) {
  return (
    <nav className={`flex items-center text-sm ${className}`} aria-label="breadcrumb">
      <ol className="flex items-center space-x-2 rtl:space-x-reverse" itemScope itemType="https://schema.org/BreadcrumbList">
        {items.map((item, index) => (
          <li 
            key={index} 
            className="flex items-center"
            itemProp="itemListElement" 
            itemScope
            itemType="https://schema.org/ListItem"
          >
            {index > 0 && (
              <span className="mx-2 text-gray-400" aria-hidden="true">
                {separator}
              </span>
            )}
            {index < items.length - 1 ? (
              <Link
                href={item.href || '#'}
                className={`text-gray-500 hover:text-amber-500 transition-colors ${itemClassName}`}
                itemProp="item"
              >
                <span itemProp="name">{item.label}</span>
                <meta itemProp="position" content={String(index + 1)} />
              </Link>
            ) : (
              <span className={`${activeClassName} ${itemClassName}`} itemProp="name">
                {item.label}
                <meta itemProp="position" content={String(index + 1)} />
              </span>
            )}
          </li>
        ))}
      </ol>
    </nav>
  );
}

// تعریف PropTypes
Breadcrumb.propTypes = {
  items: PropTypes.arrayOf(
    PropTypes.shape({
      label: PropTypes.string.isRequired,
      href: PropTypes.string
    })
  ).isRequired,
  separator: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.node
  ]),
  className: PropTypes.string,
  itemClassName: PropTypes.string,
  activeClassName: PropTypes.string
};

// مقادیر پیش‌فرض
Breadcrumb.defaultProps = {
  items: [],
  separator: '/',
  className: '',
  itemClassName: '',
  activeClassName: 'text-amber-500 font-medium'
};