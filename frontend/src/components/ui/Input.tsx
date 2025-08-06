import React from 'react';
import clsx from 'clsx';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
}

const Input: React.FC<InputProps> = ({
  label,
  error,
  helperText,
  leftIcon,
  rightIcon,
  className,
  ...props
}) => {
  return (
    <div className="w-full">
      {label && (
        <label className="block text-sm font-semibold text-gray-700 mb-1 font-montserrat">
          {label}
        </label>
      )}
      <div className="relative">
        {leftIcon && (
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <div className="text-gray-400">{leftIcon}</div>
          </div>
        )}
        <input
          className={clsx(
            'block w-full px-3 py-2 border border-gray-300 rounded-upstart font-montserrat',
            'placeholder-gray-400 text-gray-900',
            'focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500',
            'transition-all duration-200',
            {
              'pl-10': leftIcon,
              'pr-10': rightIcon,
              'border-red-300 focus:ring-red-500 focus:border-red-500': error,
            },
            className
          )}
          {...props}
        />
        {rightIcon && (
          <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
            <div className="text-gray-400">{rightIcon}</div>
          </div>
        )}
      </div>
      {error && (
        <p className="mt-1 text-sm text-red-600 font-montserrat">{error}</p>
      )}
      {helperText && !error && (
        <p className="mt-1 text-sm text-gray-500 font-montserrat">{helperText}</p>
      )}
    </div>
  );
};

export default Input;