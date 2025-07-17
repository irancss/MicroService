"use client";
import Select from "@/src/app/components/General/Select";
import Input from "@components/General/Input";
import Swal from "sweetalert2";
export default function CreateTicketPage() {
  const inputs = [
    {
      label: "موضوع تیکت",
      type: "text",
      placeholder: "موضوع تیکت را وارد کنید",
      id: "subject",
    },
    {
      label: "دسته بندی",
      type: "select",
      options: [
        { label: "حساب کاربری", value: "account" },
        { label: "محصولات", value: "products" },
        { label: "مالی", value: "finance" },
      ],
      id: "category",
    },
    {
      label: "اولویت",
      type: "select",
      options: [
        { label: "عالی", value: "high" },
        { label: "متوسط", value: "medium" },
        { label: "پایین", value: "low" },
      ],
      id: "priority",
    },
  ];
  const handleClick = (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const data = Object.fromEntries(formData.entries());
      console.log(data);
      Swal.fire({
        title: "تیکت ایجاد شد",
        text: "تیکت شما با موفقیت ایجاد شد.",
        icon: "success",
        confirmButtonText: "باشه",
      });
      console.log("Form submitted");
  };
  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">ایجاد تیکت جدید</h1>
      <form className="space-y-4" onSubmit={handleClick} method="post">
        {inputs.map((input) => (
          <div key={input.id}>
            {input.type === "select" ? (
              <Select
                label={input.label}
                options={input.options}
                id={input.id}
                placeholder={input.placeholder}
              />
            ) : (
              <Input
                type={input.type}
                label={input.label}
                placeholder={input.placeholder}
                id={input.id}
              />
            )}
          </div>
        ))}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            متن تیکت
          </label>
          <textarea
            rows="4"
            className="bg-white  w-full px-4 py-2 border rounded-lg focus:outline-none
            transition-colors duration-200"
            placeholder="متن تیکت را وارد کنید"
          ></textarea>
        </div>
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
        >
          ارسال تیکت
        </button>
      </form>
    </div>
  );
}
