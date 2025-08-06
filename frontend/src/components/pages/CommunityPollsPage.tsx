import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  MagnifyingGlassIcon,
  FunnelIcon,
  ClockIcon,
  UsersIcon,
  EyeIcon
} from '@heroicons/react/24/outline';
import { useNavigate } from 'react-router-dom';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { pollsAPI } from '../../services/api';
import { Poll } from '../../types';

const CommunityPollsPage: React.FC = () => {
  const navigate = useNavigate();
  const [polls, setPolls] = useState<Poll[]>([]);
  const [filteredPolls, setFilteredPolls] = useState<Poll[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState<'recent' | 'popular' | 'expiring'>('recent');

  useEffect(() => {
    fetchCommunityPolls();
  }, []);

  useEffect(() => {
    filterAndSortPolls();
  }, [polls, searchTerm, sortBy]);

  const fetchCommunityPolls = async () => {
    try {
      const publicPolls = await pollsAPI.getPublicPolls();
      setPolls(publicPolls);
    } catch (error) {
      console.error('Failed to fetch community polls:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filterAndSortPolls = () => {
    let filtered = polls.filter(poll =>
      poll.question.toLowerCase().includes(searchTerm.toLowerCase())
    );

    switch (sortBy) {
      case 'popular':
        filtered.sort((a, b) => (b.stats?.length || 0) - (a.stats?.length || 0));
        break;
      case 'expiring':
        filtered.sort((a, b) => {
          if (!a.expiresAt && !b.expiresAt) return 0;
          if (!a.expiresAt) return 1;
          if (!b.expiresAt) return -1;
          return new Date(a.expiresAt).getTime() - new Date(b.expiresAt).getTime();
        });
        break;
      case 'recent':
      default:
        filtered.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        break;
    }

    setFilteredPolls(filtered);
  };

  const handlePollClick = (poll: Poll) => {
    navigate(`/poll/${poll.pollGuid}`);
  };

  const formatTimeLeft = (expiresAt: string) => {
    const now = new Date();
    const expires = new Date(expiresAt);
    const diffMs = expires.getTime() - now.getTime();
    
    if (diffMs <= 0) return 'Expired';
    
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    
    if (diffDays > 0) return `${diffDays}d left`;
    if (diffHours > 0) return `${diffHours}h left`;
    return 'Ending soon';
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="h-48 bg-gray-200 rounded-upstart animate-pulse"></div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="p-6">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 font-montserrat mb-2">
          Community Polls
        </h1>
        <p className="text-gray-600 font-montserrat">
          Discover and participate in polls from the community
        </p>
      </div>

      {/* Search and Filter Controls */}
      <div className="flex flex-col sm:flex-row gap-4 mb-6">
        <div className="flex-1 relative">
          <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Search polls..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-upstart focus:ring-2 focus:ring-primary-500 focus:border-primary-500 font-montserrat"
          />
        </div>
        <div className="flex items-center gap-2">
          <FunnelIcon className="w-5 h-5 text-gray-400" />
          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value as 'recent' | 'popular' | 'expiring')}
            className="px-3 py-2 border border-gray-300 rounded-upstart focus:ring-2 focus:ring-primary-500 focus:border-primary-500 font-montserrat"
          >
            <option value="recent">Most Recent</option>
            <option value="popular">Most Popular</option>
            <option value="expiring">Expiring Soon</option>
          </select>
        </div>
      </div>

      {/* Polls Grid */}
      {filteredPolls.length > 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredPolls.map((poll, index) => (
            <motion.div
              key={poll.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
              onClick={() => handlePollClick(poll)}
              className="cursor-pointer"
            >
              <Card className="h-full hover:shadow-upstart-lg transition-shadow duration-300">
                <div className="p-6 h-full flex flex-col">
                  {/* Poll Header */}
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex-1">
                      <h3 className="font-semibold text-gray-900 font-montserrat text-lg mb-2 line-clamp-2">
                        {poll.question}
                      </h3>
                      {poll.user && (
                        <p className="text-sm text-gray-600 font-montserrat">
                          By {poll.user.firstName} {poll.user.lastName}
                        </p>
                      )}
                    </div>
                    <span className={`px-2 py-1 text-xs font-semibold rounded-full flex-shrink-0 ml-2 ${
                      poll.expiresAt && new Date(poll.expiresAt) < new Date()
                        ? 'bg-red-100 text-red-800'
                        : 'bg-green-100 text-green-800'
                    }`}>
                      {poll.expiresAt && new Date(poll.expiresAt) < new Date() ? 'Expired' : 'Active'}
                    </span>
                  </div>

                  {/* Poll Stats */}
                  <div className="flex items-center gap-4 text-sm text-gray-500 mb-4">
                    <span className="flex items-center gap-1">
                      <UsersIcon className="w-4 h-4" />
                      {poll.stats?.length || 0} votes
                    </span>
                    <span className="flex items-center gap-1">
                      <ClockIcon className="w-4 h-4" />
                      {new Date(poll.createdAt).toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric' 
                      })}
                    </span>
                  </div>

                  {/* Expiration Info */}
                  {poll.expiresAt && (
                    <div className="flex items-center gap-1 text-sm mb-4">
                      <ClockIcon className="w-4 h-4 text-orange-500" />
                      <span className={`font-medium ${
                        new Date(poll.expiresAt) < new Date() 
                          ? 'text-red-600' 
                          : 'text-orange-600'
                      }`}>
                        {formatTimeLeft(poll.expiresAt)}
                      </span>
                    </div>
                  )}

                  {/* Poll Options Preview */}
                  <div className="flex-1 mb-4">
                    <p className="text-sm text-gray-600 font-montserrat mb-2">
                      {poll.answers.length} options available
                    </p>
                    <div className="space-y-1">
                      {poll.answers.slice(0, 2).map((answer) => (
                        <div key={answer.id} className="text-sm text-gray-700 bg-gray-50 px-3 py-1 rounded-upstart truncate">
                          {answer.answerText}
                        </div>
                      ))}
                      {poll.answers.length > 2 && (
                        <div className="text-sm text-gray-500 px-3 py-1">
                          +{poll.answers.length - 2} more options
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Action Button */}
                  <Button 
                    variant="primary" 
                    className="w-full"
                    onClick={(e) => {
                      e.stopPropagation();
                      handlePollClick(poll);
                    }}
                  >
                    {poll.expiresAt && new Date(poll.expiresAt) < new Date() 
                      ? 'View Results' 
                      : 'Vote Now'
                    }
                  </Button>
                </div>
              </Card>
            </motion.div>
          ))}
        </div>
      ) : (
        <div className="text-center py-12">
          <EyeIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-2">
            {searchTerm ? 'No polls found' : 'No community polls available'}
          </h3>
          <p className="text-gray-600 font-montserrat mb-6">
            {searchTerm 
              ? 'Try adjusting your search terms or filters'
              : 'Be the first to create a poll for the community!'
            }
          </p>
          {!searchTerm && (
            <Button 
              variant="primary"
              onClick={() => navigate('/dashboard?tab=polls&action=create')}
            >
              Create Your First Poll
            </Button>
          )}
        </div>
      )}
    </div>
  );
};

export default CommunityPollsPage;