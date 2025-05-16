"use client";

import Link from "next/link";
import { faLocationPin } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
library.add(faLocationPin);

export default function HeaderAddress({ isLoggedIn = false }) {
    return (
        <div>
            {!isLoggedIn ? (
                <button className="bg-gray-100 border border-gray-300 text-gray-500 px-3 py-1 rounded-lg text-sm">
                    لطفا شهر خود را انتخاب کنید
                </button>
            ) : (
                <Link
                    href="#"
                    className="text-gray-700 hover:text-primary-600 flex items-center text-sm"
                >
                    <FontAwesomeIcon
                        icon={"fa-location-pin"}
                        className="w-4 h-4 text-gray-500 ps-2"
                    />
                    ارسال به تهران، تهران
                </Link>
            )}
        </div>
    );
}
