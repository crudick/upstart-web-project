import React, { useState } from 'react';
import { INPUTS, BUTTONS, TEXT, LAYOUT } from '../styles';

interface UserFormData {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  socialSecurityNumber: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  zipCode: string;
  annualIncome: string;
  employmentStatus: string;
  creditScore: string;
}

interface CreateUserFormProps {
  onSubmit: (userData: UserFormData) => void;
}

const CreateUserForm: React.FC<CreateUserFormProps> = ({ onSubmit }) => {
  const [formData, setFormData] = useState<UserFormData>({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    dateOfBirth: '',
    socialSecurityNumber: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    zipCode: '',
    annualIncome: '',
    employmentStatus: '',
    creditScore: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
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
      <h2 className={TEXT.title}>Create New User</h2>
      <form onSubmit={handleSubmit}>
        {/* Personal Information Section */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Personal Information</h3>
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                First Name <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                className={INPUTS.text}
                required
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Last Name <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                className={INPUTS.text}
                required
              />
            </div>
          </div>

          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>
              Email <span className={INPUTS.required}>*</span>
            </label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className={INPUTS.text}
              required
            />
          </div>

          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Phone Number</label>
              <input
                type="tel"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="(555) 123-4567"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Date of Birth</label>
              <input
                type="date"
                name="dateOfBirth"
                value={formData.dateOfBirth}
                onChange={handleChange}
                className={INPUTS.text}
              />
            </div>
          </div>

          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>Social Security Number</label>
            <input
              type="text"
              name="socialSecurityNumber"
              value={formData.socialSecurityNumber}
              onChange={handleChange}
              className={INPUTS.text}
              placeholder="XXX-XX-XXXX"
            />
          </div>
        </div>

        {/* Address Information Section */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Address Information</h3>
          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>Address Line 1</label>
            <input
              type="text"
              name="addressLine1"
              value={formData.addressLine1}
              onChange={handleChange}
              className={INPUTS.text}
              placeholder="123 Main Street"
            />
          </div>

          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>Address Line 2</label>
            <input
              type="text"
              name="addressLine2"
              value={formData.addressLine2}
              onChange={handleChange}
              className={INPUTS.text}
              placeholder="Apt, Suite, Unit (optional)"
            />
          </div>

          <div className={LAYOUT.threeColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>City</label>
              <input
                type="text"
                name="city"
                value={formData.city}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="San Francisco"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>State</label>
              <input
                type="text"
                name="state"
                value={formData.state}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="CA"
                maxLength={2}
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Zip Code</label>
              <input
                type="text"
                name="zipCode"
                value={formData.zipCode}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="94101"
              />
            </div>
          </div>
        </div>

        {/* Financial Information Section */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Financial Information</h3>
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Annual Income</label>
              <input
                type="number"
                name="annualIncome"
                value={formData.annualIncome}
                onChange={handleChange}
                className={INPUTS.text}
                placeholder="50000"
              />
            </div>

            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Employment Status</label>
              <select
                name="employmentStatus"
                value={formData.employmentStatus}
                onChange={handleChange}
                className={INPUTS.select}
              >
                <option value="">Select employment status...</option>
                <option value="Full-time">Full-time</option>
                <option value="Part-time">Part-time</option>
                <option value="Self-employed">Self-employed</option>
                <option value="Unemployed">Unemployed</option>
                <option value="Retired">Retired</option>
              </select>
            </div>
          </div>

          <div className={INPUTS.fieldGroup}>
            <label className={INPUTS.label}>Credit Score</label>
            <input
              type="number"
              name="creditScore"
              value={formData.creditScore}
              onChange={handleChange}
              className={INPUTS.text}
              min="300"
              max="850"
              placeholder="700"
            />
          </div>
        </div>

        <button type="submit" className={BUTTONS.primary}>
          Create User
        </button>
      </form>
    </div>
  );
};

export default CreateUserForm;