import Image from "next/image";
import Link from "next/link";

export default function Application() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 bg-gradient-to-r from-amber-300 via-yellow-200 to-amber-400 shadow-xl rounded-3xl gap-6 mb-10 px-6 py-4 md:mx-24 transition-all duration-300">
      <div className="flex items-center space-x-4 rounded-xl bg-white/70 p-3 shadow-md hover:scale-105 transition-transform w-full">
        <div className="justify-between flex items-center">
          <div className="flex bg-amber-100 p-2">
            <Image
              src="/logo.png"
              alt="Hero Image"
              width={40}
              height={40}
              className="h-auto object-cover"
            />
          <h3 className="text-2xl font-extrabold text-gray-800 drop-shadow-lg">
            نرم افزار دیجی کالا
          </h3>
           </div>
          <div className="flex items-center">
            <Link
              href="https://play.google.com/store"
              target="_blank"
              className="group"
            >
              <Image
                src="/google-play.png"
                alt="Google Play"
                width={28}
                height={28}
                className="h-auto object-cover mr-2"
              />
            </Link>
            <Link
              href="https://www.apple.com/app-store/"
              target="_blank"
              className="group"
            >
              <Image
                src="/google-play.png"
                alt="App Store"
                width={50}
                height={50}
                className="h-auto object-cover mr-2"
              />
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
