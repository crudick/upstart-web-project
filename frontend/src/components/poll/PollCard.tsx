import React from 'react';
import { ChartBarIcon, ClockIcon, UserIcon } from '@heroicons/react/24/outline';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { Poll } from '../../types';
import { getUserDisplayName } from '../../utils/userDisplay';

interface PollCardProps {
  poll: Poll;
  onViewPoll: (pollGuid: string) => void;
  showAuthor?: boolean;
}

const PollCard: React.FC<PollCardProps> = ({ 
  poll, 
  onViewPoll, 
  showAuthor = true 
}) => {
  const totalVotes = poll.stats?.length || 0;
  const isExpired = poll.expiresAt && new Date(poll.expiresAt) < new Date();

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  const getTimeRemaining = (expiresAt: string) => {
    const now = new Date();
    const expiry = new Date(expiresAt);
    const diff = expiry.getTime() - now.getTime();
    
    if (diff <= 0) return 'Expired';
    
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    
    if (days > 0) return `${days} day${days > 1 ? 's' : ''} left`;
    if (hours > 0) return `${hours} hour${hours > 1 ? 's' : ''} left`;
    return 'Less than 1 hour left';
  };

  return (
    <Card hover className="h-full">
      <div className="flex flex-col h-full">
        <div className="flex-1">
          <div className="flex items-start justify-between mb-3">
            <h3 className="text-lg font-semibold text-gray-900 font-montserrat line-clamp-2">
              {poll.question}
            </h3>
            {isExpired && (
              <span className="ml-2 px-2 py-1 text-xs font-semibold bg-red-100 text-red-800 rounded-full flex-shrink-0">
                Expired
              </span>
            )}
          </div>

          <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500 mb-4">
            {showAuthor && poll.user && (
              <div className="flex items-center gap-1">
                <UserIcon className="w-4 h-4" />
                <span className="font-montserrat">
                  {getUserDisplayName(poll.user)}
                </span>
              </div>
            )}

            <div className="flex items-center gap-1">
              <ChartBarIcon className="w-4 h-4" />
              <span className="font-montserrat">
                {totalVotes} vote{totalVotes !== 1 ? 's' : ''}
              </span>
            </div>

            <div className="flex items-center gap-1">
              <ClockIcon className="w-4 h-4" />
              <span className="font-montserrat">
                {formatDate(poll.createdAt)}
              </span>
            </div>
          </div>

          {poll.expiresAt && !isExpired && (
            <div className="mb-4 p-2 bg-yellow-50 border border-yellow-200 rounded-upstart">
              <div className="flex items-center gap-1 text-sm text-yellow-800">
                <ClockIcon className="w-4 h-4" />
                <span className="font-montserrat font-medium">
                  {getTimeRemaining(poll.expiresAt)}
                </span>
              </div>
            </div>
          )}

          <div className="space-y-2 mb-4">
            {poll.answers.slice(0, 3).map((answer, index) => (
              <div key={answer.id} className="flex items-center gap-2">
                <div className="w-2 h-2 rounded-full bg-primary-400"></div>
                <span className="text-sm text-gray-600 font-montserrat truncate">
                  {answer.answerText}
                </span>
              </div>
            ))}
            {poll.answers.length > 3 && (
              <div className="flex items-center gap-2">
                <div className="w-2 h-2 rounded-full bg-gray-300"></div>
                <span className="text-sm text-gray-500 font-montserrat italic">
                  +{poll.answers.length - 3} more option{poll.answers.length - 3 > 1 ? 's' : ''}
                </span>
              </div>
            )}
          </div>
        </div>

        <Button
          variant={isExpired ? 'ghost' : 'outline'}
          size="sm"
          onClick={() => onViewPoll(poll.pollGuid)}
          className="w-full"
        >
          {isExpired ? 'View Results' : 'Vote Now'}
        </Button>
      </div>
    </Card>
  );
};

export default PollCard;