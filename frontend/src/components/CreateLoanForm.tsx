import React, { useState } from 'react';
import { INPUTS, BUTTONS, TEXT, LAYOUT } from '../styles';

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
    <div className="bg-white shadow-lg rounded-lg p-8 w-full max-w-2xl mx-auto">
      <h2 className={TEXT.title}>Create New Loan</h2>
      <form onSubmit={handleSubmit}>
        
        {/* Basic Loan Information */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Basic Loan Information</h3>
          
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                User ID <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="number"
                name="userId"
                value={formData.userId}
                onChange={handleChange}
                className={INPUTS.text}
                required
                placeholder="Enter user ID"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Loan Amount <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="number"
                name="loanAmount"
                value={formData.loanAmount}
                onChange={handleChange}
                className={INPUTS.text}
                required
                placeholder="50000"
                step="0.01"
              />
            </div>
          </div>

          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Interest Rate (%) <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="number"
                name="interestRate"
                value={formData.interestRate}
                onChange={handleChange}
                className={INPUTS.text}
                required
                placeholder="5.5"
                step="0.01"
                min="0"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Term (Months) <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="number"
                name="termMonths"
                value={formData.termMonths}
                onChange={handleChange}
                className={INPUTS.text}
                required
                placeholder="36"
                min="1"
              />
            </div>
          </div>

          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Monthly Payment <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="number"
                name="monthlyPayment"
                value={formData.monthlyPayment}
                onChange={handleChange}
                className={INPUTS.text}
                required
                placeholder="1500.00"
                step="0.01"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Loan Purpose <span className={INPUTS.required}>*</span>
              </label>
              <select
                name="loanPurpose"
                value={formData.loanPurpose}
                onChange={handleChange}
                className={INPUTS.select}
                required
              >
                <option value="">Select purpose...</option>
                <option value="Debt Consolidation">Debt Consolidation</option>
                <option value="Home Improvement">Home Improvement</option>
                <option value="Personal">Personal</option>
                <option value="Auto">Auto</option>
                <option value="Medical">Medical</option>
                <option value="Business">Business</option>
                <option value="Other">Other</option>
              </select>
            </div>
          </div>
        </div>

        {/* Loan Status and Dates */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Status & Important Dates</h3>
          
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Loan Status</label>
              <select
                name="loanStatus"
                value={formData.loanStatus}
                onChange={handleChange}
                className={INPUTS.select}
              >
                <option value="Pending">Pending</option>
                <option value="Approved">Approved</option>
                <option value="Active">Active</option>
                <option value="Paid Off">Paid Off</option>
                <option value="Defaulted">Defaulted</option>
              </select>
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Application Date</label>
              <input
                type="date"
                name="applicationDate"
                value={formData.applicationDate}
                onChange={handleChange}
                className={INPUTS.text}
              />
            </div>
          </div>

          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Approval Date</label>
              <input
                type="date"
                name="approvalDate"
                value={formData.approvalDate}
                onChange={handleChange}
                className={INPUTS.text}
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Disbursement Date</label>
              <input
                type="date"
                name="disbursementDate"
                value={formData.disbursementDate}
                onChange={handleChange}
                className={INPUTS.text}
              />
            </div>
          </div>
        </div>

        {/* Financial Details */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Financial Details</h3>
          
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Outstanding Balance</label>
              <input
                type="number"
                name="outstandingBalance"
                value={formData.outstandingBalance}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="45000.00"
                step="0.01"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Total Payments Made</label>
              <input
                type="number"
                name="totalPaymentsMade"
                value={formData.totalPaymentsMade}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="0"
                step="0.01"
              />
            </div>
          </div>

          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Payment Frequency</label>
              <select
                name="paymentFrequency"
                value={formData.paymentFrequency}
                onChange={handleChange}
                className={INPUTS.select}
              >
                <option value="Weekly">Weekly</option>
                <option value="Bi-weekly">Bi-weekly</option>
                <option value="Monthly">Monthly</option>
                <option value="Quarterly">Quarterly</option>
              </select>
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Origination Fee</label>
              <input
                type="number"
                name="originationFee"
                value={formData.originationFee}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="500.00"
                step="0.01"
              />
            </div>
          </div>
        </div>

        {/* Additional Information */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Additional Information</h3>
          
          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>Loan Officer Notes</label>
            <textarea
              name="loanOfficerNotes"
              value={formData.loanOfficerNotes}
              onChange={handleChange}
              className={INPUTS.textarea}
              rows={4}
              placeholder="Any additional notes about this loan..."
            />
          </div>
        </div>

        <button type="submit" className={BUTTONS.primary}>
          Create Loan
        </button>
      </form>
    </div>
  );
};

export default CreateLoanForm;