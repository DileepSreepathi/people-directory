import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getAdminPeople, deletePerson } from '../../api/services';
import { useAuth } from '../../context/AuthContext';
import { useDebounce } from '../../hooks/useDebounce';
import type { PagedResult, PersonSummary } from '../../types';
import './Admin.css';

export default function AdminDashboard() {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const [data, setData] = useState<PagedResult<PersonSummary> | null>(null);
  const [page, setPage] = useState(1);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(true);
  const debouncedQuery = useDebounce(query, 300);

  const fetchData = async (p: number, q: string) => {
    setLoading(true);
    const result = await getAdminPeople({ query: q || undefined, page: p, pageSize: 10 });
    setData(result);
    setLoading(false);
  };

  // Reset to first page when search changes
  useEffect(() => { setPage(1); }, [debouncedQuery]);

  useEffect(() => {
    fetchData(page, debouncedQuery);
  }, [page, debouncedQuery]);

  const handleDelete = async (id: number, name: string) => {
    if (!confirm(`Are you sure you want to delete ${name}?`)) return;
    await deletePerson(id);
    fetchData(page, debouncedQuery);
  };

  return (
    <div className="admin-page">
      <div className="admin-header">
        <h1>Admin Dashboard</h1>
        <div className="admin-actions">
          <button className="btn btn-primary" onClick={() => navigate('/admin/people/new')}>+ Add New Person</button>
          <button className="btn btn-secondary" onClick={() => { logout(); navigate('/admin/login'); }}>Logout</button>
        </div>
      </div>

      <div className="admin-toolbar">
        <input
          type="text"
          className="admin-search"
          placeholder="Search by first name or surname..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          aria-label="Search people"
        />
        {query && (
          <button type="button" className="btn-clear" onClick={() => setQuery('')}>Clear</button>
        )}
      </div>

      {loading && <div className="loading">Loading...</div>}

      {data && !loading && (
        <>
          <p className="results-count">
            {data.totalCount === 0
              ? 'No people found.'
              : `${data.totalCount} ${data.totalCount === 1 ? 'person' : 'people'} total`}
          </p>

          {data.items.length > 0 && (
            <table className="admin-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>City</th>
                  <th>Country</th>
                  <th>Gender</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((p) => (
                  <tr key={p.id}>
                    <td>{p.firstName} {p.lastName}</td>
                    <td>{p.email}</td>
                    <td>{p.cityName}</td>
                    <td>{p.countryName}</td>
                    <td>{p.gender}</td>
                    <td className="actions">
                      <button className="btn-sm btn-edit" onClick={() => navigate(`/admin/people/${p.id}/edit`)}>Edit</button>
                      <button className="btn-sm btn-delete" onClick={() => handleDelete(p.id, `${p.firstName} ${p.lastName}`)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {data.totalPages > 1 && (
            <div className="pagination">
              <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
              <span>Page {page} of {data.totalPages}</span>
              <button disabled={page >= data.totalPages} onClick={() => setPage(page + 1)}>Next</button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
