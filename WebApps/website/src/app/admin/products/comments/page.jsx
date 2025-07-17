"use client";
import Table from "@components/General/Table";
import toast from "react-hot-toast";
import Swal from "sweetalert2";
export default function Comments() {
  //handle edit and delete actions
  const handleDelete = (id) => {
    toast.error(`حذف نظر با شناسه ${id}`);
  };
  //dummy data
  const comments = [
    {
      id: 1,
      text: "نظر اول",
      product: "محصول 1",
      user: "کاربر 1",
      createdAt: "2023-01-01",
    },
    {
      id: 2,
      text: "نظر دوم",
      product: "محصول 2",
      user: "کاربر 2",
      createdAt: "2023-01-02",
    },
    {
      id: 3,
      text: "نظر سوم",
      product: "محصول 3",
      user: "کاربر 3",
      createdAt: "2023-01-03",
    },
  ];
  // Import SweetAlert

  // Update handleEdit function to use SweetAlert
  const handleEdit = (id) => {
    const comment = comments.find((comment) => comment.id === id);

    Swal.fire({
      title: "ویرایش نظر",
      input: "text",
      inputValue: comment.text,
      inputAttributes: {
        dir: "rtl",
      },
      showCancelButton: true,
      confirmButtonText: "ذخیره",
      cancelButtonText: "انصراف",
      showLoaderOnConfirm: true,
      preConfirm: (newText) => {
        // Here you would make an API call to update the comment
        return new Promise((resolve) => {
          setTimeout(() => {
            resolve(newText);
          }, 500);
        });
      },
    }).then((result) => {
      if (result.isConfirmed) {
        toast.success(`نظر با شناسه ${id} با موفقیت ویرایش شد`);
        // Update local state or refetch data here
      }
    });
  };

  const columns = [
    { key: "text", label: "متن نظر" },
    { key: "product", label: "محصول" },
    { key: "user", label: "کاربر", render: (row) => row.user || "ناشناس" },
    {
      key: "createdAt",
      label: "تاریخ ایجاد",
      render: (row) => new Date(row.createdAt).toLocaleDateString("fa-IR"),
    },
    {
      key: "actions",
      label: "عملیات",
      render: (row) => (
        <div className="flex gap-2">
          <button
            className="text-blue-500 hover:text-blue-700"
            onClick={() => handleEdit(row.id)}
          >
            ویرایش
          </button>
          <button
            className="text-red-500 hover:text-red-700"
            onClick={() => handleDelete(row.id)}
          >
            حذف
          </button>
        </div>
      ),
    },
  ];
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">نظرات محصولات</h1>
      <Table data={comments} columns={columns} />
    </div>
  );
}
