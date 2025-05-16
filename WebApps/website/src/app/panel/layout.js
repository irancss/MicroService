"use client";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
    faHome,
    faShoppingCart,
    faTicket,
    faWallet,
    faHeart,
    faQuestionCircle,
    faSignOutAlt,
    faEdit,
    faComment,
    faEnvelope,
    faGear,
    faEye,
    faLocationDot,
    faBell,
} from "@fortawesome/free-solid-svg-icons";
import Cookies from "js-cookie";
import Link from "next/link";

export default function PanelLayout({ children }) {
    const userData = Cookies.get("userData")
        ? JSON.parse(Cookies.get("userData"))
        : {
                id: 1,
                name: "امیر مهدی واعظ",
                wallet: 1000,
                email: "john.doe@example.com",
            };

    const iconMap = {
        "shopping-cart": faShoppingCart,
        "location-dot": faLocationDot,
        heart: faHeart,
        comment: faComment,
        "question-circle": faQuestionCircle,
        envelope: faEnvelope,
        eye: faEye,
        ticket: faTicket,
        gear: faGear,
        wallet: faWallet,
        "sign-out-alt": faSignOutAlt,
        home: faHome,
        bell: faBell,
    };

    function getIcon(iconName) {
        return iconMap[iconName] || faHome;
    }

    const navItems = [
        { name: "پروفایل", path: "/panel/", icon: "home" },
        { name: "سفارش ها", path: "/panel/orders", icon: "shopping-cart" },
        { name: "آدرس ها", path: "/panel/address", icon: "location-dot" },
        { name: "علاقه مندی ها", path: "/panel/wishlist", icon: "heart" },
        { name: "اطلاع رسانی ها", path: "/panel/notifications", icon: "bell" },
        { name: "دیدگاه ها", path: "/panel/reviews", icon: "comment" },
        { name: "پرسش ها", path: "/panel/questions", icon: "question-circle" },
        { name: "پیام ها", path: "/panel/messages", icon: "envelope" },
        { name: "بازدید های اخیر", path: "/panel/recent-visits", icon: "eye" },
        { name: "تیکت ها", path: "/panel/tickets", icon: "ticket", count: 5 },
        { name: "حساب کاربری", path: "/panel/account", icon: "gear" },
        { name: "کیف پول", path: "/panel/wallet", icon: "wallet" },
        { name: "خروج", path: "/logout", icon: "sign-out-alt" },
    ];

    return (
        <div className="container mx-auto px-2 py-6">
            <div className="flex gap-4">
                <aside className="w-48 bg-white dark:bg-gray-900 rounded-xl p-4 flex flex-col items-center border border-gray-200 dark:border-gray-800">
                    <div className="flex flex-col items-center mb-6">
                        <span className="text-lg font-semibold text-gray-800 dark:text-gray-200">{userData.name}</span>
                        <Link href="/panel/profile" className="mt-1 text-rose-600 hover:text-rose-800">
                            <FontAwesomeIcon icon={faEdit} />
                        </Link>
                        <span className="text-xs text-gray-400 mt-2">موجودی: {userData.wallet} تومان</span>
                    </div>
                    <nav className="w-full">
                        <ul className="flex flex-col gap-2">
                            {navItems.map((item) => (
                                <li key={item.name}>
                                    <Link
                                        href={item.path}
                                        className="flex items-center gap-2 px-2 py-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition text-gray-700 dark:text-gray-200 text-sm"
                                    >
                                        <FontAwesomeIcon icon={getIcon(item.icon)} className="text-gray-400" />
                                        <span>{item.name}</span>
                                        {item.count && (
                                            <span className="ml-auto text-xs bg-rose-500 text-white rounded-full px-2">
                                                {item.count}
                                            </span>
                                        )}
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </nav>
                </aside>
                <main className="flex-1 bg-white dark:bg-gray-900 rounded-xl p-6 border border-gray-200 dark:border-gray-800">
                    {children}
                </main>
            </div>
        </div>
    );
}
