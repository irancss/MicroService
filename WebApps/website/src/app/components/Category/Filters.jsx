import React, { useState, useEffect } from "react";
import { useRouter, useSearchParams, usePathname } from 'next/navigation';

const dummyFilters = [
  {
    title: "فیلتر 1",
    key: "filter1",
    items: [
      { id: 1, name: "گزینه 1", checked: false },
      { id: 2, name: "گزینه 2", checked: false },
      { id: 3, name: "گزینه 3", checked: false },
    ],
  },
  {
    title: "فیلتر 2",
    key: "filter2",
    items: [
      { id: 4, name: "گزینه 4", checked: false },
      { id: 5, name: "گزینه 5", checked: false },
      { id: 6, name: "گزینه 6", checked: false },
    ],
  },
  {
    title: "فیلتر 3",
    key: "filter3",
    items: [
      { id: 7, name: "گزینه 7", checked: false },
      { id: 8, name: "گزینه 8", checked: false },
      { id: 9, name: "گزینه 9", checked: false },
    ],
  },
];

export default function FiltersProduct() {
  const [filters, setFilters] = useState([]);
  const [searchValues, setSearchValues] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isOpen, setIsOpen] = useState([]);

  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();

  useEffect(() => {
    async function fetchFilters() {
      try {
        const res = await fetch("/api/filters");
        if (!res.ok) throw new Error("Failed to fetch filters");
        const data = await res.json();
        setFilters(data);
        setSearchValues(Array(data.length).fill(""));
        setIsOpen(Array(data.length).fill(false));
      } catch (error) {
        setFilters(dummyFilters);
        setSearchValues(Array(dummyFilters.length).fill(""));
        setIsOpen(Array(dummyFilters.length).fill(false));
      } finally {
        setLoading(false);
      }
    }
    fetchFilters();
  }, []);

  useEffect(() => {
    if (filters.length === 0 || loading) return;
    const params = new URLSearchParams(searchParams.toString());
    filters.forEach(filter => params.delete(filter.key));
    filters.forEach(filter => {
      const selected = filter.items.filter(i => i.checked).map(i => i.name);
      if (selected.length) params.set(filter.key, selected.join(','));
    });
    router.push(`${pathname}?${params.toString()}`, { scroll: false });
  }, [filters, router, pathname, searchParams, loading]);

  const toggleFilterOpen = (index) => {
    setIsOpen(prevOpenState =>
      prevOpenState.map((state, i) => (i === index ? !state : state))
    );
  };

  const handleSearchChange = (index, value) => {
    const newSearchValues = [...searchValues];
    newSearchValues[index] = value;
    setSearchValues(newSearchValues);
  };

  const handleCheckboxChange = (filterIdx, itemId) => {
    setFilters((prevFilters) =>
      prevFilters.map((filter, idx) =>
        idx === filterIdx
          ? {
              ...filter,
              items: filter.items.map((item) =>
                item.id === itemId ? { ...item, checked: !item.checked } : item
              ),
            }
          : filter
      )
    );
  };

  const handleRemoveBadge = (filterIdx, itemId) => {
    handleCheckboxChange(filterIdx, itemId);
  };

  const handleClearFilter = (filterIdx) => {
    setFilters((prevFilters) =>
      prevFilters.map((filter, idx) =>
        idx === filterIdx
          ? {
              ...filter,
              items: filter.items.map((item) => ({ ...item, checked: false })),
            }
          : filter
      )
    );
    setSearchValues((prev) =>
      prev.map((v, idx) => (idx === filterIdx ? "" : v))
    );
  };

  if (loading)
    return (
      <div className="flex flex-col items-center justify-center py-10">
        {/* ... spinner ... */}
        <span className="text-gray-600 text-sm">در حال بارگذاری...</span>
      </div>
    );

  return (
    <div className="bg-white w-full max-w-md mx-auto">
      <div className="flex items-center justify-between mb-2 rounded-t-lg">
        <h4 className="font-bold text-md text-gray-800">فیلترها</h4>
        <button
          className="text-xs bg-amber-400 px-2 py-1 rounded-lg text-white"
          onClick={() => setFilters(dummyFilters)}
        >
          حذف فیلترها
        </button>
      </div>
      <hr className="border border-gray-300 mb-4" />
      <div className="flex flex-col gap-6">
        {filters.map((filter, index) => (
          <div key={index} className="flex flex-col gap-2 border-b border-gray-300 pb-4">
            <button
              className="flex items-center justify-between w-full font-bold text-lg text-gray-700 focus:outline-none"
              onClick={() => toggleFilterOpen(index)}
              type="button"
            >
              <span>{filter.title}</span>
              <svg
                className={`w-5 h-5 transition-transform duration-200 ${isOpen[index] ? "rotate-180" : ""}`}
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            {isOpen[index] && (
              <>
                <div className="flex items-center justify-between mt-2">
                  <span></span>
                  <button
                    className="text-xs text-red-500 hover:underline"
                    onClick={() => handleClearFilter(index)}
                  >
                    حذف همه
                  </button>
                </div>
                <div className="flex flex-wrap gap-2 mb-2">
                  {filter.items
                    .filter((item) => item.checked)
                    .map((item) => (
                      <span
                        key={item.id}
                        className="flex items-center bg-blue-100 text-blue-700 px-2 py-1 rounded-full text-xs font-medium"
                      >
                        {item.name}
                        <button
                          className="ml-1 text-blue-500 hover:text-red-500"
                          onClick={() => handleRemoveBadge(index, item.id)}
                          aria-label="حذف"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                </div>
                <input
                  type="text"
                  className="border border-gray-300 rounded p-2 focus:outline-none focus:ring-2 focus:ring-blue-200"
                  placeholder={`جستجوی ${filter.title}`}
                  value={searchValues[index]}
                  onChange={(e) => handleSearchChange(index, e.target.value)}
                />
                <div className="flex flex-col gap-1 max-h-40 overflow-y-auto pr-1">
                  {filter.items
                    .filter((item) =>
                      item.name
                        .toLowerCase()
                        .includes(searchValues[index].toLowerCase())
                    )
                    .map((item) => (
                      <label
                        key={item.id}
                        className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 rounded px-1 py-1"
                      >
                        <input
                          type="checkbox"
                          className="w-4 h-4 accent-blue-500"
                          checked={item.checked}
                          onChange={() => handleCheckboxChange(index, item.id)}
                        />
                        <span className="text-gray-700">{item.name}</span>
                      </label>
                    ))}
                </div>
              </>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
