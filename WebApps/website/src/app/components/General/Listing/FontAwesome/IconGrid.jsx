'use client';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Link from 'next/link';
import { library } from '@fortawesome/fontawesome-svg-core';
import { 
  faFacebook, faTwitter, faInstagram, faLinkedin, 
  faYoutube, faTelegram, faWhatsapp, faTiktok 
} from '@fortawesome/free-brands-svg-icons';
import PropTypes from 'prop-types';

// افزودن آیکون‌ها
library.add(
  faFacebook, faTwitter, faInstagram, faLinkedin,
  faYoutube, faTelegram, faWhatsapp, faTiktok
);

export default function SocialIcons({
  icons = [],
  direction = 'horizontal',
  iconColor = 'text-gray-700',
  size = 'md',
  className = ''
}) {
  const getIcon = (iconName) => {
    const iconsMap = {
      'facebook': ['fab', 'facebook'],
      'twitter': ['fab', 'twitter'],
      'instagram': ['fab', 'instagram'],
      'linkedin': ['fab', 'linkedin'],
      'youtube': ['fab', 'youtube'],
      'telegram': ['fab', 'telegram'],
      'whatsapp': ['fab', 'whatsapp'],
      'tiktok': ['fab', 'tiktok'],
    };
    return iconsMap[iconName] || ['fas', 'circle'];
  };

  const sizeClasses = {
    sm: 'text-base',
    md: 'text-lg',
    lg: 'text-xl',
    xl: 'text-2xl'
  };

  const containerClasses = {
    sm: 'w-10 h-10',
    md: 'w-12 h-12',
    lg: 'w-14 h-14',
    xl: 'w-16 h-16'
  };

  return (
    <div className={`flex ${
      direction === 'horizontal' 
        ? 'flex-row space-x-4' 
        : 'flex-col space-y-4'
    } ${className}`}>
      {icons.map((icon, index) => {
        const iconConfig = typeof icon === 'string' 
          ? { name: icon, color: iconColor } 
          : { 
              name: icon.name, 
              color: icon.color || iconColor,
              url: icon.url || '#',
              className: icon.className || ''
            };

        return (
          <Link
            key={index}
            href={iconConfig.url}
            target="_blank"
            rel="noopener noreferrer"
            className={`
              ${iconConfig.color} hover:opacity-80 border-2 border-gray-300
              rounded-lg flex items-center justify-center
              ${containerClasses[size]} transition-all
              hover:border-gray-400 ${iconConfig.className}
            `}
          >
            <FontAwesomeIcon 
              icon={getIcon(iconConfig.name)} 
              className={sizeClasses[size]}
            />
          </Link>
        );
      })}
    </div>
  );
}

SocialIcons.propTypes = {
  icons: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.string),
    PropTypes.arrayOf(PropTypes.shape({
      name: PropTypes.string.isRequired,
      color: PropTypes.string,
      url: PropTypes.string,
      className: PropTypes.string
    }))
  ]).isRequired,
  direction: PropTypes.oneOf(['horizontal', 'vertical']),
  iconColor: PropTypes.string,
  size: PropTypes.oneOf(['sm', 'md', 'lg', 'xl']),
  className: PropTypes.string
};

SocialIcons.defaultProps = {
  icons: [],
  direction: 'horizontal',
  iconColor: 'text-gray-700',
  size: 'md',
  className: ''
};