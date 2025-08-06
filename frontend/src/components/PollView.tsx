import React, { useState, useEffect, useCallback } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartBarIcon, 
  ClockIcon, 
  UserIcon, 
  CheckCircleIcon,
  ShareIcon,
  ArrowLeftIcon,
  PencilIcon,
  TrashIcon
} from '@heroicons/react/24/outline';
import Button from './ui/Button';
import Card from './ui/Card';
import PollEditForm from './PollEditForm';
import { useAuth } from '../contexts/AuthContext';
import { Poll, PollAnswer, PollStat } from '../types';
import { pollsAPI, pollAnswersAPI, pollStatsAPI } from '../services/api';

interface PollViewProps {
  pollGuid: string;
  onViewResults?: () => void;
}

const PollView: React.FC<PollViewProps> = ({ pollGuid, onViewResults }) => {
  const { user, isAuthenticated } = useAuth();
  const [poll, setPoll] = useState<Poll | null>(null);
  const [answers, setAnswers] = useState<PollAnswer[]>([]);
  const [selectedAnswers, setSelectedAnswers] = useState<number[]>([]);
  const [userResponse, setUserResponse] = useState<PollStat | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string>('');
  const [showThankYou, setShowThankYou] = useState(false);
  const [showEditForm, setShowEditForm] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const loadPoll = useCallback(async () => {
    try {
      setLoading(true);
      setError('');

      // Load poll data
      const pollData = await pollsAPI.getPollByGuid(pollGuid);
      setPoll(pollData);

      // Load poll answers
      const answersData = await pollAnswersAPI.getPollAnswers(pollData.id);
      setAnswers(answersData);

      // Check if user has already responded (only if authenticated)
      if (isAuthenticated && user) {
        const existingResponse = await pollStatsAPI.getUserPollResponse(pollData.id);
        setUserResponse(existingResponse);

        if (existingResponse) {
          setSelectedAnswers([existingResponse.pollAnswerId]);
        }
      }
    } catch (error: any) {
      console.error('Error loading poll:', error);
      setError(error.message || 'Failed to load poll');
    } finally {
      setLoading(false);
    }
  }, [pollGuid, isAuthenticated, user]);

  useEffect(() => {
    loadPoll();
  }, [loadPoll]);

  const handleAnswerSelect = (answerId: number) => {
    if (!poll || userResponse) return;

    if (poll.allowMultipleResponses) {
      setSelectedAnswers(prev => 
        prev.includes(answerId) 
          ? prev.filter(id => id !== answerId)
          : [...prev, answerId]
      );
    } else {
      setSelectedAnswers([answerId]);
    }
  };

  const handleSubmitVote = async () => {
    if (!poll || selectedAnswers.length === 0) return;
    
    // Check if authentication is required
    if (poll.requiresAuthentication && !isAuthenticated) {
      setError('Authentication required to vote on this poll');
      return;
    }

    setSubmitting(true);
    try {
      // For now, just submit the first selected answer
      // In a real implementation, you'd handle multiple answers
      if (poll.requiresAuthentication || isAuthenticated) {
        await pollStatsAPI.submitVote(poll.id, selectedAnswers[0]);
      } else {
        await pollStatsAPI.submitAnonymousVote(poll.id, selectedAnswers[0]);
      }
      
      setShowThankYou(true);
      setTimeout(() => {
        if (onViewResults) {
          onViewResults();
        }
      }, 2000);
    } catch (error: any) {
      console.error('Error submitting vote:', error);
      setError(error.message || 'Failed to submit vote');
    } finally {
      setSubmitting(false);
    }
  };

  const handleSharePoll = async () => {
    const pollUrl = window.location.href;
    
    try {
      await navigator.clipboard.writeText(pollUrl);
      alert('Poll link copied to clipboard!');
    } catch (error) {
      // Fallback for browsers that don't support clipboard API
      prompt('Copy this link to share the poll:', pollUrl);
    }
  };

  const handleDeletePoll = async () => {
    if (!poll || !window.confirm('Are you sure you want to delete this poll? This action cannot be undone.')) {
      return;
    }

    setDeleting(true);
    try {
      await pollsAPI.deletePoll(poll.id);
      window.location.href = '/';
    } catch (error: any) {
      setError(error.message || 'Failed to delete poll');
    } finally {
      setDeleting(false);
    }
  };

  const handlePollUpdated = (updatedPoll: Poll) => {
    setPoll(updatedPoll);
    setAnswers(updatedPoll.answers);
    setShowEditForm(false);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'long',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const isExpired = poll?.expiresAt && new Date(poll.expiresAt) < new Date();
  const requiresAuth = poll?.requiresAuthentication;
  const canVote = (!requiresAuth || isAuthenticated) && !userResponse && !isExpired;
  const isOwner = isAuthenticated && poll?.user?.id === user?.id;
  const hasVotes = poll?.stats && poll.stats.length > 0;
  const canEdit = isOwner && !hasVotes;

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600 font-montserrat">Loading poll...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Card className="max-w-md mx-auto text-center">
          <div className="text-red-600 mb-4">
            <ChartBarIcon className="w-12 h-12 mx-auto mb-4" />
          </div>
          <h2 className="text-xl font-semibold text-gray-900 font-montserrat mb-2">
            Poll Not Found
          </h2>
          <p className="text-gray-600 font-montserrat mb-6">
            {error}
          </p>
          <Button onClick={() => window.location.href = '/'} variant="primary">
            Go to Home
          </Button>
        </Card>
      </div>
    );
  }

  if (!poll) {
    return null;
  }

  if (showThankYou) {
    return (
      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        className="min-h-screen flex items-center justify-center"
      >
        <Card className="max-w-md mx-auto text-center">
          <motion.div
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            transition={{ delay: 0.2, type: 'spring', stiffness: 200 }}
          >
            <CheckCircleIcon className="w-16 h-16 text-primary-600 mx-auto mb-4" />
          </motion.div>
          <h2 className="text-2xl font-bold text-gray-900 font-montserrat mb-2">
            Thank You!
          </h2>
          <p className="text-gray-600 font-montserrat mb-6">
            Your vote has been recorded. Redirecting to results...
          </p>
          <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin mx-auto"></div>
        </Card>
      </motion.div>
    );
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="max-w-2xl mx-auto"
    >
      {/* Navigation */}
      <div className="mb-6">
        <Button 
          variant="ghost" 
          onClick={() => window.location.href = '/'}
          className="flex items-center space-x-2 text-gray-600"
        >
          <ArrowLeftIcon className="w-4 h-4" />
          <span>Back to Home</span>
        </Button>
      </div>

      <Card>
        {/* Poll Header */}
        <div className="mb-6">
          <div className="flex items-center justify-between mb-4">
            <h1 className="text-2xl font-bold text-gray-900 font-montserrat">
              {poll.question}
            </h1>
            <div className="flex items-center space-x-2">
              {isOwner && (
                <>
                  {canEdit && (
                    <Button 
                      variant="ghost" 
                      onClick={() => setShowEditForm(true)}
                      className="flex items-center space-x-1 text-blue-600"
                    >
                      <PencilIcon className="w-4 h-4" />
                      <span>Edit</span>
                    </Button>
                  )}
                  <Button 
                    variant="ghost" 
                    onClick={handleDeletePoll}
                    isLoading={deleting}
                    className="flex items-center space-x-1 text-red-600"
                  >
                    <TrashIcon className="w-4 h-4" />
                    <span>Delete</span>
                  </Button>
                </>
              )}
              <Button 
                variant="ghost" 
                onClick={handleSharePoll}
                className="flex items-center space-x-1 text-primary-600"
              >
                <ShareIcon className="w-4 h-4" />
                <span>Share</span>
              </Button>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500">
            {poll.user && (
              <div className="flex items-center space-x-1">
                <UserIcon className="w-4 h-4" />
                <span className="font-montserrat">
                  {poll.user.firstName} {poll.user.lastName}
                </span>
              </div>
            )}

            <div className="flex items-center space-x-1">
              <ClockIcon className="w-4 h-4" />
              <span className="font-montserrat">
                Created {formatDate(poll.createdAt)}
              </span>
            </div>

            {poll.expiresAt && (
              <div className="flex items-center space-x-1">
                <ClockIcon className="w-4 h-4" />
                <span className={`font-montserrat ${isExpired ? 'text-red-600' : ''}`}>
                  {isExpired ? 'Expired' : 'Expires'} {formatDate(poll.expiresAt)}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Status Messages */}
        {requiresAuth && !isAuthenticated && (
          <div className="mb-6 p-4 bg-yellow-50 border border-yellow-200 rounded-upstart">
            <p className="text-sm text-yellow-800 font-montserrat">
              🔒 This poll requires you to be signed in to vote.
            </p>
          </div>
        )}

        {userResponse && (
          <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded-upstart">
            <p className="text-sm text-green-800 font-montserrat">
              ✓ You have already voted on this poll.
            </p>
          </div>
        )}

        {isExpired && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-upstart">
            <p className="text-sm text-red-800 font-montserrat">
              This poll has expired and is no longer accepting votes.
            </p>
          </div>
        )}

        {isOwner && hasVotes && (
          <div className="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-upstart">
            <p className="text-sm text-blue-800 font-montserrat">
              📊 This poll has received votes and can no longer be edited. You can still view results and share the poll.
            </p>
          </div>
        )}

        {/* Answer Options */}
        <div className="space-y-3 mb-6">
          {answers.map((answer) => {
            const isSelected = selectedAnswers.includes(answer.id);
            const isUserAnswer = userResponse?.pollAnswerId === answer.id;
            
            return (
              <motion.div
                key={answer.id}
                whileHover={canVote ? { scale: 1.01 } : {}}
                whileTap={canVote ? { scale: 0.99 } : {}}
              >
                <button
                  onClick={() => handleAnswerSelect(answer.id)}
                  disabled={!canVote}
                  className={`w-full p-4 text-left border-2 rounded-upstart transition-all duration-200 ${
                    isSelected || isUserAnswer
                      ? 'border-primary-500 bg-primary-50'
                      : 'border-gray-200 hover:border-gray-300'
                  } ${
                    !canVote ? 'cursor-default' : 'cursor-pointer hover:shadow-upstart'
                  }`}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center space-x-3">
                      <div className={`w-4 h-4 border-2 rounded ${
                        poll.allowMultipleResponses ? 'rounded-sm' : 'rounded-full'
                      } ${
                        isSelected || isUserAnswer
                          ? 'bg-primary-500 border-primary-500'
                          : 'border-gray-300'
                      }`}>
                        {(isSelected || isUserAnswer) && (
                          <div className="w-full h-full flex items-center justify-center">
                            {poll.allowMultipleResponses ? (
                              <CheckCircleIcon className="w-3 h-3 text-white" />
                            ) : (
                              <div className="w-2 h-2 bg-white rounded-full"></div>
                            )}
                          </div>
                        )}
                      </div>
                      <span className="font-montserrat text-gray-900">
                        {answer.answerText}
                      </span>
                    </div>
                    {isUserAnswer && (
                      <CheckCircleIcon className="w-5 h-5 text-primary-600" />
                    )}
                  </div>
                </button>
              </motion.div>
            );
          })}
        </div>

        {/* Action Buttons */}
        <div className="flex flex-col sm:flex-row gap-3">
          {canVote && (
            <Button
              onClick={handleSubmitVote}
              variant="primary"
              isLoading={submitting}
              disabled={selectedAnswers.length === 0}
              className="flex-1"
            >
              Submit Vote
            </Button>
          )}
          
          {onViewResults && (
            <Button
              onClick={onViewResults}
              variant={canVote ? "outline" : "primary"}
              className="flex-1"
            >
              <ChartBarIcon className="w-4 h-4 mr-2" />
              View Results
            </Button>
          )}
        </div>

        {poll.allowMultipleResponses && canVote && (
          <p className="mt-3 text-xs text-gray-500 font-montserrat text-center">
            You can select multiple options for this poll
          </p>
        )}
      </Card>

      {/* Edit Form Modal */}
      {showEditForm && poll && (
        <PollEditForm 
          poll={poll}
          onClose={() => setShowEditForm(false)}
          onPollUpdated={handlePollUpdated}
        />
      )}
    </motion.div>
  );
};

export default PollView;