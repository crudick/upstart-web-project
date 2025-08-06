import React, { useState } from 'react';

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
    <div className="form-container">
      <h2>Create New User</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>First Name *</label>
          <input
            type="text"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Last Name *</label>
          <input
            type="text"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Email *</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Phone Number</label>
          <input
            type="tel"
            name="phoneNumber"
            value={formData.phoneNumber}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Date of Birth</label>
          <input
            type="date"
            name="dateOfBirth"
            value={formData.dateOfBirth}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Social Security Number</label>
          <input
            type="text"
            name="socialSecurityNumber"
            value={formData.socialSecurityNumber}
            onChange={handleChange}
            placeholder="XXX-XX-XXXX"
          />
        </div>

        <div className="form-group">
          <label>Address Line 1</label>
          <input
            type="text"
            name="addressLine1"
            value={formData.addressLine1}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Address Line 2</label>
          <input
            type="text"
            name="addressLine2"
            value={formData.addressLine2}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>City</label>
          <input
            type="text"
            name="city"
            value={formData.city}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>State</label>
          <input
            type="text"
            name="state"
            value={formData.state}
            onChange={handleChange}
            placeholder="CA"
          />
        </div>

        <div className="form-group">
          <label>Zip Code</label>
          <input
            type="text"
            name="zipCode"
            value={formData.zipCode}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label>Annual Income</label>
          <input
            type="number"
            name="annualIncome"
            value={formData.annualIncome}
            onChange={handleChange}
            placeholder="50000"
          />
        </div>

        <div className="form-group">
          <label>Employment Status</label>
          <select
            name="employmentStatus"
            value={formData.employmentStatus}
            onChange={handleChange}
          >
            <option value="">Select...</option>
            <option value="Full-time">Full-time</option>
            <option value="Part-time">Part-time</option>
            <option value="Self-employed">Self-employed</option>
            <option value="Unemployed">Unemployed</option>
            <option value="Retired">Retired</option>
          </select>
        </div>

        <div className="form-group">
          <label>Credit Score</label>
          <input
            type="number"
            name="creditScore"
            value={formData.creditScore}
            onChange={handleChange}
            min="300"
            max="850"
            placeholder="700"
          />
        </div>

        <button type="submit" className="submit-btn">
          Create User
        </button>
      </form>
    </div>
  );
};

export default CreateUserForm;