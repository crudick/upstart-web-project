import {
  Poll,
  PollAnswer,
  PollStat,
  User,
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UpdateProfileRequest,
  CreatePollRequest,
  PollResults,
} from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5166';

// CSRF token management
let csrfToken: string | null = null;

const getCsrfToken = async (): Promise<string> => {
  if (!csrfToken) {
    const response = await fetch(`${API_BASE_URL}/api/csrf/token`, {
      credentials: 'include',
    });
    const data = await response.json();
    csrfToken = data.token;
  }
  return csrfToken!; // Non-null assertion since we just set it above
};

// Reset CSRF token (call this when token becomes invalid)
export const resetCsrfToken = () => {
  csrfToken = null;
};

// Utility function to get cookie value
function getCookie(name: string): string | null {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift() || null;
  return null;
}

// Session management utilities
export const sessionUtils = {
  getSessionId(): string | null {
    return getCookie('upstart_session');
  },
  
  getOrCreateSessionId(): string {
    let sessionId = this.getSessionId();
    if (!sessionId) {
      sessionId = crypto.randomUUID();
      // Set cookie with 1 year expiry
      const expires = new Date();
      expires.setFullYear(expires.getFullYear() + 1);
      document.cookie = `upstart_session=${sessionId}; path=/; expires=${expires.toUTCString()}; secure; samesite=lax`;
    }
    return sessionId;
  },
  
  hasSession(): boolean {
    return this.getSessionId() !== null;
  }
};

class APIError extends Error {
  constructor(public status: number, message: string) {
    super(message);
    this.name = 'APIError';
  }
}

async function makeApiCall<T>(
  url: string,
  options: RequestInit = {},
  requiresAuth = false,
  requiresCsrf = true,
  requiresSession = false
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((options.headers as Record<string, string>) || {}),
  };

  if (requiresAuth) {
    const token = localStorage.getItem('token');
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }
  }

  // Add CSRF token for state-changing operations
  if (requiresCsrf && (options.method === 'POST' || options.method === 'PUT' || options.method === 'DELETE')) {
    try {
      const token = await getCsrfToken();
      headers['X-CSRF-TOKEN'] = token;
    } catch (error) {
      console.warn('Failed to get CSRF token:', error);
    }
  }

  // Add session ID header for unauthenticated requests that need session context
  if (!requiresAuth && (requiresSession || options.method === 'POST' || options.method === 'PUT' || options.method === 'DELETE')) {
    const sessionId = sessionUtils.getOrCreateSessionId();
    headers['X-Session-ID'] = sessionId;
  }

  // Always include cookies for session management
  const requestOptions: RequestInit = {
    ...options,
    headers,
    credentials: 'include',
  };

  try {
    const response = await fetch(url, requestOptions);

    if (!response.ok) {
      let errorMessage = `HTTP ${response.status}`;
      try {
        const errorData = await response.json();
        
        // Handle validation errors (dictionary format from FluentValidation)
        if (typeof errorData === 'object' && errorData !== null && !errorData.message && !errorData.error) {
          // Extract validation errors from dictionary format
          const validationErrors: string[] = [];
          Object.values(errorData).forEach((errors: any) => {
            if (Array.isArray(errors)) {
              validationErrors.push(...errors);
            }
          });
          if (validationErrors.length > 0) {
            errorMessage = validationErrors.join(', ');
          }
        } else {
          // Handle standard error format
          errorMessage = errorData.message || errorData.error || errorMessage;
        }
      } catch {
        try {
          errorMessage = await response.text() || errorMessage;
        } catch {
          // If we can't read the response, use the status
          errorMessage = `HTTP ${response.status}`;
        }
      }
      throw new APIError(response.status, errorMessage);
    }

    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      return await response.json();
    }
    return {} as T;
  } catch (error) {
    if (error instanceof APIError) {
      throw error;
    }
    console.error('API call failed:', error);
    throw new APIError(0, 'Network error occurred');
  }
}

// Authentication API
export const authAPI = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    return makeApiCall<AuthResponse>(`${API_BASE_URL}/api/auth/login`, {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
  },

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    // Include session ID for poll migration during registration
    const sessionId = sessionUtils.getSessionId(); // Get existing session if available
    const requestBody = sessionId ? { ...userData, sessionId } : userData;
    
    return makeApiCall<AuthResponse>(`${API_BASE_URL}/api/auth/register`, {
      method: 'POST',
      body: JSON.stringify(requestBody),
    });
  },

  async getCurrentUser(): Promise<User> {
    return makeApiCall<User>(`${API_BASE_URL}/api/auth/me`, {}, true);
  },
};

