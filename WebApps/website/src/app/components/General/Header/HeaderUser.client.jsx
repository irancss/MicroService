"use client";
import Cookie from "js-cookie";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

export default function HeaderClientActions() {
    const [user, setUser] = useState(null);
    const router = useRouter();

    useEffect(() => {
        const cookie = Cookie.get("user");
        setUser(cookie ? JSON.parse(decodeURIComponent(cookie)) : null);
    }, []);

    const isLoggedIn = !!user;

    const handleLogin = () => {
        if (isLoggedIn) {
            router.push("/panel");
        } else {
            router.push("/login");
        }
    };

    return (
        <>
            {!isLoggedIn ? (
                <button
                    onClick={handleLogin}
                    className="bg-amber-100 border border-amber-400 text-amber-700 px-3 py-1 rounded-lg text-sm"
                >
                    ورود یا ثبت نام
                </button>
            ) : (
                <div className="flex items-center gap-2">
                    <a href="#" className="text-gray-700 hover:text-primary-600 text-sm">
                        ورود
                    </a>
                    <span className="hidden sm:block border-l border-gray-300 h-6" />
                    <a href="#" className="text-gray-700 hover:text-primary-600 text-sm">
                        ثبت نام
                    </a>
                </div>
            )}
        </>
    );
}
