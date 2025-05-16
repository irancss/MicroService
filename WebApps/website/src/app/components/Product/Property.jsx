export default function Property({ children }) {
  const arrayProperty = [
    { id: 1, name: "رنگ", value: "قرمز" },
    { id: 2, name: "وزن", value: "1.5 کیلوگرم" },
    { id: 3, name: "ابعاد", value: "20x30x40 سانتی‌متر" },
    { id: 4, name: "جنس", value: "پلاستیک" },
    { id: 5, name: "گارانتی", value: "یک سال" },
  ];
  return (
    <>
      <table className="w-full text-sm text-start text-gray-500 dark:text-gray-400">
        <tbody>
          {arrayProperty.map((element) => (
            <tr
              key={element.id}
              className="bg-white border-b border-gray-300 dark:bg-gray-800 dark:border-gray-700 dark:hover:bg-gray-600"
            >
              <td className="px-6 py-4 font-medium text-gray-400 whitespace-nowrap dark:text-white">
                {element.name}
              </td>
              <td className="px-6 py-4 font-black text-center">
                {element.value}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
