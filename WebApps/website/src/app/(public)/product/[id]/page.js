import Breadcrumb from "@components/General/Breadcrumb";
import Images from "@components/Product/Images";
import MainSectionProduct from "@components/Product/Main";
import OfferTimer from "@components/Product/OfferTimer";
import OurFeature from "@components/Product/Our-Feature";
import Sellers from "@components/Product/Sellers";
import SidebarProduct from "@components/Product/Sidebar";
import SimilarProduct from "@components/Product/Similar";
import VideoReview from "@components/Product/Video-Review";
import Introduction from "@components/Product/Introduction";
import { cookies } from "next/headers";
import Specifications from "@components/Product/Specifications";
import Review from "@components/Product/Review";
import Question from "@components/Product/Question";
import BriefSidebar from "@components/Product/Breif-Sidebar";

export default function ProductPage({ params }) {
  const cookieStore = cookies();
  const isMobile = cookieStore.get("is-mobile")?.value === "1";
  return (
    <div className="mx-auto max-w-[100vw] md:px-3 px-2 my-5 overflow-x-hidden">
      <div className="grid grid-cols-1 md:grid-cols-12 gap-4">
        {/* بخش سمت چپ */}
        <div className="md:col-span-4 col-span-full order-1 md:order-none">
          <Breadcrumb
            items={[
              { label: "خانه", href: "/" },
              { label: "محصولات", href: "/products" },
              { label: `محصول ${params.id}` },
            ]}
            separator="/"
            className="mb-4 px-2"
            itemClassName="text-gray-500 text-sm md:text-base"
            activeClassName="text-amber-500 font-medium"
          />

          <OfferTimer className="mx-2" />
          <div className="flex flex-col gap-4 px-2">
            <Images />
          </div>
        </div>

        <div className="md:col-span-5 col-span-full hidden md:block order-3 md:order-none p-2">
          <MainSectionProduct />
        </div>

        <div className="md:col-span-3 col-span-full order-2 md:order-none p-2">
          <SidebarProduct />
        </div>
      </div>

      <div className="flex flex-col gap-4 mt-4">
        <OurFeature />
        <div className="md:visible hidden md:block">
          <Sellers className="mb-4" />
        </div>
        <SimilarProduct />
      </div>

      {isMobile ? (
        <div className="flex flex-col gap-6 mt-4">
          {/* نسخه موبایل - ویدیو بالا */}
          <VideoReview />
          <Introduction />
          <Specifications />
        </div>
      ) : (
        <div className="grid grid-cols-3 gap-6 mt-4 relative">
          {/* نسخه دسکتاپ */}
          <div className="col-span-2">
            <div>
              <Introduction />
            </div>
            <div className="mt-4">
              <Specifications />
            </div>
          </div>
          {/* بخش ویدیو با استیکینگ */}
          <div className="col-span-1 sticky top-4 h-fit self-start">
            <VideoReview />
          </div>
        </div>
      )}
      <hr className="border-1 border-gray-300 my-3" />
      {isMobile ? (
        <p></p>
      ) : (
        <div className="grid grid-cols-10 gap-6 relative">
          <div className="col-span-8">
            <div className="border-2 border-gray-300 rounded-lg md:px-3 px-2">
              <Review />
            </div>
            <div className="mt-4">
            <Question />
            </div>
          </div>
          <div className="col-span-2 sticky top-4 h-fit self-start">
            <BriefSidebar />
          </div>
        </div>
      )}
    </div>
  );
}
