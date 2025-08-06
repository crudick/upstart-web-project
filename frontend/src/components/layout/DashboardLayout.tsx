import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartPieIcon,
  ChartBarIcon,
  Bars3Icon,
  XMarkIcon,
  UserCircleIcon,
  ArrowRightOnRectangleIcon,
  PlusIcon,
  UsersIcon
} from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import { useAuth } from '../../contexts/AuthContext';

interface DashboardLayoutProps {
  children: React.ReactNode;
  activeTab: 'dashboard' | 'polls' | 'community';
  onTabChange: (tab: 'dashboard' | 'polls' | 'community') => void;
  onCreatePoll: () => void;
}

const DashboardLayout: React.FC<DashboardLayoutProps> = ({
  children,
  activeTab,
  onTabChange,
  onCreatePoll,
}) => {
  const { user, logout } = useAuth();
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const sidebarItems = [
    {
      id: 'dashboard' as const,
      label: 'Dashboard',
      icon: ChartBarIcon,
      description: 'Analytics & insights',
    },
    {
      id: 'polls' as const,
      label: 'My Polls',
      icon: ChartPieIcon,
      description: 'Manage your polls',
    },
    {
      id: 'community' as const,
      label: 'Community',
      icon: UsersIcon,
      description: 'Discover community polls',
    },
  ];

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="min-h-screen bg-background-light lg:flex">
      {/* Mobile sidebar backdrop */}
      {isSidebarOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={() => setIsSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <div className={`fixed inset-y-0 left-0 z-50 w-80 bg-white shadow-upstart-lg transform transition-transform lg:translate-x-0 lg:relative lg:flex lg:flex-shrink-0 ${
        isSidebarOpen ? 'translate-x-0' : '-translate-x-full'
      } lg:translate-x-0`}>
        <div className="flex flex-col h-full">
          {/* Sidebar Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div className="flex items-center space-x-3">
              <ChartPieIcon className="w-8 h-8 text-primary-600" />
              <span className="text-2xl font-bold text-gray-900 font-montserrat">
                UpPoll
              </span>
            </div>
            <button
              onClick={() => setIsSidebarOpen(false)}
              className="lg:hidden p-2 rounded-upstart hover:bg-gray-100"
            >
              <XMarkIcon className="w-6 h-6 text-gray-500" />
            </button>
          </div>

          {/* User Info */}
          <div className="p-6 border-b border-gray-200">
            <div className="flex items-center space-x-3">
              <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
                <UserCircleIcon className="w-6 h-6 text-primary-600" />
              </div>
              <div>
                <p className="font-semibold text-gray-900 font-montserrat">
                  {user?.firstName} {user?.lastName}
                </p>
                <p className="text-sm text-gray-500 font-montserrat">
                  {user?.email}
                </p>
              </div>
            </div>
          </div>

          {/* Navigation */}
          <nav className="flex-1 p-6">
            <div className="space-y-2">
              {sidebarItems.map((item) => (
                <button
                  key={item.id}
                  onClick={() => {
                    onTabChange(item.id);
                    setIsSidebarOpen(false);
                  }}
                  className={`w-full flex items-center space-x-3 px-4 py-3 rounded-upstart transition-all duration-200 ${
                    activeTab === item.id
                      ? 'bg-primary-50 text-primary-700 border border-primary-200'
                      : 'text-gray-700 hover:bg-gray-50'
                  }`}
                >
                  <item.icon className="w-5 h-5 flex-shrink-0" />
                  <div className="flex-1 text-left">
                    <p className="font-medium font-montserrat">{item.label}</p>
                    <p className="text-sm text-gray-500 font-montserrat">
                      {item.description}
                    </p>
                  </div>
                </button>
              ))}
            </div>

            <div className="mt-8">
              <Button
                onClick={onCreatePoll}
                variant="primary"
                className="w-full flex items-center justify-center space-x-2"
              >
                <PlusIcon className="w-5 h-5" />
                <span>Create New Poll</span>
              </Button>
            </div>
          </nav>

          {/* Logout Button */}
          <div className="p-6 border-t border-gray-200">
            <Button
              onClick={handleLogout}
              variant="ghost"
              className="w-full flex items-center justify-center space-x-2 text-gray-600 hover:text-red-600"
            >
              <ArrowRightOnRectangleIcon className="w-5 h-5" />
              <span>Sign Out</span>
            </Button>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 lg:flex lg:flex-col lg:min-w-0">
        {/* Mobile Header */}
        <header className="lg:hidden bg-white border-b border-gray-200 px-4 py-3">
          <div className="flex items-center justify-between">
            <button
              onClick={() => setIsSidebarOpen(true)}
              className="p-2 rounded-upstart hover:bg-gray-100"
            >
              <Bars3Icon className="w-6 h-6 text-gray-700" />
            </button>
            <div className="flex items-center space-x-2">
              <ChartPieIcon className="w-6 h-6 text-primary-600" />
              <span className="text-lg font-bold text-gray-900 font-montserrat">
                UpPoll
              </span>
            </div>
            <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
              <UserCircleIcon className="w-5 h-5 text-primary-600" />
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-auto">
          {children}
        </main>
      </div>
    </div>
  );
};

export default DashboardLayout;