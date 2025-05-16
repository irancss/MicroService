"use client";
import Head from "next/head";
import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import Swal from "sweetalert2";
import withReactContent from "sweetalert2-react-content";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import OpenLayer from "@/app/components/Panel/Address/Map/OpenLayer";
import { faEdit, faHome, faTrash } from "@fortawesome/free-solid-svg-icons";
import AnimatedHr from "@/app/components/Animated/Hr";
library.add(faHome, faTrash, faEdit);

const MySwal = withReactContent(Swal);

export default function Address() {
  const [addresses, setAddresses] = useState([
    {
      id: 1,
      name: "آدرس 1",
      state: "تهران",
      city: "تهران",
      street: "خیابان ولیعصر",
      postalCode: "123456789",
      pelak: "10",
      vahed: "1",
      address: "تهران، خیابان ولیعصر، پلاک 10، واحد 1",
    },
    {
      id: 2,
      name: "آدرس 2",
      state: "اصفهان",
      city: "اصفهان",
      street: "خیابان چهارباغ",
      postalCode: "987654321",
      pelak: "20",
      vahed: "2",
      address: "اصفهان، خیابان چهارباغ، پلاک 20، واحد 2",
    },
  ]);
  const nextIdRef = useState(addresses.length + 1)[0];

  const dummyData = [
    {
      id: 1,
      name: "آدرس 1",
      state: "تهران",
      city: "تهران",
      street: "خیابان ولیعصر",
      postalCode: "123456789",
      pelak: "10",
      vahed: "1",
      address: (state) =>
        `${state.city}، ${state.street}، پلاک ${state.pelak}، واحد ${state.vahed}`,
    },
    {
      id: 2,
      name: "آدرس 2",
      state: "اصفهان",
      city: "اصفهان",
      street: "خیابان چهارباغ",
      postalCode: "987654321",
      pelak: "20",
      vahed: "2",
      address: (state) =>
        `${state.city}، ${state.street}، پلاک ${state.pelak}، واحد ${state.vahed}`,
    },
  ];

  useEffect(() => {
    async function fetchAddresses() {
      try {
        const res = await fetch("/api/addresses");
        if (!res.ok) throw new Error();
        const data = await res.json();
        setAddresses(data);
      } catch {
        setAddresses(dummyData);
      }
    }
    fetchAddresses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  // (handleEdit function removed as it is unused)

  const handleAdd = async () => {
    const { value: formValues } = await MySwal.fire({
      title: "افزودن آدرس جدید",
      html:
        `<input id="swal-input1" class="swal2-input" placeholder="نام"/>` +
        `<input id="swal-input2" class="swal2-input" placeholder="آدرس"/>`,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "افزودن",
      cancelButtonText: "انصراف",
      preConfirm: () => {
        return [
          document.getElementById("swal-input1").value,
          document.getElementById("swal-input2").value,
        ];
      },
    });

    setAddresses((prev) => {
      const newId = nextIdRef + prev.length;
      return [
        ...prev,
        { id: newId, name: formValues[0], address: formValues[1] },
      ];
    });
    MySwal.fire("موفقیت", "آدرس جدید اضافه شد!", "success");
  };

  // حذف آدرس
  const handleDelete = async (addr) => {
    const result = await MySwal.fire({
      title: "حذف آدرس",
      text: "آیا از حذف این آدرس مطمئن هستید؟",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "بله، حذف شود",
      cancelButtonText: "انصراف",
    });
    if (result.isConfirmed) {
      setAddresses(addresses.filter((a) => a.id !== addr.id));
      MySwal.fire("حذف شد!", "آدرس با موفقیت حذف شد.", "success");
    }
  };

  // ویرایش با نقشه
  const handleEditWithMap = async (addr) => {
    // If address is a function, call it with addr to get the string
    let selectedAddress =
      typeof addr.address === "function"
        ? addr.address(addr)
        : addr.address || "";

    let mapContainer = document.createElement("div");
    mapContainer.style.width = "100%";
    mapContainer.style.height = "300px";
    mapContainer.id = "swal-map-container";

    // مقدار اولیه مختصات (مثلا تهران)
    let initialCoords = [51.389, 35.6892];
    // اگر addr مختصات داشت، مقداردهی کن
    if (addr.coords) initialCoords = addr.coords;

    // متغیر برای ذخیره مختصات انتخابی
    let selectedCoords = initialCoords;

    // تابع برای بروزرسانی آدرس از نقشه
    const handleMapChange = (coords, addressText) => {
      selectedCoords = coords;
      if (addressText) {
        document.getElementById("swal-input-address").value = addressText;
        selectedAddress = addressText;
      }
    };

    // رندر OpenLayer درون SweetAlert
    setTimeout(() => {
      // React component render
      import("react-dom/client").then((ReactDOMClient) => {
        ReactDOMClient.createRoot(mapContainer).render(
          <OpenLayer
            initialCoords={initialCoords}
            onAddressChange={handleMapChange}
          />
        );
      });
    }, 0);

    const { value: formValues } = await MySwal.fire({
      title: "ویرایش آدرس",
      html: `
          <div style="display: flex; flex-wrap: wrap; gap: 8px; width: 100%;">
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-name" class="swal2-input" placeholder="نام" value="${
                addr.name || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-state" class="swal2-input" placeholder="استان" value="${
                addr.state || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-city" class="swal2-input" placeholder="شهر" value="${
                addr.city || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-street" class="swal2-input" placeholder="خیابان" value="${
                addr.street || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-postalCode" class="swal2-input" placeholder="کد پستی" value="${
                addr.postalCode || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-pelak" class="swal2-input" placeholder="پلاک" value="${
                addr.pelak || ""
              }"/>
            </div>
            <div style="flex: 1 1 48%; min-width: 140px;">
              <input id="swal-input-vahed" class="swal2-input" placeholder="واحد" value="${
                addr.vahed || ""
              }"/>
            </div>
            <div style="flex: 1 1 100%; min-width: 140px;">
              <input id="swal-input-address" class="swal2-input" placeholder="آدرس" value="${selectedAddress}"/>
            </div>
            <div id="swal-map-container" style="width:100%;height:300px;margin-top:10px;flex: 1 1 100%;"></div>
          </div>
          <style>
            @media (max-width: 600px) {
              .swal2-popup {
                width: 98vw !important;
                max-width: 98vw !important;
                min-width: 0 !important;
              }
              #swal-map-container {
                height: 200px !important;
              }
            }
          </style>
        `,
      customClass: {
        popup: "swal2-responsive-popup",
      },
      width: "70vw",
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "ذخیره",
      cancelButtonText: "انصراف",
      didOpen: () => {
        document.getElementById("swal-map-container").appendChild(mapContainer);
        import("react-dom/client").then((ReactDOMClient) => {
          ReactDOMClient.createRoot(mapContainer).render(
            <OpenLayer
              initialCoords={initialCoords}
              onAddressChange={handleMapChange}
            />
          );
        });
      },
      preConfirm: () => {
        return [
          document.getElementById("swal-input-name").value,
          document.getElementById("swal-input-state").value,
          document.getElementById("swal-input-city").value,
          document.getElementById("swal-input-street").value,
          document.getElementById("swal-input-postalCode").value,
          document.getElementById("swal-input-pelak").value,
          document.getElementById("swal-input-vahed").value,
          document.getElementById("swal-input-address").value,
          selectedCoords,
        ];
      },
    });

    if (formValues) {
      setAddresses(
        addresses.map((a) =>
          a.id === addr.id
            ? {
                ...a,
                name: formValues[0],
                state: formValues[1],
                city: formValues[2],
                street: formValues[3],
                postalCode: formValues[4],
                pelak: formValues[5],
                vahed: formValues[6],
                address: formValues[7],
                coords: formValues[8],
              }
            : a
        )
      );
      MySwal.fire("موفقیت", "آدرس با موفقیت ویرایش شد!", "success");
    }
  };

  return (
    <>
      <Head>
        <link
          rel="preload"
          href="/_next/static/css/app/layout.css?v=1746834662907"
          as="style"
        />
        <link
          rel="stylesheet"
          href="/_next/static/css/app/layout.css?v=1746834662907"
        />
      </Head>
      <div>
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-bold">آدرس‌ها</h2>
          <button
            onClick={handleAdd}
            className="bg-rose-500 text-white px-4 py-2 rounded"
          >
            افزودن آدرس جدید
          </button>
        </div>
        <AnimatedHr />

        <div className="mb-4">
          <AnimatePresence>
            {addresses.map((addr) => (
              <motion.div
                key={addr.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -20 }}
                transition={{ duration: 0.3 }}
                style={{
                  border: "1px solid #ccc",
                  borderRadius: 8,
                  padding: 16,
                  marginBottom: 12,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                }}
              >
                <div
                  style={{ flex: 1, cursor: "pointer" }}
                  onClick={() => handleEditWithMap(addr)}
                >
                  <h3>{addr.name}</h3>
                  <p>
                    <span style={{ fontWeight: "bold" }}>آدرس کامل :</span>{" "}
                    {typeof addr.address === "function"
                      ? addr.address(addr)
                      : addr.address}
                  </p>
                  <div style={{ fontSize: "0.9em", color: "#555" }}>
                    {addr.state && <span>استان: {addr.state} </span>}
                    {addr.city && <span>شهر: {addr.city} </span>}
                    {addr.street && <span>خیابان: {addr.street} </span>}
                    {addr.postalCode && <span>کدپستی: {addr.postalCode} </span>}
                    {addr.pelak && <span>پلاک: {addr.pelak} </span>}
                    {addr.vahed && <span>واحد: {addr.vahed} </span>}
                  </div>
                </div>
                <div style={{ display: "flex", gap: 8 }}>
                  <button
                    onClick={() => handleEditWithMap(addr)}
                    style={{
                      background: "none",
                      border: "none",
                      cursor: "pointer",
                      color: "#007bff",
                      fontSize: 18,
                    }}
                    title="ویرایش"
                  >
                    <FontAwesomeIcon icon="edit" />
                  </button>
                  <button
                    onClick={() => handleDelete(addr)}
                    style={{
                      background: "none",
                      border: "none",
                      cursor: "pointer",
                      color: "#dc3545",
                      fontSize: 18,
                    }}
                    title="حذف"
                  >
                    <FontAwesomeIcon icon="trash" />
                  </button>
                </div>
              </motion.div>
            ))}
          </AnimatePresence>
        </div>
      </div>
    </>
  );
}
