import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  ChartBarIcon, 
  UsersIcon, 
  EyeIcon, 
  ArrowTrendingUpIcon,
  ClockIcon
} from '@heroicons/react/24/outline';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { pollsAPI, pollStatsAPI } from '../../services/api';
import { Poll } from '../../types';

interface DashboardPageProps {
  onCreatePoll: () => void;
  onViewPolls: () => void;
}

const DashboardPage: React.FC<DashboardPageProps> = ({ onCreatePoll, onViewPolls }) => {
  const [polls, setPolls] = useState<Poll[]>([]);
  const [stats, setStats] = useState({
    totalPolls: 0,
    totalVotes: 0,
    totalViews: 0,
    recentActivity: [] as any[],
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const [userPolls, userStats] = await Promise.all([
        pollsAPI.getUserPolls(),
        pollStatsAPI.getUserStats().catch(() => ({
          totalPolls: 0,
          totalVotes: 0,
          totalViews: 0,
          recentActivity: [],
        }))
      ]);
      
      setPolls(userPolls);
      setStats({
        totalPolls: userPolls.length,
        totalVotes: userPolls.reduce((total, poll) => total + (poll.stats?.length || 0), 0),
        totalViews: userStats.totalViews || 0,
        recentActivity: userStats.recentActivity || [],
      });
    } catch (error) {
      console.error('Failed to fetch dashboard data:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const getChartData = () => {
    const last7Days = Array.from({ length: 7 }, (_, i) => {
      const date = new Date();
      date.setDate(date.getDate() - i);
      return {
        date: date.toISOString().split('T')[0],
        label: date.toLocaleDateString('en-US', { weekday: 'short' }),
        votes: 0,
      };
    }).reverse();

    // Simulate some data based on polls
    polls.forEach((poll) => {
      if (poll.stats) {
        poll.stats.forEach((stat) => {
          try {
            const date = new Date(stat.selectedAt);
            if (!isNaN(date.getTime())) { // Check if date is valid
              const statDate = date.toISOString().split('T')[0];
              const dayData = last7Days.find(day => day.date === statDate);
              if (dayData) {
                dayData.votes += 1;
              }
            }
          } catch (error) {
            console.warn('Invalid date in poll stat:', stat.selectedAt);
          }
        });
      }
    });

    return last7Days;
  };

  const getPollDistributionData = () => {
    const colors = ['#00b1ac', '#46c2c7', '#81d9dc', '#b3e9ea', '#d7f3f3'];
    
    return polls.slice(0, 5).map((poll, index) => ({
      name: poll.question.length > 30 ? poll.question.substring(0, 30) + '...' : poll.question,
      votes: poll.stats?.length || 0,
      color: colors[index % colors.length],
    }));
  };

  const getTopPerformingPolls = () => {
    return [...polls]
      .sort((a, b) => (b.stats?.length || 0) - (a.stats?.length || 0))
      .slice(0, 5);
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="h-24 bg-gray-200 rounded-upstart animate-pulse"></div>
          ))}
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="h-64 bg-gray-200 rounded-upstart animate-pulse"></div>
          ))}
        </div>
      </div>
    );
  }

  const chartData = getChartData();
  const pollDistributionData = getPollDistributionData();
  const topPolls = getTopPerformingPolls();

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 font-montserrat mb-2">
            Dashboard
          </h1>
          <p className="text-gray-600 font-montserrat">
            Overview of your polling activity and insights
          </p>
        </div>
        <Button
          onClick={onCreatePoll}
          variant="primary"
          className="mt-4 sm:mt-0"
        >
          Create New Poll
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 font-montserrat">
                  Total Polls
                </p>
                <p className="text-3xl font-bold text-gray-900 font-montserrat">
                  {stats.totalPolls}
                </p>
              </div>
              <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
                <ChartBarIcon className="w-6 h-6 text-primary-600" />
              </div>
            </div>
          </Card>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
        >
          <Card>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 font-montserrat">
                  Total Votes
                </p>
                <p className="text-3xl font-bold text-gray-900 font-montserrat">
                  {stats.totalVotes}
                </p>
              </div>
              <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                <UsersIcon className="w-6 h-6 text-green-600" />
              </div>
            </div>
          </Card>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.3 }}
        >
          <Card>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 font-montserrat">
                  Poll Views
                </p>
                <p className="text-3xl font-bold text-gray-900 font-montserrat">
                  {stats.totalViews}
                </p>
              </div>
              <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
                <EyeIcon className="w-6 h-6 text-blue-600" />
              </div>
            </div>
          </Card>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
        >
          <Card>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-600 font-montserrat">
                  Avg. Votes/Poll
                </p>
                <p className="text-3xl font-bold text-gray-900 font-montserrat">
                  {stats.totalPolls > 0 ? Math.round(stats.totalVotes / stats.totalPolls) : 0}
                </p>
              </div>
              <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
                <ArrowTrendingUpIcon className="w-6 h-6 text-yellow-600" />
              </div>
            </div>
          </Card>
        </motion.div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Votes Over Time */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
        >
          <Card>
            <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-4">
              Votes Over Time
            </h3>
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="label" />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="votes" fill="#00b1ac" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </Card>
        </motion.div>

        {/* Poll Distribution */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.6 }}
        >
          <Card>
            <h3 className="text-lg font-semibold text-gray-900 font-montserrat mb-4">
              Vote Distribution by Poll
            </h3>
            <div className="h-64">
              {pollDistributionData.length > 0 ? (
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={pollDistributionData}
                      dataKey="votes"
                      nameKey="name"
                      cx="50%"
                      cy="50%"
                      outerRadius={80}
                      label={({ name, percent }) => `${percent && percent > 0 ? `${(percent * 100).toFixed(0)}%` : ''}`}
                    >
                      {pollDistributionData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              ) : (
                <div className="flex items-center justify-center h-full">
                  <div className="text-center">
                    <ChartBarIcon className="w-12 h-12 text-gray-400 mx-auto mb-2" />
                    <p className="text-gray-500 font-montserrat">No data to display</p>
                  </div>
                </div>
              )}
            </div>
          </Card>
        </motion.div>
      </div>

      {/* Top Performing Polls */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.7 }}
      >
        <Card>
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-semibold text-gray-900 font-montserrat">
              Top Performing Polls
            </h3>
            <Button variant="ghost" onClick={onViewPolls} className="text-primary-600">
              View All
            </Button>
          </div>

          {topPolls.length > 0 ? (
            <div className="space-y-4">
              {topPolls.map((poll, index) => (
                <div key={poll.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-upstart">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-3">
                      <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center text-primary-600 font-semibold text-sm">
                        {index + 1}
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-medium text-gray-900 font-montserrat truncate">
                          {poll.question}
                        </p>
                        <div className="flex items-center space-x-4 text-sm text-gray-500 mt-1">
                          <span className="flex items-center space-x-1">
                            <UsersIcon className="w-4 h-4" />
                            <span>{poll.stats?.length || 0} votes</span>
                          </span>
                          <span className="flex items-center space-x-1">
                            <ClockIcon className="w-4 h-4" />
                            <span>
                              {new Date(poll.createdAt).toLocaleDateString('en-US', { 
                                month: 'short', 
                                day: 'numeric' 
                              })}
                            </span>
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="ml-4">
                    <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                      poll.expiresAt && new Date(poll.expiresAt) < new Date()
                        ? 'bg-red-100 text-red-800'
                        : 'bg-green-100 text-green-800'
                    }`}>
                      {poll.expiresAt && new Date(poll.expiresAt) < new Date() ? 'Expired' : 'Active'}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8">
              <ChartBarIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-600 font-montserrat mb-4">
                No polls created yet
              </p>
              <Button onClick={onCreatePoll} variant="primary">
                Create Your First Poll
              </Button>
            </div>
          )}
        </Card>
      </motion.div>
    </div>
  );
};

export default DashboardPage;