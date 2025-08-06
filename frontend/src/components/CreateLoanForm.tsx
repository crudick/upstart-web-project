import React, { useState } from 'react';

interface LoanFormData {
  userId: string;
  loanAmount: string;
  interestRate: string;
  termMonths: string;
  monthlyPayment: string;
  loanPurpose: string;
  loanStatus: string;
  applicationDate: string;
  approvalDate: string;
  disbursementDate: string;
  maturityDate: string;
  outstandingBalance: string;
  totalPaymentsMade: string;
  nextPaymentDueDate: string;
  paymentFrequency: string;
  lateFees: string;
  originationFee: string;
  apr: string;
  loanOfficerNotes: string;
}

interface CreateLoanFormProps {
  onSubmit: (loanData: LoanFormData) => void;
}

const CreateLoanForm: React.FC<CreateLoanFormProps> = ({ onSubmit }) => {
  const [formData, setFormData] = useState<LoanFormData>({
    userId: '',
    loanAmount: '',
    interestRate: '',
    termMonths: '',
    monthlyPayment: '',
    loanPurpose: '',
    loanStatus: 'Pending',
    applicationDate: new Date().toISOString().split('T')[0],
    approvalDate: '',
    disbursementDate: '',
    maturityDate: '',
    outstandingBalance: '',
    totalPaymentsMade: '0',
    nextPaymentDueDate: '',
    paymentFrequency: 'Monthly',
    lateFees: '0',
    originationFee: '',
    apr: '',
    loanOfficerNotes: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <div className="form-container">
      <h2>Create New Loan</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>User ID *</label>
          <input
            type="number"
            name="userId"
            value={formData.userId}
            onChange={handleChange}
            required
            placeholder="Enter the user ID"
          />
        </div>

        <div className="form-group">
          <label>Loan Amount *</label>
          <input
            type="number"
            name="loanAmount"
            value={formData.loanAmount}
            onChange={handleChange}
            required
            step="0.01"
            placeholder="10000.00"
          />
        </div>

        <div className="form-group">
          <label>Interest Rate *</label>
          <input
            type="number"
            name="interestRate"
            value={formData.interestRate}
            onChange={handleChange}
            required
            step="0.01"
            placeholder="5.25"
          />
        </div>

        <div className="form-group">
          <label>Term (Months) *</label>
          <input
            type="number"
            name="termMonths"
            value={formData.termMonths}
            onChange={handleChange}
            required
            placeholder="36"
          />
        </div>

        <div className="form-group">
          <label>Monthly Payment *</label>
          <input
            type="number"
            name="monthlyPayment"
            value={formData.monthlyPayment}
            onChange={handleChange}
            required
            step="0.01"
            placeholder="300.00"
          />
        </div>

        <div className="form-group">
          <label>Loan Purpose *</label>
          <select
            name="loanPurpose"
            value={formData.loanPurpose}
            onChange={handleChange}
            required
          >
            <option value="">Select purpose...</option>
            <option value="Debt Consolidation">Debt Consolidation</option>
            <option value="Home Improvement">Home Improvement</option>
            <option value="Auto Purchase">Auto Purchase</option>
            <option value="Medical">Medical</option>
            <option value="Education">Education</option>
            <option value="Business">Business</option>
            <option value="Other">Other</option>
          </select>
        </div>

        <div className="form-group">
          <label>Loan Status *</label>
          <select
            name="loanStatus"
            value={formData.loanStatus}
            onChange={handleChange}
            required
          >
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Rejected">Rejected</option>
            <option value="Active">Active</option>
            <option value="Closed">Closed</option>
          </select>
        </div>

        <div className="form-group">
          <label>Application Date *</label>
          <input
            type="date"
            name="applicationDate"
            value={formData.applicationDate}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Approval Date</label>
          <input
            type="date"
            name="approvalDate"
            value={formData.approvalDate}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Disbursement Date</label>
          <input
            type="date"
            name="disbursementDate"
            value={formData.disbursementDate}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Maturity Date</label>
          <input
            type="date"
            name="maturityDate"
            value={formData.maturityDate}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Outstanding Balance *</label>
          <input
            type="number"
            name="outstandingBalance"
            value={formData.outstandingBalance}
            onChange={handleChange}
            required
            step="0.01"
            placeholder="10000.00"
          />
        </div>

        <div className="form-group">
          <label>Total Payments Made *</label>
          <input
            type="number"
            name="totalPaymentsMade"
            value={formData.totalPaymentsMade}
            onChange={handleChange}
            required
            step="0.01"
          />
        </div>

        <div className="form-group">
          <label>Next Payment Due Date</label>
          <input
            type="date"
            name="nextPaymentDueDate"
            value={formData.nextPaymentDueDate}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Payment Frequency *</label>
          <select
            name="paymentFrequency"
            value={formData.paymentFrequency}
            onChange={handleChange}
            required
          >
            <option value="Monthly">Monthly</option>
            <option value="Bi-weekly">Bi-weekly</option>
            <option value="Weekly">Weekly</option>
            <option value="Quarterly">Quarterly</option>
          </select>
        </div>

        <div className="form-group">
          <label>Late Fees *</label>
          <input
            type="number"
            name="lateFees"
            value={formData.lateFees}
            onChange={handleChange}
            required
            step="0.01"
          />
        </div>

        <div className="form-group">
          <label>Origination Fee *</label>
          <input
            type="number"
            name="originationFee"
            value={formData.originationFee}
            onChange={handleChange}
            required
            step="0.01"
            placeholder="100.00"
          />
        </div>

        <div className="form-group">
          <label>APR</label>
          <input
            type="number"
            name="apr"
            value={formData.apr}
            onChange={handleChange}
            step="0.01"
            placeholder="5.99"
          />
        </div>

        <div className="form-group">
          <label>Loan Officer Notes</label>
          <textarea
            name="loanOfficerNotes"
            value={formData.loanOfficerNotes}
            onChange={handleChange}
            rows={3}
            placeholder="Additional notes..."
          />
        </div>

        <button type="submit" className="submit-btn">
          Create Loan
        </button>
      </form>
    </div>
  );
};

export default CreateLoanForm;