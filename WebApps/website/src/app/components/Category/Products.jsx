import React from "react";
import ProductCart from "../General/Product/Cart";
import Cookies from "js-cookie";

// Use js-cookie for client-side cookie access
const DESKTOP_LIMIT = 15;
const MOBILE_LIMIT = 10;

// Only check cookies on client side
const isMobile = () => {
  if (typeof window === "undefined") return false;
  return Cookies.get("is-mobile") === "1";
};

const Dummy_Products = [
  {
    id: 1,
    name: "محصول 1",
    image: "/product.png",
    datePublished: "2023-01-01",
    items: [
      {
        id: 1,
        price: 800000,
        specialOffer: 500000,
        property: "ویژگی 1",
        selected: false,
        recommended: true,
        description : 'این ویژگی به شما این امکان را می‌دهد که با استفاده از آن، تجربه کاربری بهتری داشته باشید.',
        stock: 10,
      },
      {
        id: 2,
        price: 2,
        specialOffer: 800000,
        property: "ویژگی 2",
        selected: false,
        description : 'این ویژگی به شما این امکان را می‌دهد که با استفاده از آن، تجربه کاربری بهتری داشته باشید.',
        stock: 5,
      },
      {
        id: 3,
        price: 3,
        specialOffer: 800000,
        property: "ویژگی 3",
        selected: false,
        description : 'این ویژگی به شما ا3 که با استفاده از آن، تجربه کاربری بهتری داشته باشید.',
        stock: 0,
      },
    ],
  },
  {
    id: 2,
    name: "محصول 2",
    image: "/product.png",
    price: 2000000,
    datePublished: "2023-01-02",
    description: "این یک توضیحات تستی برای محصول 2 است. این محصول دارای ویژگی‌های خاصی است که آن را از سایر محصولات متمایز می‌کند.",
    stock: 20,
    recommended: true,
  },
  {
    id: 3,
    name: "محصول 3",
    image: "/product.png",
    price: 3000000,
    recommended: false,
    specialOffer: 2500000,
    datePublished: "2023-01-03",
    stock: 0,
  },
  {
    id: 4,
    name: "محصول 4",
    image: "/product.png",
    price: 4000000,
    recommended: true,
    specialOffer: 3500000,
    datePublished: "2023-01-04",
    stock: 1
  },
  {
    id: 5,
    name: "محصول 5",
    image: "/product.png",
    price: 5000000,
    datePublished: "2023-01-05",
  },
  {
    id: 6,
    name: "محصول 6",
    image: "/product.png",
    price: 6000000,
    datePublished: "2023-01-06",
  },
  {
    id: 7,
    name: "محصول 7",
    image: "/product.png",
    price: 7000000,
    datePublished: "2023-01-07",
  },
  {
    id: 8,
    name: "محصول 8",
    image: "/product.png",
    price: 8000000,
    datePublished: "2023-01-08",
  },
  {
    id: 9,
    name: "محصول 9",
    image: "/product.png",
    price: 9000000,
    datePublished: "2023-01-09",
  },
  {
    id: 10,
    name: "محصول 10",
    image: "/product.png",
    price: 10000000,
    datePublished: "2023-01-10",
  },
  {
    id: 11,
    name: "محصول 11",
    image: "/product.png",
    price: 11000000,
    datePublished: "2023-01-11",
  },
  {
    id: 12,
    name: "محصول 12",
    image: "/product.png",
    price: 12000000,
    datePublished: "2023-01-12",
  },
  {
    id: 13,
    name: "محصول 13",
    image: "/product.png",
    price: 13000000,
    datePublished: "2023-01-13",
  },
  {
    id: 14,
    name: "محصول 14",
    image: "/product.png",
    price: 14000000,
    datePublished: "2023-01-14",
  },

  {
    id: 15,
    name: "محصول 15",
    image: "/product.png",
    price: 15000000,
    datePublished: "2023-01-15",
  },
  {
    id: 16,
    name: "محصول 16",
    image: "/product.png",
    price: 16000000,
    datePublished: "2023-01-16",
  },
  {
    id: 17,
    name: "محصول 17",
    image: "/product.png",
    price: 17000000,
    datePublished: "2023-01-17",
  },
  {
    id: 18,
    name: "محصول 18",
    image: "/product.png",
    price: 18000000,
    datePublished: "2023-01-18",
  },
  {
    id: 19,
    name: "محصول 19",
    image: "/product.png",
    price: 19000000,
    datePublished: "2023-01-19",
  },
  {
    id: 20,
    name: "محصول 20",
    image: "/product.png",
    price: 20000000,
    datePublished: "2023-01-20",
  },
  {
    id: 21,
    name: "محصول 21",
    image: "/product.png",
    price: 21000000,
    datePublished: "2023-01-21",
  },
  {
    id: 22,
    name: "محصول 22",
    image: "/product.png",
    price: 22000000,
    datePublished: "2023-01-22",
  },
  {
    id: 23,
    name: "محصول 23",
    image: "/product.png",
    price: 23000000,
    datePublished: "2023-01-23",
  },
  {
    id: 24,
    name: "محصول 24",
    image: "/product.png",
    price: 24000000,
    datePublished: "2023-01-24",
  },
  {
    id: 25,
    name: "محصول 25",
    image: "/product.png",
    price: 25000000,
    datePublished: "2023-01-25",
  },
  {
    id: 26,
    name: "محصول 26",
    image: "/product.png",
    price: 26000000,
    datePublished: "2023-01-26",
  },
  {
    id: 27,
    name: "محصول 27",
    image: "/product.png",
    price: 27000000,
    datePublished: "2023-01-27",
  },
  {
    id: 28,
    name: "محصول 28",
    image: "/product.png",
    price: 28000000,
    datePublished: "2023-01-28",
  },
  {
    id: 29,
    name: "محصول 29",
    image: "/product.png",
    price: 29000000,
    datePublished: "2023-01-29",
  },
  {
    id: 30,
    name: "محصول 30",
    image: "/product.png",
    price: 30000000,
    datePublished: "2023-01-30",
  },
];

