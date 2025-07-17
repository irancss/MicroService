import React, { useState } from "react";
import FontAwesomeIcon from "@components/General/FontAwesomeIcon";
import AnimatedDiv from "../../Animated/Div";

const TableFilter = ({ filtersConfig, filters, onFilterChange }) => {
  const [showFilters, setShowFilters] = useState(true);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    onFilterChange(name, value);
  };

  const toggleFilters = () => {
    setShowFilters(!showFilters);
  };

  return (
    <div className="mb-4 p-4">
      <button
        onClick={toggleFilters}
        className="mb-3 flex items-center px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-300 transition duration-150 ease-in-out"
      >
        {showFilters ? (
          <FontAwesomeIcon icon="eye-slash" className="mr-2" />
        ) : (
          <FontAwesomeIcon icon="eye" className="mr-2" />
        )}
        {showFilters ? "مخفی کردن فیلترها" : "نمایش فیلترها"}
        <FontAwesomeIcon icon="filter" className="ml-2" />
      </button>

      {showFilters && (
        <AnimatedDiv
          initial={{ opacity: 0, height: 0 }}
          animate={{ opacity: 1, height: "auto" }}
          exit={{ opacity: 0, height: 0 }}
          transition={{ duration: 0.3, ease: "easeInOut" }} // Use framer-motion's transition
          style={{ overflow: "hidden" }} // overflow: hidden is important for height animation
        >
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 items-end">
            {filtersConfig.map((filter) => {
              if (filter.type === "text") {
                return (
                  <div key={filter.name} className="flex flex-col">
                    <label
                      htmlFor={filter.name}
                      className="text-sm font-medium text-gray-700 mb-1"
                    >
                      {filter.label}
                    </label>
                    <input
                      type="text"
                      id={filter.name}
                      name={filter.name}
                      placeholder={
                        filter.placeholder || `جستجوی ${filter.label}`
                      }
                      value={filters[filter.name] || ""}
                      onChange={handleInputChange}
                      className="border border-gray-300 p-2 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 w-full text-sm"
                    />
                  </div>
                );
              } else if (filter.type === "date") {
                return (
                  <div key={filter.name} className="flex flex-col">
                    <label
                      htmlFor={filter.name}
                      className="text-sm font-medium text-gray-700 mb-1"
                    >
                      {filter.label}
                    </label>
                    <input
                      type="date"
                      id={filter.name}
                      name={filter.name}
                      value={filters[filter.name] || ""}
                      onChange={handleInputChange}
                      className="border border-gray-300 p-2 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 w-full text-sm"
                    />
                  </div>
                );
              } else if (filter.type === "select") {
                return (
                  <div key={filter.name} className="flex flex-col">
                    <label
                      htmlFor={filter.name}
                      className="text-sm font-medium text-gray-700 mb-1"
                    >
                      {filter.label}
                    </label>
                    <select
                      id={filter.name}
                      name={filter.name}
                      value={filters[filter.name] || ""}
                      onChange={handleInputChange}
                      className="border border-gray-300 p-2 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 w-full text-sm bg-white"
                    >
                      <option value="">همه {filter.label}</option>
                      {filter.options.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                  </div>
                );
              }
              return null;
            })}
          </div>
        </AnimatedDiv>
      )}
    </div>
  );
};

export default TableFilter;
