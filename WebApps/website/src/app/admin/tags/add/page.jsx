"use client";
import QuillEditor from "@components/General/Quill";
import Input from "@components/General/Input";

export default function AddTag() {
  const inputs = [
    {
      label: "نام تگ",
      name: "name",
      type: "text",
      placeholder: "نام تگ را وارد کنید",
      required: true,
    },
    {
      label: "آدرس",
      name: "url",
      type: "text",
      placeholder: "آدرس تگ را وارد کنید",
    },
  ];
  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">افزودن تگ جدید</h1>
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
        <QuillEditor />
        <button
          type="submit"
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          افزودن تگ
        </button>
      </form>
    </div>
  );
}
