'use client';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import {
  faCheckCircle, faChevronRight, faStar, faCircle,
  faInfoCircle, faLocationDot, faDotCircle, faPhone, faEnvelope
} from '@fortawesome/free-solid-svg-icons';
import Link from 'next/link';
import PropTypes from 'prop-types';

// افزودن آیکون‌ها به کتابخانه
library.add(
  faCheckCircle, faChevronRight, faStar, faCircle,
  faInfoCircle, faLocationDot, faDotCircle, faPhone, faEnvelope
);

export default function OlHorizontal({
  items = [],
  numbered = true,
  bgColor = 'bg-gray-50',
  textColor = 'text-gray-700',
  className = '',
}) {
  const getIcon = (iconName) => {
    const iconsMap = {
      'check-circle': 'check-circle',
      'chevron-right': 'chevron-right',
      'star': 'star',
      'circle': 'circle',
      'info': 'info-circle',
      'location': 'location-dot',
      'location-dot': 'location-dot',
      'dot-circle': 'dot-circle',
      'phone': 'phone',
      'email': 'envelope',
      'mail': 'envelope'
    };
    return iconsMap[iconName] || 'circle';
  };

  return (
    <div className={`flex flex-col w-full ${bgColor} rounded-lg  ${className}`}>
      <ol className={`${numbered ? 'list-decimal' : 'list-none'} list-inside space-y-3`}>
        {items.map((item, index) => {
          const {
            text = typeof item === 'string' ? item : item.text || item.title || '',
            icon = item.icon || 'circle',
            iconColor = item.iconColor || 'text-gray-600',
            itemTextColor = item.textColor || textColor,
            description = item.description || null,
            href = item.href || null,
            target = item.target || '_self'
          } = typeof item === 'object' ? item : { text: item };

          const content = (
            <>
              <span className={`${iconColor} mt-1 flex-shrink-0`}>
                <FontAwesomeIcon 
                  icon={Array.isArray(icon) ? icon : getIcon(icon)} 
                  className="w-3 h-3"
                />
              </span>
              <span className={`${itemTextColor}`}>
                {description ? (
                  <div>
                    <p className="font-medium">{text}</p>
                    <p className="text-sm opacity-80 mt-1">{description}</p>
                  </div>
                ) : (
                  text
                )}
              </span>
            </>
          );

          return (
            <li key={index} className="flex items-start gap-3">
              {href ? (
                <Link 
                  href={href} 
                  target={target}
                  rel={target === '_blank' ? 'noopener noreferrer' : undefined}
                  className="flex items-start gap-3 w-full hover:opacity-80 transition-opacity"
                >
                  {content}
                </Link>
              ) : (
                content
              )}
            </li>
          );
        })}
      </ol>
    </div>
  );
}

OlHorizontal.propTypes = {
  items: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.string),
    PropTypes.arrayOf(PropTypes.shape({
      text: PropTypes.string,
      title: PropTypes.string,
      description: PropTypes.string,
      icon: PropTypes.string,
      iconColor: PropTypes.string,
      textColor: PropTypes.string,
      href: PropTypes.string,
      target: PropTypes.string
    }))
  ]).isRequired,
  numbered: PropTypes.bool,
  bgColor: PropTypes.string,
  textColor: PropTypes.string,
  className: PropTypes.string
};

OlHorizontal.defaultProps = {
  items: [],
  numbered: true,
  bgColor: 'bg-gray-50',
  textColor: 'text-gray-700',
  className: ''
};