import { useState } from "react";
import Input from "@components/General/Input";
import toast from "react-hot-toast";
import Select from "@components/General/Select";

export default function AddSchema() {
  const [schemaType, setSchemaType] = useState("");

  const handleSelectChange = (e) => {
    debugger;
    setSchemaType(e);
  };

  const schemaTypes = [
    { value: "", label: "انتخاب کنید" },
    { value: "Product", label: "محصول" },
    { value: "Article", label: "مقاله" },
    { value: "Event", label: "رویداد" },
    { value: "Organization", label: "سازمان" },
  ];

  // تعریف فیلدهای مختص به هر نوع اسکیما
  const schemaFields = {
    Product: [
      {
        label: "نام محصول",
        name: "name",
        type: "text",
        placeholder: "نام محصول را وارد کنید",
        required: true,
      },
      {
        label: "توضیحات",
        name: "description",
        type: "textarea",
        placeholder: "توضیحات محصول",
      },
      {
        label: "قیمت",
        name: "price",
        type: "number",
        placeholder: "قیمت محصول",
        required: true,
      },
      {
        label: "واحد پول",
        name: "currency",
        type: "text",
        placeholder: "مثال: IRR",
        required: true,
      },
      {
        label: "برند",
        name: "brand",
        type: "text",
        placeholder: "نام برند محصول",
      },
    ],
    Article: [
      {
        label: "عنوان مقاله",
        name: "headline",
        type: "text",
        placeholder: "عنوان مقاله",
        required: true,
      },
      {
        label: "نویسنده",
        name: "author",
        type: "text",
        placeholder: "نام نویسنده",
        required: true,
      },
      {
        label: "تاریخ انتشار",
        name: "datePublished",
        type: "date",
        required: true,
      },
      {
        label: "خلاصه مقاله",
        name: "description",
        type: "textarea",
        placeholder: "خلاصه مقاله",
      },
    ],
    Event: [
      {
        label: "نام رویداد",
        name: "name",
        type: "text",
        placeholder: "نام رویداد",
        required: true,
      },
      {
        label: "مکان",
        name: "location",
        type: "text",
        placeholder: "مکان برگزاری",
        required: true,
      },
      {
        label: "تاریخ شروع",
        name: "startDate",
        type: "datetime-local",
        required: true,
      },
      { label: "تاریخ پایان", name: "endDate", type: "datetime-local" },
      {
        label: "توضیحات",
        name: "description",
        type: "textarea",
        placeholder: "توضیحات رویداد",
      },
    ],
    Organization: [
      {
        label: "نام سازمان",
        name: "name",
        type: "text",
        placeholder: "نام سازمان",
        required: true,
      },
      {
        label: "آدرس وب‌سایت",
        name: "url",
        type: "url",
        placeholder: "آدرس وب‌سایت",
        required: true,
      },
      {
        label: "لوگو",
        name: "logo",
        type: "url",
        placeholder: "آدرس URL لوگو",
        required: true,
      },
      // سایر فیلدها (sameAs و contactPoint) اجباری نیستند
    ],
    LocalBusiness: [
      {
        label: "نام کسب‌وکار",
        name: "name",
        type: "text",
        placeholder: "نام کسب‌وکار",
        required: true,
      },
      {
        label: "آدرس",
        name: "address",
        type: "text",
        placeholder: "آدرس کسب‌وکار",
        required: true,
      },
      {
        label: "تلفن",
        name: "telephone",
        type: "tel",
        placeholder: "شماره تلفن",
        required: true,
      },
      {
        label: "ایمیل",
        name: "email",
        type: "email",
        placeholder: "آدرس ایمیل",
      },
      {
        label: "وب‌سایت",
        name: "url",
        type: "url",
        placeholder: "آدرس وب‌سایت",
      },
    ],
    Review: [
      {
        label: "نام بررسی",
        name: "name",
        type: "text",
        placeholder: "نام بررسی",
      },
      {
        label: "نویسنده",
        name: "author",
        type: "text",
        placeholder: "نام نویسنده",
        required: true,
      },
      {
        label: "تاریخ انتشار",
        name: "datePublished",
        type: "date",
        required: true,
      },
      {
        label: "امتیاز",
        name: "ratingValue",
        type: "number",
        placeholder: "امتیاز",
        required: true,
      },
      {
        label: "توضیحات",
        name: "description",
        type: "textarea",
        placeholder: "توضیحات بررسی",
      },
    ],
    Breadcrumbs: [
      {
        label: "عنوان",
        name: "title",
        type: "text",
        placeholder: "عنوان مسیر",
        required: true,
      },
      {
        label: "آدرس URL",
        name: "url",
        type: "url",
        placeholder: "آدرس URL مسیر",
        required: true,
      },
    ],
    Person: [
      {
        label: "نام",
        name: "name",
        type: "text",
        placeholder: "نام شخص",
        required: true,
      },
      {
        label: "عنوان شغلی",
        name: "jobTitle",
        type: "text",
        placeholder: "عنوان شغلی",
      },
      {
        label: "آدرس",
        name: "address",
        type: "text",
        placeholder: "آدرس محل سکونت",
      },
      {
        label: "تلفن",
        name: "telephone",
        type: "tel",
        placeholder: "شماره تلفن",
      },
      {
        label: "ایمیل",
        name: "email",
        type: "email",
        placeholder: "آدرس ایمیل",
      },
      {
        label: "آدرس وب‌ سایت",
        name: "url",
        type: "url",
        placeholder: "آدرس وب‌سایت",
      },
      { label: "تاریخ تولد", name: "birthDate", type: "date" },
    ],
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const data = Object.fromEntries(formData.entries());

    // ایجاد ساختار JSON Schema
    const schemaData = {
      "@context": "https://schema.org",
      "@type": schemaType,
      ...data,
    };

    toast.success("اسکیما با موفقیت اضافه شد!");
  };

  return (
    <div className="p-6 bg-white rounded-lg shadow-md">
      <h2 className="text-2xl font-bold mb-4">افزودن اسکیما جدید</h2>
      <form onSubmit={handleSubmit}>
        <Select
          label="نوع اسکیما"
          name="schemaType"
          options={schemaTypes}
          onChange={handleSelectChange}
          required
        />

        {schemaType && (
          <>
            <div className="mt-4 mb-2 p-2 bg-gray-50 rounded border border-gray-200">
              <h3 className="font-semibold text-gray-700">
                فیلدهای اسکیمای{" "}
                {schemaTypes.find((t) => t.value === schemaType)?.label}
              </h3>
            </div>

            {schemaFields[schemaType]?.map((field) => (
              <Input
                key={field.name}
                label={field.label}
                name={field.name}
                type={field.type}
                placeholder={field.placeholder}
                required={field.required}
              />
            ))}
          </>
        )}

        <button
          type="submit"
          className="mt-4 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          disabled={!schemaType}
        >
          افزودن اسکیما
        </button>
      </form>
    </div>
  );
}
