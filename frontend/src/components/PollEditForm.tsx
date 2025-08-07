import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { XMarkIcon, PlusIcon, TrashIcon } from '@heroicons/react/24/outline';
import Button from './ui/Button';
import Input from './ui/Input';
import Card from './ui/Card';
import { Poll } from '../types';
import { pollsAPI, pollAnswersAPI } from '../services/api';

interface PollEditFormProps {
  poll: Poll;
  onClose: () => void;
  onPollUpdated: (updatedPoll: Poll) => void;
}

const PollEditForm: React.FC<PollEditFormProps> = ({ poll, onClose, onPollUpdated }) => {
  const [formData, setFormData] = useState({
    question: poll.question,
    expiresAt: poll.expiresAt ? new Date(poll.expiresAt).toISOString().slice(0, 16) : '',
    allowMultipleResponses: poll.allowMultipleResponses || false,
    answers: poll.answers.map(a => a.answerText),
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    const validAnswers = formData.answers.filter(answer => answer.trim().length > 0);
    if (validAnswers.length < 2) {
      setError('Please provide at least 2 answer options');
      setIsLoading(false);
      return;
    }

    try {
      // Update the poll
      await pollsAPI.updatePoll(poll.id, {
        question: formData.question.trim(),
        expiresAt: formData.expiresAt || undefined,
        allowMultipleResponses: formData.allowMultipleResponses,
      });

      // Handle answer updates (this is simplified - in a real app you'd want to be more surgical)
      // Delete existing answers and create new ones
      const existingAnswers = await pollAnswersAPI.getPollAnswers(poll.id);
      await Promise.all(existingAnswers.map(answer => pollAnswersAPI.deletePollAnswer(answer.id)));

      // Create new answers
      await Promise.all(
        validAnswers.map((answerText, index) =>
          pollAnswersAPI.createPollAnswer(poll.id, answerText.trim(), index + 1)
        )
      );

      // Fetch the complete updated poll with answers
      const refreshedPoll = await pollsAPI.getPollByGuid(poll.pollGuid);
      onPollUpdated(refreshedPoll);
      onClose();
    } catch (err: any) {
      setError(err.message || 'Failed to update poll. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const addAnswer = () => {
    if (formData.answers.length < 10) {
      setFormData({
        ...formData,
        answers: [...formData.answers, ''],
      });
    }
  };

  const removeAnswer = (index: number) => {
    if (formData.answers.length > 2) {
      setFormData({
        ...formData,
        answers: formData.answers.filter((_, i) => i !== index),
      });
    }
  };

  const updateAnswer = (index: number, value: string) => {
    const newAnswers = [...formData.answers];
    newAnswers[index] = value;
    setFormData({ ...formData, answers: newAnswers });
  };

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      exit={{ opacity: 0 }}
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
    >
      <motion.div
        initial={{ scale: 0.95, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        exit={{ scale: 0.95, opacity: 0 }}
        className="w-full max-w-2xl max-h-[90vh] overflow-y-auto"
      >
        <Card>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-gray-900 font-montserrat">
              Edit Poll
            </h2>
            <Button variant="ghost" onClick={onClose} className="p-2">
              <XMarkIcon className="w-6 h-6" />
            </Button>
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

          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Question */}
            <Input
              label="Poll Question"
              placeholder="What would you like to ask your audience?"
              value={formData.question}
              onChange={(e) => setFormData({ ...formData, question: e.target.value })}
              required
            />

            {/* Answer Options */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-3 font-montserrat">
                Answer Options
              </label>
              <div className="space-y-3">
                {formData.answers.map((answer, index) => (
                  <motion.div
                    key={index}
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    className="flex items-center space-x-3"
                  >
                    <div className="flex-1">
                      <Input
                        placeholder={`Answer option ${index + 1}`}
                        value={answer}
                        onChange={(e) => updateAnswer(index, e.target.value)}
                        required
                      />
                    </div>
                    {formData.answers.length > 2 && (
                      <Button
                        type="button"
                        variant="ghost"
                        onClick={() => removeAnswer(index)}
                        className="p-2 text-red-600 hover:text-red-700 hover:bg-red-50"
                      >
                        <TrashIcon className="w-5 h-5" />
                      </Button>
                    )}
                  </motion.div>
                ))}
              </div>

              {formData.answers.length < 10 && (
                <Button
                  type="button"
                  variant="ghost"
                  onClick={addAnswer}
                  className="mt-3 flex items-center space-x-2 text-primary-600 hover:text-primary-700"
                >
                  <PlusIcon className="w-4 h-4" />
                  <span>Add Answer Option</span>
                </Button>
              )}
            </div>

            {/* Settings */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 font-montserrat">
                Poll Settings
              </h3>

              <div className="flex items-center space-x-3">
                <input
                  type="checkbox"
                  id="allowMultiple"
                  checked={formData.allowMultipleResponses}
                  onChange={(e) => setFormData({ ...formData, allowMultipleResponses: e.target.checked })}
                  className="w-4 h-4 text-primary-600 bg-gray-100 border-gray-300 rounded focus:ring-primary-500 focus:ring-2"
                />
                <label htmlFor="allowMultiple" className="text-sm font-medium text-gray-700 font-montserrat">
                  Allow multiple responses per user
                </label>
              </div>

              <Input
                type="datetime-local"
                label="Expiration Date (Optional)"
                value={formData.expiresAt}
                onChange={(e) => setFormData({ ...formData, expiresAt: e.target.value })}
                helperText="Leave empty for no expiration"
              />
            </div>

            <div className="flex flex-col sm:flex-row gap-3 pt-4">
              <Button
                type="submit"
                variant="primary"
                isLoading={isLoading}
                className="flex-1"
              >
                Update Poll
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={onClose}
                className="flex-1"
              >
                Cancel
              </Button>
            </div>
          </form>
        </Card>
      </motion.div>
    </motion.div>
  );
};

export default PollEditForm;