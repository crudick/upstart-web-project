import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartPieIcon, 
  PlusIcon,
  ArrowRightIcon 
} from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import PollCard from '../poll/PollCard';
import LoginForm from '../auth/LoginForm';
import RegisterForm from '../auth/RegisterForm';
import { Poll } from '../../types';
import { pollsAPI } from '../../services/api';

interface PublicLandingPageProps {
  onViewPoll: (pollGuid: string) => void;
}

const PublicLandingPage: React.FC<PublicLandingPageProps> = ({ onViewPoll }) => {
  const [polls, setPolls] = useState<Poll[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showAuth, setShowAuth] = useState<'login' | 'register' | null>(null);

  useEffect(() => {
    const fetchPublicPolls = async () => {
      try {
        const publicPolls = await pollsAPI.getPublicPolls();
        setPolls(publicPolls.slice(0, 6)); // Show first 6 polls
      } catch (error) {
        console.error('Failed to fetch public polls:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchPublicPolls();
  }, []);


  if (showAuth) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-background-light to-white flex items-center justify-center p-4">
        <div className="w-full max-w-md">
          {showAuth === 'login' ? (
            <LoginForm onSwitchToRegister={() => setShowAuth('register')} />
          ) : (
            <RegisterForm onSwitchToLogin={() => setShowAuth('login')} />
          )}
          <div className="text-center mt-4">
            <Button
              variant="ghost"
              onClick={() => setShowAuth(null)}
              className="text-gray-600 hover:text-gray-800"
            >
              ← Back to home
            </Button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-background-light to-white">
      {/* Navigation */}
      <nav className="bg-white/80 backdrop-blur-md border-b border-gray-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-2">
              <ChartPieIcon className="w-8 h-8 text-primary-600" />
              <span className="text-2xl font-bold text-gray-900 font-montserrat">
                UpPoll
              </span>
            </div>
            <div className="flex items-center space-x-3">
              <Button
                variant="ghost"
                onClick={() => setShowAuth('login')}
              >
                Sign In
              </Button>
              <Button
                variant="primary"
                onClick={() => setShowAuth('register')}
              >
                Get Started
              </Button>
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="py-20 px-4 sm:px-6 lg:px-8">
        <div className="max-w-4xl mx-auto text-center">
          <motion.div
            initial={{ opacity: 0, y: 30 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
          >
            <h1 className="text-5xl md:text-6xl font-bold text-gray-900 font-montserrat mb-6">
              Create Amazing
              <span className="text-primary-600 block">Polls & Surveys</span>
            </h1>
            <p className="text-xl text-gray-600 font-montserrat mb-8 max-w-2xl mx-auto">
              Engage your audience, gather insights, and make data-driven decisions
              with our modern polling platform.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button
                variant="primary"
                size="lg"
                onClick={() => setShowAuth('register')}
                className="flex items-center gap-2"
              >
                <PlusIcon className="w-5 h-5" />
                Create Your First Poll
              </Button>
              <Button
                variant="outline"
                size="lg"
                onClick={() => setShowAuth('login')}
                className="flex items-center gap-2"
              >
                Sign In
                <ArrowRightIcon className="w-5 h-5" />
              </Button>
            </div>
          </motion.div>
        </div>
      </section>


      {/* Recent Polls Section */}
      <section className="py-16 px-4 sm:px-6 lg:px-8">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 font-montserrat mb-4">
              Recent Public Polls
            </h2>
            <p className="text-gray-600 font-montserrat text-lg">
              See what others are asking about
            </p>
          </div>

          {isLoading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {Array.from({ length: 6 }).map((_, index) => (
                <div key={index} className="h-64 bg-gray-200 rounded-upstart animate-pulse"></div>
              ))}
            </div>
          ) : polls.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {polls.map((poll) => (
                <motion.div
                  key={poll.id}
                  initial={{ opacity: 0, scale: 0.95 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ duration: 0.3 }}
                >
                  <PollCard poll={poll} onViewPoll={onViewPoll} />
                </motion.div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12">
              <ChartPieIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-600 font-montserrat">
                No public polls available at the moment.
              </p>
            </div>
          )}
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 bg-primary-600">
        <div className="max-w-4xl mx-auto text-center">
          <h2 className="text-3xl font-bold text-white font-montserrat mb-4">
            Ready to Get Started?
          </h2>
          <p className="text-primary-100 font-montserrat text-lg mb-8">
            Join thousands of users who trust UpPoll for their polling needs
          </p>
          <Button
            variant="secondary"
            size="lg"
            onClick={() => setShowAuth('register')}
            className="bg-white text-primary-600 hover:bg-gray-50"
          >
            Create Free Account
          </Button>
        </div>
      </section>
    </div>
  );
};

export default PublicLandingPage;