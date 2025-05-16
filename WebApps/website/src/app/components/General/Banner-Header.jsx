export default function BannerHeader({children}) {
    return (
        <div className="bg-gray-400 flex flex-col items-center justify-center py-2">
            <div className="container mx-auto px-4  flex flex-col items-center justify-center">
                <h1 className="text-3xl font-bold text-white">عنوان بنر</h1>
            </div>
            {children}
        </div>
    )
}