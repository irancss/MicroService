"use client";
export default function Input({
  type = "text",
  label,
  placeholder,
  value,
  onChange,
  error,
  disabled = false,
  className,
  leftIcon,
  rightIcon,
  ...props
}) {
  return (
    <div className={`w-full ${className} mt-3`}>
      {label && (
        <label className="block text-sm font-medium text-gray-700 mb-1">
          {label}
        </label>
      )}

      <div className="relative">
        {leftIcon && (
          <div className="absolute left-3 top-1/2 -translate-y-1/2">
            {leftIcon}
          </div>
        )}

        <input
          type={type}
          value={value}
          onChange={onChange}
          placeholder={placeholder}
          disabled={disabled}
          className={`
            w-full px-4 py-2 border rounded-lg focus:outline-none
            transition-colors duration-200
            ${leftIcon ? "pl-10" : ""}
            ${rightIcon ? "pr-10" : ""}
            ${
              error
                ? "border-red-500 focus:border-red-500 focus:ring-red-500"
                : "border-gray-300 focus:border-blue-500 focus:ring-blue-500"
            }
            ${disabled ? "bg-gray-100 cursor-not-allowed" : "bg-white"}
          `}
          {...props}
        />

        {rightIcon && (
          <div className="absolute right-3 top-1/2 -translate-y-1/2">
            {rightIcon}
          </div>
        )}
      </div>

      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
    </div>
  );
}
