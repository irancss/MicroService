"use client";
import { useState } from "react";
import Table from "@components/General/Table";

export default function OrderItems() {
  const items = [
    { id: 1, name: "محصول 1", quantity: 2, price: 100000 },
    { id: 2, name: "محصول 2", quantity: 1, price: 200000 },
    { id: 3, name: "محصول 3", quantity: 3, price: 150000 },
  ];

  const columns = [
    {
      key: "id",
      label: "شناسه",
    },
    {
      key: "name",
      label: "نام محصول",
    },
    {
      key: "quantity",
      label: "تعداد",
      render: (item) => (
        <input
          type="number"
          value={item.quantity}
          min={1}
          className="w-16 text-center border border-gray-300 rounded-md"
          onChange={(e) => {
            const newQuantity = Number(e.target.value);
            if (newQuantity >= 1) {
              setTableItems((prev) =>
                prev.map((i) =>
                  i.id === item.id ? { ...i, quantity: newQuantity } : i
                )
              );
            }
          }}
        />
      ),
    },
    {
      key: "price",
      label: "قیمت واحد",
      render: (item) => `${item.price.toLocaleString()} تومان`,
    },
    {
      key: "total",
      label: "قیمت کل",
      render: (item) =>
        `${(item.price * item.quantity).toLocaleString()} تومان`,
    },
  ];
  const shippingMethods = [
    { id: 1, name: "روش حمل و نقل 1", price: 50000 },
    { id: 2, name: "روش حمل و نقل 2", price: 70000 },
    { id: 3, name: "روش حمل و نقل 3", price: 90000 },
  ];
  const [tableItems, setTableItems] = useState(items);
  const [selectedShipping, setSelectedShipping] = useState(null);

  const products = [
    { id: 1, name: "محصول 1", price: 100000 },
    { id: 2, name: "محصول 2", price: 200000 },
    { id: 3, name: "محصول 3", price: 150000 },
  ];

  const handleAddShipping = () => {
    import("sweetalert2").then((Swal) => {
      Swal.default
        .fire({
          title: "انتخاب روش حمل و نقل",
          input: "select",
          inputOptions: Object.fromEntries(
            shippingMethods.map((method) => [
              method.id,
              `${method.name} (${method.price.toLocaleString()} تومان)`,
            ])
          ),
          inputPlaceholder: "یک روش حمل و نقل انتخاب کنید",
          showCancelButton: true,
          confirmButtonText: "تایید",
          cancelButtonText: "انصراف",
          inputValidator: (value) => {
            return new Promise((resolve) => {
              if (value) {
                resolve();
              } else {
                resolve("لطفا یک گزینه را انتخاب کنید");
              }
            });
          },
        })
        .then((result) => {
          if (result.isConfirmed) {
            const selectedMethod = shippingMethods.find(
              (method) => method.id === parseInt(result.value)
            );
            setSelectedShipping(selectedMethod);

            // Check if there's already a shipping row in the table
            const hasShippingItem = tableItems.some((item) =>
              String(item.id).startsWith("shipping-")
            );

            if (hasShippingItem) {
              // Update existing shipping row
              setTableItems((prevItems) =>
                prevItems.map((item) =>
                  String(item.id).startsWith("shipping-")
                    ? {
                        id: `shipping-${selectedMethod.id}`,
                        name: `حمل و نقل: ${selectedMethod.name}`,
                        quantity: 1,
                        price: selectedMethod.price,
                      }
                    : item
                )
              );
            } else {
              // Add shipping as a new row to the table
              setTableItems((prevItems) => [
                ...prevItems,
                {
                  id: `shipping-${selectedMethod.id}`,
                  name: `حمل و نقل: ${selectedMethod.name}`,
                  quantity: 1,
                  price: selectedMethod.price,
                },
              ]);
            }
          }
        });
    });
  };

  const handleAddProduct = () => {
    import("sweetalert2").then((Swal) => {
      Swal.default.fire({
        title: "افزودن محصول",
        html: `
                            <input id="swal-input-search" class="swal2-input" placeholder="جستجوی محصول...">
                            <div id="search-results" style="max-height: 200px; overflow-y: auto; margin-top: 10px;"></div>
                    `,
        showCancelButton: true,
        confirmButtonText: "افزودن",
        cancelButtonText: "انصراف",
        showConfirmButton: false,
        didOpen: () => {
          const searchInput = document.getElementById("swal-input-search");
          const resultsDiv = document.getElementById("search-results");

          searchInput.addEventListener("input", () => {
            const searchTerm = searchInput.value.toLowerCase();

            // Only search if at least 2 characters are typed
            if (searchTerm.length >= 2) {
              const filteredProducts = products.filter((p) =>
                p.name.toLowerCase().includes(searchTerm)
              );

              resultsDiv.innerHTML = filteredProducts
                .map(
                  (p) => `
                                            <div class="p-2 hover:bg-gray-100 cursor-pointer flex justify-between" 
                                                             data-id="${
                                                               p.id
                                                             }" data-name="${
                    p.name
                  }" data-price="${p.price}">
                                                    <span>${p.name}</span>
                                                    <span>${p.price.toLocaleString()} تومان</span>
                                            </div>
                                    `
                )
                .join("");

              document.querySelectorAll("#search-results div").forEach((el) => {
                el.addEventListener("click", () => {
                  const id = el.getAttribute("data-id");
                  const name = el.getAttribute("data-name");
                  const price = Number(el.getAttribute("data-price"));

                  // Add to table
                  setTableItems((prev) => {
                    const existingItem = prev.find(
                      (item) => item.id === Number(id)
                    );
                    if (existingItem) {
                      return prev.map((item) =>
                        item.id === Number(id)
                          ? { ...item, quantity: item.quantity + 1 }
                          : item
                      );
                    } else {
                      return [
                        ...prev,
                        { id: Number(id), name, quantity: 1, price },
                      ];
                    }
                  });
                  Swal.default.close();
                });
              });
            } else {
              // Clear results if search term is less than 2 characters
              resultsDiv.innerHTML = "";
            }
          });
        },
      });
    });
  };

  const handleAddDiscount = () => {
    import("sweetalert2").then((Swal) => {
      Swal.default
        .fire({
          title: "اعمال کد تخفیف",
          input: "text",
          inputPlaceholder: "کد تخفیف را وارد کنید",
          showCancelButton: true,
          confirmButtonText: "تایید",
          cancelButtonText: "انصراف",
          inputValidator: (value) => {
            if (!value) {
              return "لطفا کد تخفیف را وارد کنید";
            }
          },
        })
        .then((result) => {
          if (result.isConfirmed) {
            // Simulate discount validation
            // In a real app, this would be an API call
            const discountCode = result.value;
            const discountAmount = 50000; // Fixed discount for demo

            // Remove any existing discounts
            const itemsWithoutDiscount = tableItems.filter(
              (item) => !item.id.toString().startsWith("discount-")
            );

            // Add discount as a negative line item
            setTableItems([
              ...itemsWithoutDiscount,
              {
                id: `discount-${Date.now()}`,
                name: `تخفیف: ${discountCode}`,
                quantity: 1,
                price: -discountAmount,
              },
            ]);

            Swal.default.fire({
              icon: "success",
              title: "کد تخفیف اعمال شد",
              text: `${discountAmount.toLocaleString()} تومان تخفیف اعمال شد`,
            });
          }
        });
    });
  };

  return (
    <div className="w-2/3">
      <form className="flex flex-col gap-4">
        <Table
          columns={columns}
          data={tableItems}
          rowKey="id"
          pagination={false}
          className="w-full"
        />
        <div className="overflow-x-auto p-4  rounded-lg ">
          <div className=" p-4 rounded-lg shadow-sm w-full">
            <div className="text-lg font-bold text-blue-700 mb-2">
              خلاصه سفارش
            </div>
            <div className="space-y-2 border-t border-gray-300 pt-2">
              <div className="flex justify-between">
                <span>مجموع محصولات:</span>
                <span>
                  {tableItems
                    .filter(
                      (item) =>
                        !String(item.id).startsWith("shipping-") &&
                        !String(item.id).startsWith("discount-")
                    )
                    .reduce((sum, item) => sum + item.price * item.quantity, 0)
                    .toLocaleString()}{" "}
                  تومان
                </span>
              </div>

              {tableItems.some((item) =>
                String(item.id).startsWith("shipping-")
              ) && (
                <div className="flex justify-between">
                  <span>هزینه حمل و نقل:</span>
                  <span>
                    {tableItems
                      .filter((item) => String(item.id).startsWith("shipping-"))
                      .reduce(
                        (sum, item) => sum + item.price * item.quantity,
                        0
                      )
                      .toLocaleString()}{" "}
                    تومان
                  </span>
                </div>
              )}

              {tableItems.some((item) =>
                String(item.id).startsWith("discount-")
              ) && (
                <div className="flex justify-between text-green-600">
                  <span>تخفیف:</span>
                  <span>
                    {Math.abs(
                      tableItems
                        .filter((item) =>
                          String(item.id).startsWith("discount-")
                        )
                        .reduce(
                          (sum, item) => sum + item.price * item.quantity,
                          0
                        )
                    ).toLocaleString()}{" "}
                    تومان
                  </span>
                </div>
              )}

              <div className="flex justify-between font-bold text-lg border-t border-gray-300 pt-2 mt-2">
                <span>مجموع کل:</span>
                <span>
                  {tableItems
                    .reduce((sum, item) => sum + item.price * item.quantity, 0)
                    .toLocaleString()}{" "}
                  تومان
                </span>
              </div>
            </div>
          </div>
        </div>
        <div className="flex justify-end gap-3 mt-4">
          <button
            type="button"
            className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600 transition duration-200"
            onClick={handleAddProduct}
          >
            افزودن محصول
          </button>
          <button
            type="button"
            className="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600 transition duration-200"
            onClick={handleAddDiscount}
          >
            اعمال کد تخفیف
          </button>
          <button
            type="button"
            className="bg-purple-500 text-white px-4 py-2 rounded-md hover:bg-purple-600 transition duration-200"
            onClick={handleAddShipping}
          >
            انتخاب روش حمل و نقل
          </button>
        </div>
      </form>
    </div>
  );
}
