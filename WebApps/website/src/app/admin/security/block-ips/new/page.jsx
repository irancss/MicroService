import Input from "@components/General/Input";

export default function NewBlockIP() {
  const inputs = [
    {
      label: "آدرس IP",
      name: "ip",
      type: "text",
      placeholder: "آدرس IP را وارد کنید",
      required: true,
    },
    {
      label: "توضیحات",
      name: "description",
      type: "text",
      placeholder: "توضیحات را وارد کنید (اختیاری)",
    },
  ];

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">افزودن IP مسدود شده جدید</h1>
      <form className="space-y-4">
        {inputs.map((input) => (
          <Input
            key={input.name}
            label={input.label}
            name={input.name}
            type={input.type}
            placeholder={input.placeholder}
            required={input.required}
          />
        ))}
        <button
          type="submit"
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          افزودن IP مسدود شده
        </button>
      </form>
    </div>
  );
}
