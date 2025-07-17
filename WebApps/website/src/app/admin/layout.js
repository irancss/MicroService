// app/admin/layout.js
import SidebarAdmin from "../components/Admin/Sidebar"; // مسیر را بررسی کنید که درست باشد

export default function AdminLayout({ children }) {
  // دیگر نیازی به تگ‌های html و body در اینجا نیست
  return (
    <div className="flex min-h-screen bg-gray-50 dark:bg-gray-900">
      {/* Sidebar */}
      <div className="sticky top-0 h-screen w-64 bg-white dark:bg-gray-800 shadow-lg border-r border-gray-200 dark:border-gray-700 transition-all duration-300">
        <div className="p-4">
          <SidebarAdmin />
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 p-8">
        <div className="w-full bg-gradient-to-br from-blue-50 via-white to-purple-50 dark:from-gray-800 dark:via-gray-900 dark:to-indigo-900 p-8 rounded-2xl shadow-xl border border-blue-100 dark:border-blue-900 transition-all duration-300">
          {children}
        </div>
      </div>
    </div>
  );
}
