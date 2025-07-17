
export default function TagPage({ params }) {
  return (
    <div className="mx-auto max-w-[100vw] md:px-3 px-2 my-5 overflow-x-hidden">
      <h1 className="text-2xl font-bold mb-4">برچسب: {params.id}</h1>
      <p>این صفحه مربوط به برچسب {params.id} است.</p>
      {/* اینجا می‌توانید محتوای مربوط به برچسب را اضافه کنید */}
    </div>
  );
}