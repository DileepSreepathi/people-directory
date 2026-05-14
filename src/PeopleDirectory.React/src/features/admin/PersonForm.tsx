import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getAdminPersonById, getCountries, getCitiesByCountry, createPerson, updatePerson } from '../../api/services';
import type { Country, City, PersonDetail } from '../../types';
import './Admin.css';

export default function PersonForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [mobileNumber, setMobileNumber] = useState('');
  const [gender, setGender] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [countryId, setCountryId] = useState<number>(0);
  const [cityId, setCityId] = useState<number>(0);
  const [addressLine, setAddressLine] = useState('');
  const [profilePicture, setProfilePicture] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const [countries, setCountries] = useState<Country[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    getCountries().then(setCountries);
  }, []);

  useEffect(() => {
    if (countryId > 0) {
      getCitiesByCountry(countryId).then(setCities);
    } else {
      setCities([]);
    }
  }, [countryId]);

  useEffect(() => {
    if (isEdit && id) {
      getAdminPersonById(Number(id)).then((p: PersonDetail) => {
        setFirstName(p.firstName);
        setLastName(p.lastName);
        setEmail(p.email);
        setMobileNumber(p.mobileNumber || '');
        setGender(p.gender || '');
        setDateOfBirth(p.dateOfBirth || '');
        setCountryId(p.countryId);
        setCityId(p.cityId);
        setAddressLine(p.addressLine || '');
        if (p.profilePictureUrl) setPreviewUrl(p.profilePictureUrl);
      });
    }
  }, [isEdit, id]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setProfilePicture(file);
      setPreviewUrl(URL.createObjectURL(file));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrors({});

    const formData = new FormData();
    formData.append('firstName', firstName);
    formData.append('lastName', lastName);
    formData.append('email', email);
    if (mobileNumber) formData.append('mobileNumber', mobileNumber);
    if (gender) formData.append('gender', gender);
    if (dateOfBirth) formData.append('dateOfBirth', dateOfBirth);
    formData.append('cityId', String(cityId));
    if (addressLine) formData.append('addressLine', addressLine);
    if (profilePicture) formData.append('profilePicture', profilePicture);

    try {
      if (isEdit) {
        await updatePerson(Number(id), formData);
      } else {
        await createPerson(formData);
      }
      navigate('/admin');
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: Array<{ propertyName: string; errorMessage: string }> } };
      if (axiosErr.response?.data && Array.isArray(axiosErr.response.data)) {
        const fieldErrors: Record<string, string> = {};
        axiosErr.response.data.forEach((e: { propertyName: string; errorMessage: string }) => {
          fieldErrors[e.propertyName] = e.errorMessage;
        });
        setErrors(fieldErrors);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="admin-page">
      <h1>{isEdit ? 'Edit Person' : 'Add New Person'}</h1>

      <form onSubmit={handleSubmit} className="person-form">
        <div className="form-row">
          <div className="form-group">
            <label>First Name *</label>
            <input type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
            {errors.FirstName && <span className="field-error">{errors.FirstName}</span>}
          </div>
          <div className="form-group">
            <label>Last Name *</label>
            <input type="text" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
            {errors.LastName && <span className="field-error">{errors.LastName}</span>}
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Email *</label>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            {errors.Email && <span className="field-error">{errors.Email}</span>}
          </div>
          <div className="form-group">
            <label>Mobile Number</label>
            <input type="text" value={mobileNumber} onChange={(e) => setMobileNumber(e.target.value)} />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Gender</label>
            <select value={gender} onChange={(e) => setGender(e.target.value)}>
              <option value="">Select...</option>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
          <div className="form-group">
            <label>Date of Birth</label>
            <input type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Country *</label>
            <select value={countryId} onChange={(e) => { setCountryId(Number(e.target.value)); setCityId(0); }} required>
              <option value={0}>Select Country...</option>
              {countries.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <div className="form-group">
            <label>City *</label>
            <select value={cityId} onChange={(e) => setCityId(Number(e.target.value))} required disabled={!countryId}>
              <option value={0}>Select City...</option>
              {cities.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
            {errors.CityId && <span className="field-error">{errors.CityId}</span>}
          </div>
        </div>

        <div className="form-group">
          <label>Address</label>
          <input type="text" value={addressLine} onChange={(e) => setAddressLine(e.target.value)} />
        </div>

        <div className="form-group">
          <label>Profile Picture</label>
          <input type="file" accept="image/*" onChange={handleFileChange} />
          {previewUrl && <img src={previewUrl} alt="Preview" className="image-preview" />}
        </div>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : (isEdit ? 'Update Person' : 'Create Person')}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/admin')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}
