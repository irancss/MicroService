import React, { useEffect, useRef } from "react";
import Quill from "quill";
import "quill/dist/quill.snow.css";

const toolbarOptions = [
  [{ header: [1, 2, 3, 4, 5, 6, false] }],
  ["bold", "italic", "underline", "strike"],
  [{ color: [] }, { background: [] }],
  [{ script: "sub" }, { script: "super" }],
  [{ list: "ordered" }, { list: "bullet" }],
  [{ indent: "-1" }, { indent: "+1" }],
  [{ direction: "rtl" }], // فعال کردن راست‌به‌چپ
  [{ align: [] }],
  ["link", "image", "video"],
  ["clean"],
];

const QuillEditor = ({ value, onChange }) => {
  const editorRef = useRef(null);
  const quillRef = useRef(null);

  useEffect(() => {
    if (editorRef.current && !quillRef.current) {
      quillRef.current = new Quill(editorRef.current, {
        theme: "snow",
        modules: {
          toolbar: toolbarOptions,
        },
        placeholder: "متن خود را وارد کنید...",
      });

      // تنظیم جهت پیش‌فرض به راست‌به‌چپ
      quillRef.current.format("direction", "rtl");
      quillRef.current.format("align", "right");

      // تنظیم فونت فارسی (در صورت نیاز فونت را در css اضافه کنید)
      quillRef.current.root.style.fontFamily =
        "Tahoma, Vazirmatn, Arial, sans-serif";
      quillRef.current.root.style.direction = "rtl";
      quillRef.current.root.style.textAlign = "right";

      if (value) {
        quillRef.current.root.innerHTML = value;
      }

      quillRef.current.on("text-change", () => {
        if (onChange) {
          onChange(quillRef.current.root.innerHTML);
        }
      });
    }
  }, [value, onChange]);

  return (
    <div>
      <div
        ref={editorRef}
        style={{
          height: 300,
          direction: "rtl",
          textAlign: "right",
          fontFamily: "Tahoma, Vazirmatn, Arial, sans-serif",
        }}
      />
    </div>
  );
};

export default QuillEditor;
