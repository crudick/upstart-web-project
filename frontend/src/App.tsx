import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, useParams, useNavigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import PublicLandingPage from './components/pages/PublicLandingPage';
import DashboardLayout from './components/layout/DashboardLayout';
import DashboardPage from './components/pages/DashboardPage';
import PollsPage from './components/pages/PollsPage';
import CommunityPollsPage from './components/pages/CommunityPollsPage';
import PollCreationForm from './components/PollCreationForm';
import PollView from './components/PollView';
import PollResults from './components/PollResults';

// Poll Page Component
const PollPage: React.FC = () => {
  const { guid } = useParams<{ guid: string }>();
  const [showResults, setShowResults] = useState(false);

  if (!guid) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background-light">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 font-montserrat mb-2">
            Invalid Poll URL
          </h1>
          <p className="text-gray-600 font-montserrat">
            The poll you're looking for doesn't exist or has been removed.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background-light">
      <div className="max-w-4xl mx-auto px-4 py-8">
        {showResults ? (
          <PollResults 
            pollGuid={guid} 
            onBackToPoll={() => setShowResults(false)} 
          />
        ) : (
          <PollView 
            pollGuid={guid}
            onViewResults={() => setShowResults(true)}
          />
        )}
      </div>
    </div>
  );
};

// Authenticated Dashboard Component
const AuthenticatedApp: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<'dashboard' | 'polls' | 'community'>('dashboard');
  const [showCreatePoll, setShowCreatePoll] = useState(false);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleCreatePoll = () => {
    setShowCreatePoll(true);
  };

  const handlePollCreated = () => {
    setRefreshTrigger(prev => prev + 1);
    if (activeTab !== 'polls') {
      setActiveTab('polls');
    }
  };

  const handleViewPoll = (pollGuid: string) => {
    navigate(`/poll/${pollGuid}`);
  };

  return (
    <>
      <DashboardLayout 
        activeTab={activeTab}
        onTabChange={setActiveTab}
        onCreatePoll={handleCreatePoll}
      >
        {activeTab === 'dashboard' ? (
          <DashboardPage 
            key={refreshTrigger}
            onCreatePoll={handleCreatePoll}
            onViewPolls={() => setActiveTab('polls')}
          />
        ) : activeTab === 'polls' ? (
          <PollsPage 
            key={refreshTrigger}
            onCreatePoll={handleCreatePoll}
            onViewPoll={handleViewPoll}
          />
        ) : (
          <CommunityPollsPage />
        )}
      </DashboardLayout>

      {showCreatePoll && (
        <PollCreationForm 
          onClose={() => setShowCreatePoll(false)}
          onPollCreated={handlePollCreated}
        />
      )}
    </>
  );
};

// Main App Router Component
const AppRouter: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();

  const handleViewPoll = (pollGuid: string) => {
    navigate(`/poll/${pollGuid}`);
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background-light">
        <div className="text-center">
          <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600 font-montserrat">Loading...</p>
        </div>
      </div>
    );
  }

  return (
    <Routes>
      <Route 
        path="/" 
        element={
          isAuthenticated ? (
            <AuthenticatedApp />
          ) : (
            <PublicLandingPage onViewPoll={handleViewPoll} />
          )
        } 
      />
      <Route path="/poll/:guid" element={<PollPage />} />
    </Routes>
  );
};

// Root App Component
function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="font-montserrat">
          <AppRouter />
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;