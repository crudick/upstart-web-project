import React, { useState } from 'react';
import './App.css';
import CreateUserForm from './components/CreateUserForm';
import CreateLoanForm from './components/CreateLoanForm';
import { createUser, createLoan, CreateUserRequest, CreateLoanRequest } from './services/api';

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
    <div className="App">
      <header className="App-header">
        <h1>Upstart Loan Management</h1>
        <nav className="form-nav">
          <button 
            className={activeForm === 'user' ? 'active' : ''}
            onClick={() => setActiveForm('user')}
          >
            Create User
          </button>
          <button 
            className={activeForm === 'loan' ? 'active' : ''}
            onClick={() => setActiveForm('loan')}
          >
            Create Loan
          </button>
        </nav>
      </header>

      <main className="App-main">
        {message && (
          <div className={`message ${message.includes('Error') ? 'error' : 'success'}`}>
            {message}
          </div>
        )}

        {loading && (
          <div className="loading">
            Processing...
          </div>
        )}

        {activeForm === 'user' && (
          <CreateUserForm onSubmit={handleUserSubmit} />
        )}

        {activeForm === 'loan' && (
          <CreateLoanForm onSubmit={handleLoanSubmit} />
        )}
      </main>
    </div>
  );
}

export default App;
