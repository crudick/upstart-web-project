import { getUserDisplayName } from '../userDisplay';
import { User } from '../../types';

describe('getUserDisplayName', () => {
  it('should return full name when both firstName and lastName are provided', () => {
    const user: User = {
      id: 1,
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('John Doe');
  });

  it('should return email when firstName and lastName are null', () => {
    const user: User = {
      id: 1,
      firstName: null,
      lastName: null,
      email: 'john.doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('john.doe@example.com');
  });

  it('should return email when firstName and lastName are empty strings', () => {
    const user: User = {
      id: 1,
      firstName: '',
      lastName: '',
      email: 'john.doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('john.doe@example.com');
  });

  it('should return email when firstName and lastName are whitespace only', () => {
    const user: User = {
      id: 1,
      firstName: '   ',
      lastName: '  ',
      email: 'john.doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('john.doe@example.com');
  });

  it('should return firstName when only firstName is provided', () => {
    const user: User = {
      id: 1,
      firstName: 'John',
      lastName: null,
      email: 'john@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('John');
  });

  it('should return lastName when only lastName is provided', () => {
    const user: User = {
      id: 1,
      firstName: null,
      lastName: 'Doe',
      email: 'doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('Doe');
  });

  it('should return empty string when user is null', () => {
    expect(getUserDisplayName(null)).toBe('');
  });

  it('should return empty string when user is undefined', () => {
    expect(getUserDisplayName(undefined)).toBe('');
  });

  it('should trim whitespace from names', () => {
    const user: User = {
      id: 1,
      firstName: '  John  ',
      lastName: '  Doe  ',
      email: 'john.doe@example.com',
      createdAt: '2023-01-01',
      updatedAt: '2023-01-01'
    };
    
    expect(getUserDisplayName(user)).toBe('John Doe');
  });
});