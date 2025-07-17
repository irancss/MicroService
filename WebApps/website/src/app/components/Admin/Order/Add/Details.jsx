"use client";
import Input from "@components/General/Input";
import Select from "@components/General/Select";

export default function DetailsOrder() {
  const inputs = [
    {
      id: 1,
      label: "نام مشتری",
      type: "text",
      placeholder: "نام مشتری را وارد کنید",
    },
    {
      id: 2,
      label: "تاریخ",
      type: "date",
      placeholder: "تاریخ را وارد کنید",
    },
  ];
  const selectOptions = [
    {
      label: "وضعیت",
      value: "status",
      options: [
        { value: "pending", label: "در انتظار" },
        { value: "processing", label: "در حال پردازش" },
      ],
    },
    {
      label: "روش پرداخت",
      value: "paymentMethod",
      options: [
        { value: "bankCard", label: "کارت بانکی" },
        { value: "cashOnDelivery", label: "در محل" },
        { value: "online", label: "آنلاین" },
      ],
    },
  ];

  return (
    <div className="w-1/3">
      <form className="flex flex-col gap-4">
        {inputs.map((input) => (
          <Input
            key={input.id}
            label={input.label}
            type={input.type}
            placeholder={input.placeholder}
          />
        ))}
        {selectOptions.map((option) => (
          <Select
            key={option.value}
            label={option.label}
            options={option.options}
            placeholder="انتخاب کنید"
          />
        ))}
        <div className="flex justify-end mt-4">
          <button
            type="button"
            onClick={async () => {
              const Swal = (await import("sweetalert2")).default;

              Swal.fire({
                title: "افزودن کاربر جدید",
                html: `
                            <div class="flex flex-col gap-4">
                                <div>
                                    <label class="block text-sm font-medium text-gray-700">نام</label>
                                    <input id="firstName" class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2" />
                                </div>
                                <div>
                                    <label class="block text-sm font-medium text-gray-700">نام خانوادگی</label>
                                    <input id="lastName" class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2" />
                                </div>
                                <div>
                                    <label class="block text-sm font-medium text-gray-700">شماره موبایل</label>
                                    <input id="mobile" class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2" />
                                </div>
                            </div>
                        `,
                showCancelButton: true,
                confirmButtonText: "افزودن",
                cancelButtonText: "لغو",
                preConfirm: () => {
                  const firstName = document.getElementById("firstName").value;
                  const lastName = document.getElementById("lastName").value;
                  const mobile = document.getElementById("mobile").value;

                  if (!firstName || !lastName || !mobile) {
                    Swal.showValidationMessage("لطفاً تمام فیلدها را پر کنید");
                    return false;
                  }

                  return { firstName, lastName, mobile };
                },
              }).then((result) => {
                if (result.isConfirmed) {

                  Swal.fire({
                    title: "موفقیت",
                    text: "کاربر جدید با موفقیت اضافه شد",
                    icon: "success",
                    confirmButtonText: "تأیید",
                  });
                }
              });
            }}
            className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors"
          >
            افزودن کاربر
          </button>
        </div>
      </form>
    </div>
  );
}
