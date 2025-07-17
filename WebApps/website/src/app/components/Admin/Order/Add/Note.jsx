export default function Note() {
  return (
      <div className="p-4  rounded-lg  w-1/3">
      <label htmlFor="note" className="block text-xl font-medium text-gray-700">
        یادداشت
      </label>
      <textarea
        id="note"
        rows="6"
        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring-blue-500"
        placeholder="یادداشت خود را اینجا وارد کنید..."
      />
    </div>
  );
}
