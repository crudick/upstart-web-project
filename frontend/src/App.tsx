import React, { useState } from 'react';
import CreateUserForm from './components/CreateUserForm';
import CreateLoanForm from './components/CreateLoanForm';
import { createUser, createLoan, CreateUserRequest, CreateLoanRequest } from './services/api';
import { FORM_CONTAINERS, TEXT, LAYOUT } from './styles';

function App() {
  const [activeForm, setActiveForm] = useState<'user' | 'loan'>('user');
  const [message, setMessage] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);

  const handleUserSubmit = async (userData: any) => {
    setLoading(true);
    setMessage('');
    
    try {
      const userRequest: CreateUserRequest = {
        firstName: userData.firstName,
        lastName: userData.lastName,
        email: userData.email,
        phoneNumber: userData.phoneNumber || undefined,
        dateOfBirth: userData.dateOfBirth || undefined,
        socialSecurityNumber: userData.socialSecurityNumber || undefined,
        addressLine1: userData.addressLine1 || undefined,
        addressLine2: userData.addressLine2 || undefined,
        city: userData.city || undefined,
        state: userData.state || undefined,
        zipCode: userData.zipCode || undefined,
        annualIncome: userData.annualIncome ? Number(userData.annualIncome) : undefined,
        employmentStatus: userData.employmentStatus || undefined,
        creditScore: userData.creditScore ? Number(userData.creditScore) : undefined,
      };

      const response = await createUser(userRequest);
      setMessage(`User created successfully! User ID: ${response.id}`);
    } catch (error) {
      setMessage(`Error creating user: ${error instanceof Error ? error.message : 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };

  const handleLoanSubmit = async (loanData: any) => {
    setLoading(true);
    setMessage('');
    
    try {
      const loanRequest: CreateLoanRequest = {
        userId: Number(loanData.userId),
        loanAmount: Number(loanData.loanAmount),
        interestRate: Number(loanData.interestRate),
        termMonths: Number(loanData.termMonths),
        monthlyPayment: Number(loanData.monthlyPayment),
        loanPurpose: loanData.loanPurpose,
        loanStatus: loanData.loanStatus,
        applicationDate: loanData.applicationDate,
        approvalDate: loanData.approvalDate || undefined,
        disbursementDate: loanData.disbursementDate || undefined,
        maturityDate: loanData.maturityDate || undefined,
        outstandingBalance: Number(loanData.outstandingBalance),
        totalPaymentsMade: Number(loanData.totalPaymentsMade),
        nextPaymentDueDate: loanData.nextPaymentDueDate || undefined,
        paymentFrequency: loanData.paymentFrequency,
        lateFees: Number(loanData.lateFees),
        originationFee: Number(loanData.originationFee),
        apr: loanData.apr ? Number(loanData.apr) : undefined,
        loanOfficerNotes: loanData.loanOfficerNotes || undefined,
      };

      const response = await createLoan(loanRequest);
      setMessage(`Loan created successfully! Loan ID: ${response.id}`);
    } catch (error) {
      setMessage(`Error creating loan: ${error instanceof Error ? error.message : 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={FORM_CONTAINERS.main}>
      <div className="w-full max-w-4xl">
        {/* Header */}
        <header className="text-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-8">
            Upstart Loan Management
          </h1>
          
          {/* Navigation */}
          <nav className={LAYOUT.nav}>
            <button 
              className={`px-6 py-3 font-medium rounded-lg transition-colors ${
                activeForm === 'user' 
                  ? 'bg-primary-600 text-white' 
                  : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
              }`}
              onClick={() => setActiveForm('user')}
            >
              Create User
            </button>
            <button 
              className={`px-6 py-3 font-medium rounded-lg transition-colors ${
                activeForm === 'loan' 
                  ? 'bg-primary-600 text-white' 
                  : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
              }`}
              onClick={() => setActiveForm('loan')}
            >
              Create Loan
            </button>
          </nav>
        </header>

        {/* Messages */}
        {message && (
          <div className={message.includes('Error') ? TEXT.error : TEXT.success}>
            {message}
          </div>
        )}

        {loading && (
          <div className="text-center py-8">
            <div className="inline-flex items-center px-4 py-2 font-semibold leading-6 text-sm shadow rounded-md text-white bg-primary-600 transition ease-in-out duration-150 cursor-not-allowed">
              <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Processing...
            </div>
          </div>
        )}

        {/* Forms */}
        {activeForm === 'user' && !loading && (
          <CreateUserForm onSubmit={handleUserSubmit} />
        )}

        {activeForm === 'loan' && !loading && (
          <CreateLoanForm onSubmit={handleLoanSubmit} />
        )}
      </div>
    </div>
  );
}

export default App;
