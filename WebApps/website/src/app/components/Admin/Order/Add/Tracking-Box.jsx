import Input from "@components/General/Input";

export default function TrackingBox() {
  const inputs = [
    {
      id: 1,
      label: "کد رهگیری ",
      type: "text",
      placeholder: "کد رهگیری را وارد کنید",
    },
    {
      id: 2,
      label: "تاریخ ارسال ",
      type: "date",
      placeholder: "تاریخ را وارد کنید",
    },
  ];
  return (
    <div className="mb-4 w-1/3">
      {inputs.map((input) => (
        <div key={input.id} className="mb-4">
          <label
            htmlFor={input.id}
            className="block  text-lg font-medium text-gray-700"
          >
            {input.label}
          </label>
          <Input
            id={input.id}
            type={input.type}
            placeholder={input.placeholder}
            className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring-blue-500"
          />
        </div>
      ))}
    </div>
  );
}
