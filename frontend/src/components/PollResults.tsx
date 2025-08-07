import React, { useState, useEffect, useCallback } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartBarIcon, 
  ArrowLeftIcon, 
  ShareIcon, 
  ClockIcon, 
  UserIcon,
  TrophyIcon
} from '@heroicons/react/24/outline';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import Button from './ui/Button';
import Card from './ui/Card';
import { Poll } from '../types';
import { pollsAPI, pollAnswersAPI, pollStatsAPI } from '../services/api';
import { getUserDisplayName } from '../utils/userDisplay';

interface PollResultsProps {
  pollGuid: string;
  onBackToPoll?: () => void;
}

interface AnswerResult {
  id: number;
  answerText: string;
  count: number;
  percentage: number;
  color: string;
}

const colors = [
  '#00b1ac', '#46c2c7', '#81d9dc', '#b3e9ea', '#d7f3f3',
  '#00807b', '#009b97', '#0a4f4d', '#00625f', '#37465a'
];

const PollResults: React.FC<PollResultsProps> = ({ pollGuid, onBackToPoll }) => {
  const [poll, setPoll] = useState<Poll | null>(null);
  const [results, setResults] = useState<AnswerResult[]>([]);
  const [totalResponses, setTotalResponses] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadResults = useCallback(async () => {
    try {
      setLoading(true);
      setError('');

      // Load poll data and answers
      const [pollData, answersData] = await Promise.all([
        pollsAPI.getPollByGuid(pollGuid),
        pollsAPI.getPollByGuid(pollGuid).then(p => pollAnswersAPI.getPollAnswers(p.id))
      ]);

      setPoll(pollData);

      // Load results
      try {
        const resultsData = await pollStatsAPI.getPollResults(pollData.id);
        const total = resultsData.totalResponses;
        
        const answerResults: AnswerResult[] = answersData.map((answer, index) => {
          const result = resultsData.results.find(r => r.answerId === answer.id);
          const count = result?.count || 0;
          
          return {
            id: answer.id,
            answerText: answer.answerText,
            count,
            percentage: total > 0 ? (count / total) * 100 : 0,
            color: colors[index % colors.length],
          };
        });

        setResults(answerResults);
        setTotalResponses(total);
      } catch (resultsError) {
        // If results endpoint doesn't exist, create mock results from poll stats
        const answerResults: AnswerResult[] = answersData.map((answer, index) => {
          const count = pollData.stats?.filter(stat => stat.pollAnswerId === answer.id).length || 0;
          
          return {
            id: answer.id,
            answerText: answer.answerText,
            count,
            percentage: pollData.stats && pollData.stats.length > 0 ? (count / pollData.stats.length) * 100 : 0,
            color: colors[index % colors.length],
          };
        });

        setResults(answerResults);
        setTotalResponses(pollData.stats?.length || 0);
      }

    } catch (error: any) {
      console.error('Error loading results:', error);
      setError(error.message || 'Failed to load poll results');
    } finally {
      setLoading(false);
    }
  }, [pollGuid]);

  useEffect(() => {
    loadResults();
  }, [loadResults]);

  const handleSharePoll = async () => {
    const pollUrl = `${window.location.origin}/poll/${pollGuid}`;
    
    try {
      await navigator.clipboard.writeText(pollUrl);
      alert('Poll link copied to clipboard!');
    } catch (error) {
      prompt('Copy this link to share the poll:', pollUrl);
    }
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

  const getTopAnswer = () => {
    return results.reduce((top, current) => 
      current.count > top.count ? current : top
    , results[0]);
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600 font-montserrat">Loading results...</p>
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
            Results Not Available
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

  if (!poll) return null;

  const topAnswer = results.length > 0 ? getTopAnswer() : null;
  const isExpired = poll.expiresAt && new Date(poll.expiresAt) < new Date();

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="max-w-4xl mx-auto space-y-6"
    >
      {/* Navigation */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center space-x-4">
          {onBackToPoll ? (
            <Button 
              variant="ghost" 
              onClick={onBackToPoll}
              className="flex items-center space-x-2 text-gray-600"
            >
              <ArrowLeftIcon className="w-4 h-4" />
              <span>Back to Poll</span>
            </Button>
          ) : (
            <Button 
              variant="ghost" 
              onClick={() => window.location.href = '/'}
              className="flex items-center space-x-2 text-gray-600"
            >
              <ArrowLeftIcon className="w-4 h-4" />
              <span>Back to Home</span>
            </Button>
          )}
        </div>

        <Button 
          variant="outline" 
          onClick={handleSharePoll}
          className="flex items-center space-x-2"
        >
          <ShareIcon className="w-4 h-4" />
          <span>Share Poll</span>
        </Button>
      </div>

      {/* Poll Header */}
      <Card>
        <div className="text-center mb-6">
          <div className="flex items-center justify-center mb-4">
            <ChartBarIcon className="w-8 h-8 text-primary-600 mr-3" />
            <h1 className="text-2xl font-bold text-gray-900 font-montserrat">
              Poll Results
            </h1>
          </div>
          
          <h2 className="text-xl text-gray-800 font-montserrat mb-4">
            {poll.question}
          </h2>

          <div className="flex flex-wrap items-center justify-center gap-6 text-sm text-gray-500">
            {poll.user && (
              <div className="flex items-center space-x-1">
                <UserIcon className="w-4 h-4" />
                <span className="font-montserrat">
                  {getUserDisplayName(poll.user)}
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

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
          <div className="text-center">
            <div className="text-3xl font-bold text-primary-600 font-montserrat">
              {totalResponses}
            </div>
            <div className="text-sm text-gray-600 font-montserrat">
              Total Votes
            </div>
          </div>
          
          <div className="text-center">
            <div className="text-3xl font-bold text-primary-600 font-montserrat">
              {results.length}
            </div>
            <div className="text-sm text-gray-600 font-montserrat">
              Answer Options
            </div>
          </div>

          {topAnswer && (
            <div className="text-center">
              <div className="text-3xl font-bold text-primary-600 font-montserrat">
                {topAnswer.percentage.toFixed(0)}%
              </div>
              <div className="text-sm text-gray-600 font-montserrat">
                Top Answer
              </div>
            </div>
          )}
        </div>

        {/* Winner Banner */}
        {topAnswer && topAnswer.count > 0 && (
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            className="mb-6 p-4 bg-gradient-to-r from-yellow-50 to-yellow-100 border border-yellow-200 rounded-upstart"
          >
            <div className="flex items-center justify-center space-x-2">
              <TrophyIcon className="w-6 h-6 text-yellow-600" />
              <div className="text-center">
                <p className="font-semibold text-yellow-800 font-montserrat">
                  Leading Answer
                </p>
                <p className="text-yellow-700 font-montserrat">
                  "{topAnswer.answerText}" with {topAnswer.count} vote{topAnswer.count !== 1 ? 's' : ''} ({topAnswer.percentage.toFixed(1)}%)
                </p>
              </div>
            </div>
          </motion.div>
        )}
      </Card>

      {/* Results Visualization */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Bar Chart */}
        <Card>
          <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-4">
            Vote Distribution
          </h3>
          <div className="h-64">
            {totalResponses > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={results} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis 
                    dataKey="answerText" 
                    tick={false}
                    height={20}
                  />
                  <YAxis />
                  <Tooltip 
                    formatter={(value, name) => [`${value} votes`, 'Votes']}
                    labelFormatter={(label) => `Option: ${label}`}
                  />
                  <Bar dataKey="count" fill="#00b1ac" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <div className="flex items-center justify-center h-full">
                <div className="text-center">
                  <ChartBarIcon className="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <p className="text-gray-500 font-montserrat">No votes yet</p>
                </div>
              </div>
            )}
          </div>
        </Card>

        {/* Pie Chart */}
        <Card>
          <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-4">
            Percentage Breakdown
          </h3>
          <div className="h-64">
            {totalResponses > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={results}
                    dataKey="count"
                    nameKey="answerText"
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    label={({ percentage }) => percentage > 0 ? `${percentage.toFixed(0)}%` : ''}
                  >
                    {results.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip 
                    formatter={(value, name) => [`${value} votes (${((value as number / totalResponses) * 100).toFixed(1)}%)`, name]}
                  />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <div className="flex items-center justify-center h-full">
                <div className="text-center">
                  <ChartBarIcon className="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <p className="text-gray-500 font-montserrat">No votes yet</p>
                </div>
              </div>
            )}
          </div>
        </Card>
      </div>

      {/* Detailed Results */}
      <Card>
        <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-4">
          Detailed Results
        </h3>
        <div className="space-y-4">
          {results.map((result, index) => (
            <motion.div
              key={result.id}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center space-x-3">
                  <div 
                    className="w-4 h-4 rounded"
                    style={{ backgroundColor: result.color }}
                  ></div>
                  <span className="font-medium text-gray-900 font-montserrat">
                    {result.answerText}
                  </span>
                </div>
                <div className="text-right">
                  <div className="font-semibold text-gray-900 font-montserrat">
                    {result.count} vote{result.count !== 1 ? 's' : ''}
                  </div>
                  <div className="text-sm text-gray-600 font-montserrat">
                    {result.percentage.toFixed(1)}%
                  </div>
                </div>
              </div>
              
              {/* Progress Bar */}
              <div className="w-full bg-gray-200 rounded-full h-3">
                <motion.div
                  className="h-3 rounded-full"
                  style={{ backgroundColor: result.color }}
                  initial={{ width: 0 }}
                  animate={{ width: `${result.percentage}%` }}
                  transition={{ duration: 1, delay: index * 0.1 }}
                />
              </div>
            </motion.div>
          ))}
        </div>

        {totalResponses === 0 && (
          <div className="text-center py-8">
            <ChartBarIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 font-montserrat mb-2">
              No votes have been cast yet
            </p>
            <p className="text-sm text-gray-500 font-montserrat">
              Share the poll to start collecting responses
            </p>
          </div>
        )}
      </Card>
    </motion.div>
  );
};

export default PollResults;