// Polls API
export const pollsAPI = {
  async getPublicPolls(): Promise<Poll[]> {
    return makeApiCall<Poll[]>(`${API_BASE_URL}/api/polls/public`);
  },

  async getUserPolls(): Promise<Poll[]> {
    return makeApiCall<Poll[]>(`${API_BASE_URL}/api/polls/user`, {}, true);
  },

  async getPollByGuid(guid: string): Promise<Poll> {
    return makeApiCall<Poll>(`${API_BASE_URL}/api/polls/guid/${encodeURIComponent(guid)}`);
  },

  async createPoll(pollData: CreatePollRequest): Promise<Poll> {
    // Transform frontend request to backend format
    const backendRequest = {
      question: pollData.question,
      isActive: true,
      isMultipleChoice: pollData.allowMultipleResponses || false,
      requiresAuthentication: pollData.requiresAuthentication || false,
      expiresAt: pollData.expiresAt || null
    };
    
    return makeApiCall<Poll>(`${API_BASE_URL}/api/polls`, {
      method: 'POST',
      body: JSON.stringify(backendRequest),
    }, false); // Changed from true to false - no auth required
  },

  async updatePoll(pollId: number, pollData: Partial<CreatePollRequest>): Promise<Poll> {
    // Transform frontend request to backend format
    const backendRequest = {
      question: pollData.question,
      isActive: true,
      isMultipleChoice: pollData.allowMultipleResponses || false,
      requiresAuthentication: pollData.requiresAuthentication || false,
      expiresAt: pollData.expiresAt || null
    };
    
    return makeApiCall<Poll>(`${API_BASE_URL}/api/polls/${pollId}`, {
      method: 'PUT',
      body: JSON.stringify(backendRequest),
    }, true);
  },

  async deletePoll(pollId: number): Promise<void> {
    return makeApiCall<void>(`${API_BASE_URL}/api/polls/${pollId}`, {
      method: 'DELETE',
    }, true);
  },

  async replaceAnswersForPoll(pollId: number, answers: string[]): Promise<void> {
    return makeApiCall<void>(`${API_BASE_URL}/api/polls/${pollId}/answers`, {
      method: 'PUT',
      body: JSON.stringify(answers),
    }, true);
  },
};

// Poll Answers API
export const pollAnswersAPI = {
  async getPollAnswers(pollId: number): Promise<PollAnswer[]> {
    return makeApiCall<PollAnswer[]>(`${API_BASE_URL}/api/poll-answers/poll/${pollId}`);
  },

  async createPollAnswer(pollId: number, answerText: string, displayOrder: number): Promise<PollAnswer> {
    return makeApiCall<PollAnswer>(`${API_BASE_URL}/api/poll-answers`, {
      method: 'POST',
      body: JSON.stringify({ pollId, answerText, displayOrder }),
    }, false); // Changed from true to false - no auth required
  },

  async deletePollAnswer(answerId: number): Promise<void> {
    return makeApiCall<void>(`${API_BASE_URL}/api/poll-answers/${answerId}`, {
      method: 'DELETE',
    }, true);
  },
};

// Poll Stats API
export const pollStatsAPI = {
  async submitVote(pollId: number, pollAnswerId: number): Promise<PollStat> {
    return makeApiCall<PollStat>(`${API_BASE_URL}/api/poll-stats`, {
      method: 'POST',
      body: JSON.stringify({ pollId, pollAnswerId, userId: 0 }), // userId will be set by backend from JWT
    }, true);
  },

  async submitAnonymousVote(pollId: number, pollAnswerId: number): Promise<PollStat> {
    return makeApiCall<PollStat>(`${API_BASE_URL}/api/poll-stats/anonymous`, {
      method: 'POST',
      body: JSON.stringify({ pollId, pollAnswerId, userId: 0 }),
    }, false);
  },

  async getPollResults(pollId: number): Promise<PollResults> {
    return makeApiCall<PollResults>(`${API_BASE_URL}/api/poll-stats/poll/${pollId}/results`);
  },

  async getUserPollResponse(pollId: number): Promise<PollStat | null> {
    try {
      return await makeApiCall<PollStat>(
        `${API_BASE_URL}/api/poll-stats/poll/${pollId}/user/me`,
        {},
        true
      );
    } catch (error) {
      if (error instanceof APIError && error.status === 404) {
        return null;
      }
      throw error;
    }
  },

  async getSessionPollResponse(pollId: number): Promise<PollStat | null> {
    try {
      return await makeApiCall<PollStat>(
        `${API_BASE_URL}/api/poll-stats/poll/${pollId}/session/me`,
        {},
        false,
        true,
        true // requiresSession = true
      );
    } catch (error) {
      if (error instanceof APIError && error.status === 404) {
        return null;
      }
      throw error;
    }
  },

  async updatePollResponse(responseId: number, pollAnswerId: number): Promise<PollStat> {
    return makeApiCall<PollStat>(`${API_BASE_URL}/api/poll-stats/${responseId}`, {
      method: 'PUT',
      body: JSON.stringify({ pollAnswerId }),
    }, false); // Works for both authenticated and unauthenticated users
  },

  async getUserStats(): Promise<{
    totalPolls: number;
    totalVotes: number;
    totalViews: number;
    recentActivity: any[];
  }> {
    return makeApiCall<{
      totalPolls: number;
      totalVotes: number;
      totalViews: number;
      recentActivity: any[];
    }>(`${API_BASE_URL}/api/poll-stats/user/dashboard`, {}, true);
  },
};

// Main API object
export const api = {
  async updateProfile(profileData: UpdateProfileRequest): Promise<User> {
    return makeApiCall<User>(`${API_BASE_URL}/api/users/me`, {
      method: 'PUT',
      body: JSON.stringify(profileData),
    }, true);
  },
};

// Export individual functions for backward compatibility
export const createUser = authAPI.register;
export const createPoll = pollsAPI.createPoll;
export const getPollByGuid = pollsAPI.getPollByGuid;
export const getPollAnswers = pollAnswersAPI.getPollAnswers;
export const createPollAnswer = pollAnswersAPI.createPollAnswer;
export const createPollStat = pollStatsAPI.submitVote;
export const getPollResults = pollStatsAPI.getPollResults;
export const getUserPollResponse = pollStatsAPI.getUserPollResponse;