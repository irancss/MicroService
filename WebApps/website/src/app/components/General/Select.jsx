"use client";
import { useState, useEffect, useRef } from "react";

export default function Select({
  options = [],
  value,
  onChange,
  label,
  placeholder = "انتخاب کنید...",
  isLoading = false,
  isSearchable = true,
  isClearable = false,
  error,
  className,
  disabled = false,
  onSearch,
}) {
  // Ensure options is always an array of objects with label/value
  const normalizedOptions = Array.isArray(options)
    ? options.filter(opt => typeof opt === "object" && opt !== null && "label" in opt && "value" in opt)
    : [];

  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const wrapperRef = useRef(null);

  // Handle click outside
  useEffect(() => {
    function handleClickOutside(event) {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [wrapperRef]);

  // Handle search debounce
  useEffect(() => {
    if (onSearch && isSearchable && isOpen) {
      // Only search if dropdown is open
      const timer = setTimeout(() => onSearch(searchTerm), 300);
      return () => clearTimeout(timer);
    }
  }, [searchTerm, onSearch, isSearchable, isOpen]);

  const filteredOptions = normalizedOptions.filter((option) =>
    option.label.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSelect = (selectedValue) => {
    if (onChange) {
      onChange(selectedValue);
    }
    setIsOpen(false);
    setSearchTerm("");
  };

  const handleClear = (e) => {
    e.stopPropagation(); // Prevent opening the dropdown
    if (onChange) {
      onChange(null); // Clear the selected value
    }
    setSearchTerm("");
  };

  const selectedOption = options.find((opt) => opt.value === value);
  const selectedLabel = selectedOption?.label;

  return (
    <div className={`relative ${className}`} ref={wrapperRef}>
      {label && (
        <label className="block text-sm font-medium text-gray-700 mb-1.5">
          {label}
        </label>
      )}

      <div className="relative">
        <div
          onClick={() => !disabled && setIsOpen(!isOpen)}
          className={`
                        w-full flex items-center justify-between px-3 py-2.5 border rounded-md shadow-sm
                        text-sm transition-colors duration-150 ease-in-out
                        ${
                          disabled
                            ? "bg-gray-100 text-gray-500 cursor-not-allowed"
                            : "bg-white text-gray-700 cursor-pointer hover:border-blue-500"
                        }
                        ${
                          error
                            ? "border-red-500 focus-within:border-red-500 focus-within:ring-red-500"
                            : "border-gray-300 focus-within:border-blue-500 focus-within:ring-blue-500"
                        }
                        ${isOpen ? "ring-2 ring-blue-500 border-blue-500" : ""}
                    `}
        >
          <span
            className={`truncate ${
              selectedLabel ? "text-gray-900" : "text-gray-400"
            }`}
          >
            {selectedLabel || placeholder}
          </span>

          <div className="flex items-center space-x-2 rtl:space-x-reverse">
            {isLoading && (
              <svg
                className="animate-spin h-5 w-5 text-gray-400"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                />
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                />
              </svg>
            )}

            {isClearable && value && !disabled && !isLoading && (
              <button
                type="button"
                onClick={handleClear}
                className="text-gray-400 hover:text-gray-600 focus:outline-none"
                aria-label="Clear selection"
              >
                <svg
                  className="h-4 w-4"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            )}

            {!isLoading && (
              <svg
                className={`h-5 w-5 text-gray-400 transition-transform duration-200
                                                                ${
                                                                  isOpen
                                                                    ? "transform rotate-180"
                                                                    : ""
                                                                }`}
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 20 20"
                fill="currentColor"
                aria-hidden="true"
              >
                <path
                  fillRule="evenodd"
                  d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                  clipRule="evenodd"
                />
              </svg>
            )}
          </div>
        </div>

        {isOpen && !disabled && (
          <div
            className="absolute z-20 w-full mt-1 bg-white border 
                                                 border-gray-200 rounded-md shadow-lg max-h-60 
                                                 overflow-y-auto focus:outline-none"
          >
            {isSearchable && (
              <div className="p-2 sticky top-0 bg-white z-10 border-b border-gray-200">
                <input
                  type="text"
                  placeholder="جستجو..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full px-3 py-2 text-sm border border-gray-300 rounded-md 
                                                        focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                  autoFocus
                />
              </div>
            )}

            <ul className="py-1">
              {isLoading && !filteredOptions.length && !searchTerm && (
                <li className="px-3 py-2 text-sm text-gray-500 text-center">
                  در حال بارگذاری گزینه‌ها...
                </li>
              )}
              {!isLoading && filteredOptions.length > 0
                ? filteredOptions.map((option) => (
                    <li
                      key={option.value}
                      onClick={() => handleSelect(option.value)}
                      className={`px-3 py-2 text-sm cursor-pointer select-none
                                                            ${
                                                              value ===
                                                              option.value
                                                                ? "bg-blue-500 text-white"
                                                                : "text-gray-900 hover:bg-blue-50 hover:text-blue-600"
                                                            }`}
                    >
                      {option.label}
                    </li>
                  ))
                : !isLoading && ( // Only show "No results" if not loading
                    <li className="px-3 py-2 text-sm text-gray-500 text-center">
                      {onSearch && searchTerm
                        ? "موردی یافت نشد"
                        : options.length === 0 && !onSearch
                        ? "گزینه‌ای موجود نیست"
                        : "موردی یافت نشد"}
                    </li>
                  )}
            </ul>
          </div>
        )}
      </div>

      {error && <p className="mt-1.5 text-sm text-red-600">{error}</p>}
    </div>
  );
}
