import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  PlusIcon, 
  ChartBarIcon, 
  EyeIcon, 
  TrashIcon,
  ShareIcon,
  ClockIcon,
  CheckCircleIcon
} from '@heroicons/react/24/outline';
import Button from '../ui/Button';
import Card from '../ui/Card';
import { Poll } from '../../types';
import { pollsAPI } from '../../services/api';

interface PollsPageProps {
  onCreatePoll: () => void;
  onViewPoll: (pollGuid: string) => void;
}

const PollsPage: React.FC<PollsPageProps> = ({ onCreatePoll, onViewPoll }) => {
  const [polls, setPolls] = useState<Poll[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [deletingPollId, setDeletingPollId] = useState<number | null>(null);

  useEffect(() => {
    fetchUserPolls();
  }, []);

  const fetchUserPolls = async () => {
    try {
      const userPolls = await pollsAPI.getUserPolls();
      setPolls(userPolls);
    } catch (error) {
      console.error('Failed to fetch user polls:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeletePoll = async (pollId: number) => {
    if (!window.confirm('Are you sure you want to delete this poll? This action cannot be undone.')) {
      return;
    }

    setDeletingPollId(pollId);
    try {
      await pollsAPI.deletePoll(pollId);
      setPolls(polls.filter(poll => poll.id !== pollId));
    } catch (error) {
      console.error('Failed to delete poll:', error);
      alert('Failed to delete poll. Please try again.');
    } finally {
      setDeletingPollId(null);
    }
  };

  const handleSharePoll = async (pollGuid: string) => {
    const pollUrl = `${window.location.origin}/poll/${pollGuid}`;
    
    try {
      await navigator.clipboard.writeText(pollUrl);
      alert('Poll link copied to clipboard!');
    } catch (error) {
      // Fallback for browsers that don't support clipboard API
      prompt('Copy this link to share your poll:', pollUrl);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getPollStatus = (poll: Poll) => {
    const isExpired = poll.expiresAt && new Date(poll.expiresAt) < new Date();
    const totalVotes = poll.stats?.length || 0;
    
    if (isExpired) {
      return { status: 'Expired', color: 'red', icon: ClockIcon };
    }
    
    if (totalVotes > 0) {
      return { status: 'Active', color: 'green', icon: CheckCircleIcon };
    }
    
    return { status: 'No votes yet', color: 'yellow', icon: ClockIcon };
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="flex items-center justify-between mb-8">
          <div>
            <div className="h-8 w-32 bg-gray-200 rounded animate-pulse mb-2"></div>
            <div className="h-5 w-48 bg-gray-200 rounded animate-pulse"></div>
          </div>
          <div className="h-10 w-32 bg-gray-200 rounded animate-pulse"></div>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, index) => (
            <div key={index} className="h-64 bg-gray-200 rounded-upstart animate-pulse"></div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 font-montserrat mb-2">
            My Polls
          </h1>
          <p className="text-gray-600 font-montserrat">
            Create, manage, and track your polls
          </p>
        </div>
        <Button
          onClick={onCreatePoll}
          variant="primary"
          className="mt-4 sm:mt-0 flex items-center space-x-2"
        >
          <PlusIcon className="w-5 h-5" />
          <span>Create New Poll</span>
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600 font-montserrat">
                Total Polls
              </p>
              <p className="text-3xl font-bold text-gray-900 font-montserrat">
                {polls.length}
              </p>
            </div>
            <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
              <ChartBarIcon className="w-6 h-6 text-primary-600" />
            </div>
          </div>
        </Card>

        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600 font-montserrat">
                Total Votes
              </p>
              <p className="text-3xl font-bold text-gray-900 font-montserrat">
                {polls.reduce((total, poll) => total + (poll.stats?.length || 0), 0)}
              </p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <CheckCircleIcon className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </Card>

        <Card>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600 font-montserrat">
                Active Polls
              </p>
              <p className="text-3xl font-bold text-gray-900 font-montserrat">
                {polls.filter(poll => !poll.expiresAt || new Date(poll.expiresAt) > new Date()).length}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <ClockIcon className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </Card>
      </div>

      {/* Polls List */}
      {polls.length === 0 ? (
        <Card className="text-center py-12">
          <ChartBarIcon className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-2">
            No polls yet
          </h3>
          <p className="text-gray-600 font-montserrat mb-6">
            Create your first poll to start gathering responses from your audience.
          </p>
          <Button onClick={onCreatePoll} variant="primary">
            <PlusIcon className="w-5 h-5 mr-2" />
            Create Your First Poll
          </Button>
        </Card>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {polls.map((poll) => {
            const pollStatus = getPollStatus(poll);
            const totalVotes = poll.stats?.length || 0;
            
            return (
              <motion.div
                key={poll.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                <Card hover className="h-full">
                  <div className="flex flex-col h-full">
                    <div className="flex-1">
                      <div className="flex items-start justify-between mb-3">
                        <h3 className="text-lg font-semibold text-gray-900 font-montserrat line-clamp-2 flex-1 mr-2">
                          {poll.question}
                        </h3>
                        <div className={`px-2 py-1 text-xs font-semibold rounded-full flex items-center space-x-1 ${
                          pollStatus.color === 'green' ? 'bg-green-100 text-green-800' :
                          pollStatus.color === 'red' ? 'bg-red-100 text-red-800' :
                          'bg-yellow-100 text-yellow-800'
                        }`}>
                          <pollStatus.icon className="w-3 h-3" />
                          <span>{pollStatus.status}</span>
                        </div>
                      </div>

                      <div className="space-y-2 mb-4">
                        <div className="flex items-center justify-between text-sm text-gray-600">
                          <span className="font-montserrat">
                            {poll.answers.length} option{poll.answers.length !== 1 ? 's' : ''}
                          </span>
                          <span className="font-montserrat">
                            {totalVotes} vote{totalVotes !== 1 ? 's' : ''}
                          </span>
                        </div>
                        
                        <p className="text-sm text-gray-500 font-montserrat">
                          Created {formatDate(poll.createdAt)}
                        </p>
                        
                        {poll.expiresAt && (
                          <p className="text-sm text-gray-500 font-montserrat">
                            {new Date(poll.expiresAt) > new Date() ? 'Expires' : 'Expired'} {formatDate(poll.expiresAt)}
                          </p>
                        )}
                      </div>
                    </div>

                    <div className="flex items-center space-x-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onViewPoll(poll.pollGuid)}
                        className="flex-1"
                      >
                        <EyeIcon className="w-4 h-4 mr-1" />
                        View
                      </Button>
                      
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleSharePoll(poll.pollGuid)}
                      >
                        <ShareIcon className="w-4 h-4" />
                      </Button>
                      
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleDeletePoll(poll.id)}
                        className="text-red-600 hover:text-red-700 hover:bg-red-50"
                        disabled={deletingPollId === poll.id}
                      >
                        {deletingPollId === poll.id ? (
                          <div className="w-4 h-4 border-2 border-red-600 border-t-transparent rounded-full animate-spin"></div>
                        ) : (
                          <TrashIcon className="w-4 h-4" />
                        )}
                      </Button>
                    </div>
                  </div>
                </Card>
              </motion.div>
            );
          })}
        </div>
      )}
    </div>
  );
};

export default PollsPage;