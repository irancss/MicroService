import { useState, useMemo } from "react";

const useTableFilter = (initialFilters, data) => {
  const [filters, setFilters] = useState(initialFilters);

  const handleFilterChange = (name, value) => {
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const filteredData = useMemo(() => {
    return data.filter((item) => {
      return Object.entries(filters).every(([key, filterValue]) => {
        if (!filterValue) return true; // Skip empty filters
        
        const itemValue = item[key];
        
        // Handle date range filters
        if (key === "startDate") {
          return new Date(item.blockDate) >= new Date(filterValue);
        }
        if (key === "endDate") {
          return new Date(item.blockDate) <= new Date(filterValue);
        }
        
        // Handle text filters
        if (typeof itemValue === "string") {
          return itemValue.includes(filterValue);
        }
        
        // Handle exact match filters
        return itemValue === filterValue;
      });
    });
  }, [data, filters]);

  return {
    filters,
    handleFilterChange,
    filteredData,
    resetFilters: () => setFilters(initialFilters)
  };
};

export default useTableFilter;