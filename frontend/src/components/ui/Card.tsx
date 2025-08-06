import React from 'react';
import { motion } from 'framer-motion';
import clsx from 'clsx';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  hover?: boolean;
  padding?: 'none' | 'sm' | 'md' | 'lg';
  onClick?: () => void;
}

const Card: React.FC<CardProps> = ({
  children,
  className,
  hover = false,
  padding = 'md',
  onClick,
}) => {
  const paddingClasses = {
    none: '',
    sm: 'p-4',
    md: 'p-6',
    lg: 'p-8',
  };

  const Component = onClick ? motion.div : 'div';
  const motionProps = onClick ? {
    whileHover: { y: -2, scale: 1.01 },
    whileTap: { scale: 0.99 },
  } : {};

  return (
    <Component
      className={clsx(
        'bg-background-card rounded-upstart shadow-upstart border border-gray-100',
        paddingClasses[padding],
        {
          'cursor-pointer': onClick,
          'hover:shadow-upstart-lg transition-all duration-200': hover || onClick,
        },
        className
      )}
      onClick={onClick}
      {...motionProps}
    >
      {children}
    </Component>
  );
};

export default Card;