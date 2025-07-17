import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon as OriginalFAIcon } from '@fortawesome/react-fontawesome';
import { getIcon } from '@lib/fontAwesome';

export default function  FontAwesomeIcon  ({ icon, className, ...props })  {
  let iconDefinition;

  if (typeof icon === 'string') {
    if (icon.includes(' ')) {
      // فرمت "prefix iconName" مثل "fas fa-box"
      const [prefix, iconNameWithFa] = icon.split(' ');
      const iconName = iconNameWithFa?.replace(/^fa-/, '') || '';
      iconDefinition = getIcon(iconName);
    } else {
      // فرمت تک‌کلمه‌ای مثل "box"
      iconDefinition = getIcon(icon);
    }
  } else {
    iconDefinition = icon;
  }

  if (!iconDefinition) {
    console.warn(`آیکون "${icon}" یافت نشد!`);
    return <OriginalFAIcon icon={iconMap.question} className={className} {...props} />;
  }

  return <OriginalFAIcon icon={iconDefinition} className={className} {...props} />;
};