function getInitialSort() {
  if (typeof window !== "undefined") {
    const url = new URL(window.location.href);
    const sortParam = url.searchParams.get("sort");
    if (["latest", "price-asc", "price-desc", "popular"].includes(sortParam)) {
      return sortParam;
    }
  }
  return "latest";
}

function Products() {
  const [sort, setSort] = React.useState(getInitialSort());
  const [visibleCount, setVisibleCount] = React.useState(
    isMobile() ? MOBILE_LIMIT : DESKTOP_LIMIT
  );
  const [loading, setLoading] = React.useState(false);
  const [products, setProducts] = React.useState([]);
  const [hasMore, setHasMore] = React.useState(false);

  // Set visibleCount based on device after mount (client-side only)
  React.useEffect(() => {
    setVisibleCount(isMobile() ? MOBILE_LIMIT : DESKTOP_LIMIT);
  }, []);

  // بررسی نیاز به فراخوانی بک‌اند
  React.useEffect(() => {
    // Only run on client
    if (typeof window === "undefined") return;
    setLoading(true);
    setTimeout(() => {
      setProducts(Dummy_Products.slice(0, visibleCount));
      setHasMore(Dummy_Products.length > visibleCount);
      setLoading(false);
    }, 500);
    // Uncomment below for real API
    /*
        fetch(`/api/products?sort=${sort}&limit=${visibleCount}`)
            .then(res => res.json())
            .then(data => {
                setProducts(data.products);
                setHasMore(data.hasMore); // فرض بر این که بک‌اند این مقدار را برمی‌گرداند
            })
            .finally(() => setLoading(false));
        */
  }, [sort, visibleCount]);

  // Infinite scroll handler
  React.useEffect(() => {
    if (typeof window === "undefined") return;
    const handleScroll = () => {
      if (
        window.innerHeight + window.scrollY >=
          document.body.offsetHeight - 600 &&
        hasMore &&
        !loading
      ) {
        setLoading(true);
        setTimeout(() => {
          setVisibleCount((prev) => prev + (isMobile() ? 5 : 5));
          setLoading(false);
        }, 800);
      }
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, [hasMore, loading]);

  // Reset visibleCount on resize (for device change)
  React.useEffect(() => {
    if (typeof window === "undefined") return;
    const handleResize = () => {
      setVisibleCount(isMobile() ? MOBILE_LIMIT : DESKTOP_LIMIT);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const handleSort = (type) => {
    setSort(type);
    setVisibleCount(isMobile() ? MOBILE_LIMIT : DESKTOP_LIMIT);

    // اضافه کردن مرتب‌سازی به URL به صورت کوئری
    if (typeof window !== "undefined") {
      const url = new URL(window.location.href);
      if (type === "latest") {
        url.searchParams.delete("sort");
      } else {
        url.searchParams.set("sort", type);
      }
      window.history.replaceState({}, "", url);
    }
  };

  return (
    <div className="flex flex-col gap-4 w-full h-full">
      <div className="flex flex-col gap-2 w-full h-full bg-gray-100 rounded-lg shadow-md p-4">
        <div className="flex justify-between items-center mb-4">
          <div className="flex justify-center">
            <span className="text-sm text-gray-400">مرتب سازی بر اساس:</span>
            <ul className="flex px-2 text-sm">
              <li
                className={`px-2 hover:text-rose-500 cursor-pointer ${
                  sort === "latest" ? "text-rose-500 font-bold" : ""
                }`}
                onClick={() => handleSort("latest")}
                tabIndex={0}
                role="button"
                aria-pressed={sort === "latest"}
              >
                جدیدترین
              </li>
              <li
                className={`px-2 hover:text-rose-500 cursor-pointer ${
                  sort === "price-asc" ? "text-rose-500 font-bold" : ""
                }`}
                onClick={() => handleSort("price-asc")}
                tabIndex={0}
                role="button"
                aria-pressed={sort === "price-asc"}
              >
                ارزان‌ترین
              </li>
              <li
                className={`px-2 hover:text-rose-500 cursor-pointer ${
                  sort === "price-desc" ? "text-rose-500 font-bold" : ""
                }`}
                onClick={() => handleSort("price-desc")}
                tabIndex={0}
                role="button"
                aria-pressed={sort === "price-desc"}
              >
                گران‌ترین
              </li>
              <li
                className={`px-2 hover:text-rose-500 cursor-pointer ${
                  sort === "popular" ? "text-rose-500 font-bold" : ""
                }`}
                onClick={() => handleSort("popular")}
                tabIndex={0}
                role="button"
                aria-pressed={sort === "popular"}
              >
                پرفروش ترین
              </li>
            </ul>
          </div>
          <div>
            <span className="text-sm text-gray-400">
              تعداد محصولات: {Dummy_Products.length}
            </span>
          </div>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-1">
          {products.slice(0, visibleCount).map((product) => (
            <ProductCart
              key={product.id}
              name={product.name}
              image={product.image}
              price={product.price}
              specialOffer={product.specialOffer}
              recommended={product.recommended}
              items={product.items}
              datePublished={product.datePublished}
              stock={product.stock}
              description={product.description}
            />
          ))}
        </div>
        {loading && hasMore && (
          <div className="flex justify-center py-4">
            <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-rose-500"></div>
          </div>
        )}
      </div>
    </div>
  );
}

export default Products;
