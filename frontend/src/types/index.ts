export interface User {
  id: number;
  firstName?: string;
  lastName?: string;
  email: string;
  phoneNumber?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Poll {
  id: number;
  pollGuid: string;
  question: string;
  expiresAt?: string;
  allowMultipleResponses: boolean;
  requiresAuthentication: boolean;
  userId: number;
  user?: User;
  answers: PollAnswer[];
  stats: PollStat[];
  createdAt: string;
  updatedAt: string;
}

export interface PollAnswer {
  id: number;
  pollId: number;
  answerText: string;
  displayOrder: number;
  poll?: Poll;
  stats: PollStat[];
  createdAt: string;
  updatedAt: string;
}

export interface PollStat {
  id: number;
  pollId: number;
  pollAnswerId: number;
  userId: number;
  user?: User;
  poll?: Poll;
  pollAnswer?: PollAnswer;
  selectedAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface UpdateProfileRequest {
  firstName?: string;
  lastName?: string;
}

export interface CreatePollRequest {
  question: string;
  answers: string[];
  expiresAt?: string;
  allowMultipleResponses?: boolean;
  requiresAuthentication?: boolean;
}

export interface PollResults {
  pollId: number;
  totalResponses: number;
  results: {
    answerId: number;
    answerText: string;
    count: number;
    percentage: number;
  }[];
}