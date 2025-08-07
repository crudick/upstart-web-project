import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { EyeIcon, EyeSlashIcon, EnvelopeIcon } from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import Input from '../ui/Input';
import Card from '../ui/Card';
import { useAuth } from '../../contexts/AuthContext';
import { RegisterRequest } from '../../types';

interface RegisterFormProps {
  onSwitchToLogin: () => void;
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onSwitchToLogin }) => {
  const { register } = useAuth();
  const [formData, setFormData] = useState<RegisterRequest>({
    email: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    // Basic validation
    if (formData.password.length < 8) {
      setError('Password must be at least 8 characters long');
      setIsLoading(false);
      return;
    }

    try {
      await register(formData);
    } catch (err: any) {
      setError(err.message || 'Registration failed. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
    >
      <Card className="w-full max-w-md mx-auto">
        <div className="text-center mb-6">
          <h2 className="text-2xl font-bold text-gray-900 font-montserrat mb-2">
            Create your account
          </h2>
          <p className="text-gray-600 font-montserrat">
            Join UpPoll to start creating and sharing polls
          </p>
        </div>

        {error && (
          <motion.div
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: 'auto' }}
            className="mb-4 p-3 bg-red-50 border border-red-200 rounded-upstart"
          >
            <p className="text-sm text-red-600 font-montserrat">{error}</p>
          </motion.div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            type="email"
            label="Email"
            placeholder="john@example.com"
            leftIcon={<EnvelopeIcon className="w-5 h-5" />}
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            required
          />

          <Input
            type={showPassword ? 'text' : 'password'}
            label="Password"
            placeholder="Create a strong password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            rightIcon={
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="p-1 hover:bg-gray-100 rounded"
              >
                {showPassword ? (
                  <EyeSlashIcon className="w-5 h-5" />
                ) : (
                  <EyeIcon className="w-5 h-5" />
                )}
              </button>
            }
            helperText="Minimum 8 characters"
            required
          />

          <Button
            type="submit"
            variant="primary"
            size="lg"
            isLoading={isLoading}
            className="w-full"
          >
            Create Account
          </Button>
        </form>

        <div className="mt-6 text-center">
          <p className="text-gray-600 font-montserrat">
            Already have an account?{' '}
            <button
              onClick={onSwitchToLogin}
              className="text-primary-600 hover:text-primary-700 font-semibold transition-colors"
            >
              Sign in
            </button>
          </p>
        </div>
      </Card>
    </motion.div>
  );
};

export default RegisterForm;