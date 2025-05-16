import Link from "next/link";
import {
  faBasketShopping,
  faStar,
  faList,
  faPercent,
  faBlog,
  faPhone,
  faLocationPin,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import HeaderClientActions from "./HeaderUser.client";
import HeaderAddress from "./HeaderAddress.client";
library.add(
  faStar,
  faBasketShopping,
  faList,
  faPercent,
  faBlog,
  faPhone,
  faLocationPin
);

export default function Header() {
  return (
    <header className="bg-white shadow-md sticky top-0 z-50">
      <div className="px-4 md:px-10 py-2">
        {/* Top Row */}
        <div className="flex flex-col md:flex-row justify-between gap-4 items-center">
          {/* Logo & Search */}
          <div className="flex items-center gap-4 w-full md:w-auto">
            <Link href="/">
              <img
                src="/logo.png"
                alt="Logo"
                className="w-28 h-10 md:w-32 md:h-12"
              />
            </Link>
            <form className="hidden sm:block border rounded-lg overflow-hidden w-full md:w-80">
              <input
                type="text"
                placeholder="جستجو..."
                className="px-4 py-2 w-full focus:outline-none border-gray-100 bg-gray-100"
              />
            </form>
          </div>
          {/* User & Cart */}
          <div className="flex items-center gap-3 mt-2 md:mt-0">
            <HeaderClientActions />
            <span className="hidden sm:block border-l border-gray-300 h-6" />
            <Link href="/cart" className="text-gray-700 hover:text-primary-600">
              <FontAwesomeIcon
                icon={"fa-basket-shopping"}
                className="w-5 h-5 text-gray-700"
              />
            </Link>
          </div>
        </div>
        {/* Divider */}
        <hr className="border border-t border-gray-300 mt-3" />
        {/* Nav Row */}
        <div className="flex flex-col md:flex-row justify-between gap-4 mt-4 items-center">
          <nav className="flex flex-wrap gap-2 md:gap-4 justify-center">
            <Link
              href="#"
              className="text-gray-700 hover:text-primary-600 flex items-center text-sm"
            >
              <FontAwesomeIcon
                icon={"fa-list"}
                className="w-4 h-4 text-gray-500 pe-2"
              />
              دسته بندی کالاها
            </Link>
            <Link
              href="#"
              className="text-gray-700 hover:text-primary-600 flex items-center text-sm"
            >
              <FontAwesomeIcon
                icon={"fa-percent"}
                className="w-4 h-4 text-gray-500 pe-2"
              />
              تخفیف‌ ها
            </Link>
            <Link
              href="#"
              className="text-gray-700 hover:text-primary-600 flex items-center text-sm"
            >
              <FontAwesomeIcon
                icon={"fa-blog"}
                className="w-4 h-4 text-gray-500 pe-2"
              />
              وبلاگ
            </Link>
            <Link
              href="#"
              className="text-gray-700 hover:text-primary-600 flex items-center text-sm"
            >
              <FontAwesomeIcon
                icon={"fa-phone"}
                className="w-4 h-4 text-gray-500 pe-2"
              />
              تماس با ما
            </Link>
          </nav>
          <div className="mt-2 md:mt-0">
            <HeaderAddress />
          </div>
        </div>
        {/* Mobile Search */}
        <form className="block sm:hidden mt-3 border rounded-lg overflow-hidden w-full">
          <input
            type="text"
            placeholder="جستجو..."
            className="px-4 py-2 w-full focus:outline-none border-gray-100 bg-gray-100"
          />
        </form>
      </div>
    </header>
  );
}
