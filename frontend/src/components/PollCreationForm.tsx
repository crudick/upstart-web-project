import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { PlusIcon, TrashIcon, CalendarIcon } from '@heroicons/react/24/outline';
import Button from './ui/Button';
import Input from './ui/Input';
import Card from './ui/Card';
import { CreatePollRequest } from '../types';
import { pollsAPI, pollAnswersAPI } from '../services/api';

interface PollCreationFormProps {
  onClose: () => void;
  onPollCreated: () => void;
}

const PollCreationForm: React.FC<PollCreationFormProps> = ({ onClose, onPollCreated }) => {
  const [formData, setFormData] = useState<CreatePollRequest>({
    question: '',
    answers: ['', ''],
    expiresAt: '',
    allowMultipleResponses: false,
    requiresAuthentication: false,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    // Validation
    if (formData.question.trim().length < 5) {
      setError('Question must be at least 5 characters long');
      setIsLoading(false);
      return;
    }

    const validAnswers = formData.answers.filter(answer => answer.trim().length > 0);
    if (validAnswers.length < 2) {
      setError('At least 2 answer options are required');
      setIsLoading(false);
      return;
    }

    try {
      // Create the poll
      const poll = await pollsAPI.createPoll({
        question: formData.question.trim(),
        answers: validAnswers,
        expiresAt: formData.expiresAt || undefined,
        allowMultipleResponses: formData.allowMultipleResponses,
        requiresAuthentication: formData.requiresAuthentication,
      });

      // Create poll answers
      await Promise.all(
        validAnswers.map((answerText, index) =>
          pollAnswersAPI.createPollAnswer(poll.id, answerText.trim(), index + 1)
        )
      );

      onPollCreated();
      onClose();
    } catch (err: any) {
      setError(err.message || 'Failed to create poll. Please try again.');
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
    setFormData({
      ...formData,
      answers: newAnswers,
    });
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
              Create New Poll
            </h2>
            <Button variant="ghost" onClick={onClose} className="p-2">
              <span className="text-2xl">&times;</span>
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
                    className="flex items-center space-x-2"
                  >
                    <div className="flex-1">
                      <Input
                        placeholder={`Option ${index + 1}`}
                        value={answer}
                        onChange={(e) => updateAnswer(index, e.target.value)}
                      />
                    </div>
                    {formData.answers.length > 2 && (
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => removeAnswer(index)}
                        className="text-red-600 hover:text-red-700 hover:bg-red-50 p-2"
                      >
                        <TrashIcon className="w-4 h-4" />
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
                  className="mt-3 text-primary-600 hover:text-primary-700 flex items-center space-x-2"
                >
                  <PlusIcon className="w-4 h-4" />
                  <span>Add Option</span>
                </Button>
              )}
            </div>

            {/* Expiration Date */}
            <Input
              type="datetime-local"
              label="Expiration Date (Optional)"
              helperText="Leave empty for polls that never expire"
              leftIcon={<CalendarIcon className="w-5 h-5" />}
              value={formData.expiresAt}
              onChange={(e) => setFormData({ ...formData, expiresAt: e.target.value })}
              min={new Date().toISOString().slice(0, 16)}
            />

            {/* Multiple Responses */}
            <div className="flex items-center space-x-3">
              <input
                type="checkbox"
                id="allowMultiple"
                checked={formData.allowMultipleResponses}
                onChange={(e) => setFormData({ ...formData, allowMultipleResponses: e.target.checked })}
                className="w-4 h-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
              />
              <label htmlFor="allowMultiple" className="text-sm text-gray-700 font-montserrat">
                Allow multiple responses from the same user
              </label>
            </div>

            {/* Require Authentication */}
            <div className="flex items-center space-x-3">
              <input
                type="checkbox"
                id="requireAuth"
                checked={formData.requiresAuthentication}
                onChange={(e) => setFormData({ ...formData, requiresAuthentication: e.target.checked })}
                className="w-4 h-4 text-primary-600 border-gray-300 rounded focus:ring-primary-500"
              />
              <label htmlFor="requireAuth" className="text-sm text-gray-700 font-montserrat">
                Require users to sign in before voting
              </label>
            </div>

            {/* Submit Buttons */}
            <div className="flex space-x-3 pt-4">
              <Button
                type="button"
                variant="ghost"
                onClick={onClose}
                className="flex-1"
                disabled={isLoading}
              >
                Cancel
              </Button>
              <Button
                type="submit"
                variant="primary"
                isLoading={isLoading}
                className="flex-1"
              >
                Create Poll
              </Button>
            </div>
          </form>
        </Card>
      </motion.div>
    </motion.div>
  );
};

export default PollCreationForm;