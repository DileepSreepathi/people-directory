import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getPersonById } from '../../api/services';
import type { PersonDetail } from '../../types';
import './PersonDetailPage.css';

export default function PersonDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [person, setPerson] = useState<PersonDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      getPersonById(Number(id)).then(setPerson).finally(() => setLoading(false));
    }
  }, [id]);

  if (loading) return <div className="loading">Loading...</div>;
  if (!person) return <div className="not-found">Person not found.</div>;

  return (
    <div className="detail-page">
      <button className="btn btn-secondary back-btn" onClick={() => navigate(-1)}>
        &larr; Back to results
      </button>

      <div className="detail-card">
        <div className="detail-header">
          <div className="detail-avatar">
            {person.profilePictureUrl
              ? <img src={person.profilePictureUrl} alt={`${person.firstName} ${person.lastName}`} />
              : <div className="avatar-placeholder large">{person.firstName[0]}{person.lastName[0]}</div>
            }
          </div>
          <div>
            <h1>{person.firstName} {person.lastName}</h1>
            {person.gender && <span className={`badge badge-${person.gender.toLowerCase()}`}>{person.gender}</span>}
          </div>
        </div>

        <div className="detail-grid">
          <div className="detail-field">
            <label>Email</label>
            <p><a href={`mailto:${person.email}`}>{person.email}</a></p>
          </div>
          <div className="detail-field">
            <label>Mobile Number</label>
            <p>{person.mobileNumber || '—'}</p>
          </div>
          <div className="detail-field">
            <label>Address</label>
            <p>{person.addressLine || '—'}</p>
          </div>
          <div className="detail-field">
            <label>City</label>
            <p>{person.cityName}</p>
          </div>
          <div className="detail-field">
            <label>Country</label>
            <p>{person.countryName}</p>
          </div>
          <div className="detail-field">
            <label>Date of Birth</label>
            <p>{person.dateOfBirth || '—'}</p>
          </div>
        </div>
      </div>
    </div>
  );
}
