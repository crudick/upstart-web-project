import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { UserIcon } from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import Input from '../ui/Input';
import Card from '../ui/Card';
import { useAuth } from '../../contexts/AuthContext';
import { api } from '../../services/api';

const ProfilePage: React.FC = () => {
  const { user, refreshUser } = useAuth();
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
  });
  const [isLoading, setIsLoading] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');
    setSuccess('');

    try {
      await api.updateProfile({
        firstName: formData.firstName.trim() || undefined,
        lastName: formData.lastName.trim() || undefined,
      });
      
      setSuccess('Profile updated successfully!');
      await refreshUser();
    } catch (err: any) {
      setError(err.message || 'Failed to update profile. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3 }}
      >
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 font-montserrat mb-2">
            Profile Settings
          </h1>
          <p className="text-gray-600 font-montserrat">
            Manage your profile information and preferences
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Profile Info Card */}
          <div className="lg:col-span-1">
            <Card>
              <div className="text-center">
                <div className="w-20 h-20 bg-primary-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <UserIcon className="w-10 h-10 text-primary-600" />
                </div>
                <h3 className="font-semibold text-gray-900 font-montserrat mb-1">
                  {user?.firstName && user?.lastName 
                    ? `${user.firstName} ${user.lastName}`
                    : 'Complete your profile'
                  }
                </h3>
                <p className="text-sm text-gray-500 font-montserrat mb-4">
                  {user?.email}
                </p>
                <div className="text-xs text-gray-400 font-montserrat">
                  Member since {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : ''}
                </div>
              </div>
            </Card>
          </div>

          {/* Profile Form */}
          <div className="lg:col-span-2">
            <Card>
              <div className="mb-6">
                <h2 className="text-xl font-semibold text-gray-900 font-montserrat mb-2">
                  Personal Information
                </h2>
                <p className="text-gray-600 font-montserrat text-sm">
                  Update your personal details. These fields are optional.
                </p>
              </div>

              {success && (
                <motion.div
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: 'auto' }}
                  className="mb-4 p-3 bg-green-50 border border-green-200 rounded-upstart"
                >
                  <p className="text-sm text-green-600 font-montserrat">{success}</p>
                </motion.div>
              )}

              {error && (
                <motion.div
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: 'auto' }}
                  className="mb-4 p-3 bg-red-50 border border-red-200 rounded-upstart"
                >
                  <p className="text-sm text-red-600 font-montserrat">{error}</p>
                </motion.div>
              )}

              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <Input
                    type="text"
                    label="First Name"
                    placeholder="Enter your first name"
                    leftIcon={<UserIcon className="w-5 h-5" />}
                    value={formData.firstName}
                    onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                    helperText="Optional"
                  />
                  <Input
                    type="text"
                    label="Last Name"
                    placeholder="Enter your last name"
                    value={formData.lastName}
                    onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                    helperText="Optional"
                  />
                </div>

                <div className="pt-4">
                  <Button
                    type="submit"
                    variant="primary"
                    isLoading={isLoading}
                    className="w-full md:w-auto"
                  >
                    Save Changes
                  </Button>
                </div>
              </form>
            </Card>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default ProfilePage;