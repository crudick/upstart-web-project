const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5166';

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  socialSecurityNumber?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  annualIncome?: number;
  employmentStatus?: string;
  creditScore?: number;
}

export interface CreateLoanRequest {
  userId: number;
  loanAmount: number;
  interestRate: number;
  termMonths: number;
  monthlyPayment: number;
  loanPurpose: string;
  loanStatus: string;
  applicationDate: string;
  approvalDate?: string;
  disbursementDate?: string;
  maturityDate?: string;
  outstandingBalance: number;
  totalPaymentsMade: number;
  nextPaymentDueDate?: string;
  paymentFrequency: string;
  lateFees: number;
  originationFee: number;
  apr?: number;
  loanOfficerNotes?: string;
}

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

export interface LoanResponse {
  id: number;
  userId: number;
  loanAmount: number;
  loanStatus: string;
}

async function makeApiCall<T>(url: string, options: RequestInit): Promise<T> {
  try {
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`API Error: ${response.status} - ${errorText}`);
    }

    return await response.json();
  } catch (error) {
    console.error('API call failed:', error);
    throw error;
  }
}

export async function createUser(userData: CreateUserRequest): Promise<UserResponse> {
  const processedData = {
    ...userData,
    phoneNumber: userData.phoneNumber || null,
    dateOfBirth: userData.dateOfBirth ? new Date(userData.dateOfBirth).toISOString() : null,
    socialSecurityNumber: userData.socialSecurityNumber || null,
    addressLine1: userData.addressLine1 || null,
    addressLine2: userData.addressLine2 || null,
    city: userData.city || null,
    state: userData.state || null,
    zipCode: userData.zipCode || null,
    annualIncome: userData.annualIncome ? Number(userData.annualIncome) : null,
    employmentStatus: userData.employmentStatus || null,
    creditScore: userData.creditScore ? Number(userData.creditScore) : null,
  };

  return makeApiCall<UserResponse>(`${API_BASE_URL}/api/users`, {
    method: 'POST',
    body: JSON.stringify(processedData),
  });
}

export async function createLoan(loanData: CreateLoanRequest): Promise<LoanResponse> {
  const processedData = {
    ...loanData,
    userId: Number(loanData.userId),
    loanAmount: Number(loanData.loanAmount),
    interestRate: Number(loanData.interestRate),
    termMonths: Number(loanData.termMonths),
    monthlyPayment: Number(loanData.monthlyPayment),
    applicationDate: new Date(loanData.applicationDate).toISOString(),
    approvalDate: loanData.approvalDate ? new Date(loanData.approvalDate).toISOString() : null,
    disbursementDate: loanData.disbursementDate ? new Date(loanData.disbursementDate).toISOString() : null,
    maturityDate: loanData.maturityDate ? new Date(loanData.maturityDate).toISOString() : null,
    outstandingBalance: Number(loanData.outstandingBalance),
    totalPaymentsMade: Number(loanData.totalPaymentsMade),
    nextPaymentDueDate: loanData.nextPaymentDueDate ? new Date(loanData.nextPaymentDueDate).toISOString() : null,
    lateFees: Number(loanData.lateFees),
    originationFee: Number(loanData.originationFee),
    apr: loanData.apr ? Number(loanData.apr) : null,
    loanOfficerNotes: loanData.loanOfficerNotes || null,
  };

  return makeApiCall<LoanResponse>(`${API_BASE_URL}/api/loans`, {
    method: 'POST',
    body: JSON.stringify(processedData),
  });
}