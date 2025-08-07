import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartPieIcon, 
  PlusIcon,
  ArrowRightIcon,
  AdjustmentsHorizontalIcon,
  ChevronDownIcon
} from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import PollCard from '../poll/PollCard';
import LoginForm from '../auth/LoginForm';
import RegisterForm from '../auth/RegisterForm';
import PollCreationForm from '../PollCreationForm';
import { Poll } from '../../types';
import { pollsAPI } from '../../services/api';

interface PublicLandingPageProps {
  onViewPoll: (pollGuid: string) => void;
}

type SortOption = 'newest' | 'votes' | 'expiring';

const PublicLandingPage: React.FC<PublicLandingPageProps> = ({ onViewPoll }) => {
  const [polls, setPolls] = useState<Poll[]>([]);
  const [allPolls, setAllPolls] = useState<Poll[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showAuth, setShowAuth] = useState<'login' | 'register' | null>(null);
  const [showCreatePoll, setShowCreatePoll] = useState(false);
  const [showAllPolls, setShowAllPolls] = useState(false);
  const [sortBy, setSortBy] = useState<SortOption>('newest');
  const [showSortDropdown, setShowSortDropdown] = useState(false);

  useEffect(() => {
    const fetchPublicPolls = async () => {
      try {
        const publicPolls = await pollsAPI.getPublicPolls();
        const sortedPolls = sortPolls(publicPolls, sortBy);
        setAllPolls(publicPolls);
        setPolls(sortedPolls.slice(0, 6)); // Show first 6 polls initially
      } catch (error) {
        console.error('Failed to fetch public polls:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchPublicPolls();
  }, [sortBy]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (showSortDropdown) {
        const target = event.target as Element;
        if (!target.closest('.sort-dropdown')) {
          setShowSortDropdown(false);
        }
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [showSortDropdown]);

  const handlePollCreated = () => {
    // Refresh polls after creation
    setPolls([]);
    setIsLoading(true);
    const fetchPublicPolls = async () => {
      try {
        const publicPolls = await pollsAPI.getPublicPolls();
        const sortedPolls = sortPolls(publicPolls, sortBy);
        setAllPolls(publicPolls);
        setPolls(showAllPolls ? sortedPolls : sortedPolls.slice(0, 6));
      } catch (error) {
        console.error('Failed to fetch public polls:', error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchPublicPolls();
  };

  const sortPolls = (pollsToSort: Poll[], sortOption: SortOption): Poll[] => {
    const sorted = [...pollsToSort];
    
    switch (sortOption) {
      case 'votes':
        return sorted.sort((a, b) => {
          const aVotes = a.stats?.length || 0;
          const bVotes = b.stats?.length || 0;
          return bVotes - aVotes; // Most votes first
        });
      case 'expiring':
        return sorted.sort((a, b) => {
          // Polls without expiry go to the end
          if (!a.expiresAt && !b.expiresAt) return 0;
          if (!a.expiresAt) return 1;
          if (!b.expiresAt) return -1;
          
          const aDate = new Date(a.expiresAt);
          const bDate = new Date(b.expiresAt);
          return aDate.getTime() - bDate.getTime(); // Expiring soonest first
        });
      case 'newest':
      default:
        return sorted.sort((a, b) => {
          const aDate = new Date(a.createdAt);
          const bDate = new Date(b.createdAt);
          return bDate.getTime() - aDate.getTime(); // Newest first
        });
    }
  };

  const applySort = (sortOption: SortOption) => {
    const sortedPolls = sortPolls(allPolls, sortOption);
    setPolls(showAllPolls ? sortedPolls : sortedPolls.slice(0, 6));
    setSortBy(sortOption);
    setShowSortDropdown(false);
  };

  const toggleShowAllPolls = () => {
    const newShowAll = !showAllPolls;
    setShowAllPolls(newShowAll);
    
    const sortedPolls = sortPolls(allPolls, sortBy);
    setPolls(newShowAll ? sortedPolls : sortedPolls.slice(0, 6));
  };


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
                onClick={() => setShowCreatePoll(true)}
                className="flex items-center gap-2"
              >
                <PlusIcon className="w-5 h-5" />
                Create Your First Poll
              </Button>
              <Button
                variant="outline"
                size="lg"
                onClick={() => setShowAuth('register')}
                className="flex items-center gap-2"
              >
                Sign Up for More Features
                <ArrowRightIcon className="w-5 h-5" />
              </Button>
            </div>
          </motion.div>
        </div>
      </section>


      {/* Recent Polls Section */}
      <section className="py-20 px-4 sm:px-6 lg:px-8 bg-gray-50 border-t border-gray-100">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-16">
            <div className="inline-flex items-center justify-center w-12 h-12 bg-primary-100 rounded-xl mb-6">
              <ChartPieIcon className="w-6 h-6 text-primary-600" />
            </div>
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 font-montserrat mb-4">
              {showAllPolls ? 'All Public Polls' : 'Recent Public Polls'}
            </h2>
            <p className="text-gray-600 font-montserrat text-lg max-w-2xl mx-auto mb-8">
              See what others are asking about and join the conversation
            </p>
            
            {/* Sort Options */}
            <div className="flex justify-center">
              <div className="relative sort-dropdown">
                <button
                  onClick={() => setShowSortDropdown(!showSortDropdown)}
                  className="inline-flex items-center gap-2 px-4 py-2 bg-white border border-gray-200 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors"
                >
                  <AdjustmentsHorizontalIcon className="w-4 h-4" />
                  Sort by: {
                    sortBy === 'newest' ? 'Newest' :
                    sortBy === 'votes' ? 'Most Votes' :
                    'Expiring Soon'
                  }
                  <ChevronDownIcon className={`w-4 h-4 transition-transform ${showSortDropdown ? 'rotate-180' : ''}`} />
                </button>
                
                {showSortDropdown && (
                  <div className="absolute top-full mt-2 left-1/2 transform -translate-x-1/2 w-48 bg-white border border-gray-200 rounded-lg shadow-lg z-10">
                    <div className="py-2">
                      <button
                        onClick={() => applySort('newest')}
                        className={`w-full text-left px-4 py-2 text-sm hover:bg-gray-50 ${sortBy === 'newest' ? 'text-primary-600 bg-primary-50' : 'text-gray-700'}`}
                      >
                        Newest First
                      </button>
                      <button
                        onClick={() => applySort('votes')}
                        className={`w-full text-left px-4 py-2 text-sm hover:bg-gray-50 ${sortBy === 'votes' ? 'text-primary-600 bg-primary-50' : 'text-gray-700'}`}
                      >
                        Most Votes
                      </button>
                      <button
                        onClick={() => applySort('expiring')}
                        className={`w-full text-left px-4 py-2 text-sm hover:bg-gray-50 ${sortBy === 'expiring' ? 'text-primary-600 bg-primary-50' : 'text-gray-700'}`}
                      >
                        Expiring Soon
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>

          {isLoading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {Array.from({ length: 6 }).map((_, index) => (
                <div key={index} className="h-64 bg-white rounded-upstart shadow-sm border border-gray-100 animate-pulse"></div>
              ))}
            </div>
          ) : polls.length > 0 ? (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
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
              
              {allPolls.length > 6 && (
                <div className="text-center mt-8">
                  <Button
                    variant="outline"
                    onClick={toggleShowAllPolls}
                    className="flex items-center gap-2 mx-auto"
                  >
                    {showAllPolls ? (
                      <>Show Less</>
                    ) : (
                      <>
                        View All {allPolls.length} Polls
                        <ArrowRightIcon className="w-4 h-4" />
                      </>
                    )}
                  </Button>
                </div>
              )}
            </>
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
      <section className="py-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-r from-primary-600 to-primary-700 relative overflow-hidden">
        <div className="absolute inset-0 bg-black/10"></div>
        <div className="max-w-4xl mx-auto text-center relative z-10">
          <h2 className="text-3xl font-bold text-white font-montserrat mb-4">
            Ready to Get Started?
          </h2>
          <p className="text-primary-100 font-montserrat text-lg mb-8">
            Join users who trust UpPoll for their polling needs
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

      {/* Poll Creation Modal */}
      {showCreatePoll && (
        <PollCreationForm
          onClose={() => setShowCreatePoll(false)}
          onPollCreated={() => {
            setShowCreatePoll(false);
            handlePollCreated();
            // Show a message encouraging registration
            setTimeout(() => {
              setShowAuth('register');
            }, 500);
          }}
        />
      )}
    </div>
  );
};

export default PublicLandingPage;