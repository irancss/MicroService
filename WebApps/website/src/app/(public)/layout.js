 import MainFooter from "@components/Footers/Main";
import PostFooter from "@components/Footers/Postfooter";
import Header from "@components/General/Header/Header";
import BannerHeader from "@components/General/Banner-Header";

export default function PublicLayout({ children }) {
  return (
    <>
      <div className="grid grid-cols-1 gap-4">
        <BannerHeader />
        <div className="flex flex-col gap-4">
          <Header />
          <div className="w-full">{children}</div>
        </div>
      </div>
      <footer>
        <MainFooter />
        <PostFooter />
      </footer>
    </>
  );
}
