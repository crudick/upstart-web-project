import { User } from '../types';

/**
 * Get display name for a user, falling back to email if first/last name are not available
 */
export const getUserDisplayName = (user: User | null | undefined): string => {
  if (!user) return '';
  
  // If both firstName and lastName are present and not empty
  if (user.firstName?.trim() && user.lastName?.trim()) {
    return `${user.firstName.trim()} ${user.lastName.trim()}`;
  }
  
  // If only firstName is present
  if (user.firstName?.trim()) {
    return user.firstName.trim();
  }
  
  // If only lastName is present
  if (user.lastName?.trim()) {
    return user.lastName.trim();
  }
  
  // Fall back to email
  return user.email || '';
};