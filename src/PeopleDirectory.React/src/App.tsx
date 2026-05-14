import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute } from './routes/ProtectedRoute';
import SearchPage from './features/search/SearchPage';
import PersonDetailPage from './features/detail/PersonDetailPage';
import LoginPage from './features/admin/LoginPage';
import AdminDashboard from './features/admin/AdminDashboard';
import PersonForm from './features/admin/PersonForm';
import './App.css';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <nav className="navbar">
          <Link to="/" className="nav-brand">People Directory</Link>
          <div className="nav-links">
            <Link to="/">Search</Link>
            <Link to="/admin">Admin</Link>
          </div>
        </nav>

        <main>
          <Routes>
            {/* Client Routes */}
            <Route path="/" element={<SearchPage />} />
            <Route path="/people/:id" element={<PersonDetailPage />} />

            {/* Admin Routes */}
            <Route path="/admin/login" element={<LoginPage />} />
            <Route path="/admin" element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />
            <Route path="/admin/people/new" element={<ProtectedRoute><PersonForm /></ProtectedRoute>} />
            <Route path="/admin/people/:id/edit" element={<ProtectedRoute><PersonForm /></ProtectedRoute>} />
          </Routes>
        </main>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
