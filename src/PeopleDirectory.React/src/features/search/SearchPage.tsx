import { useState, useEffect, useRef, useCallback } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { searchPeople, getPeople, getCountries, getCitiesByCountry } from '../../api/services';
import { useDebounce } from '../../hooks/useDebounce';
import type { SearchResult, PersonSummary, Country, City, PagedResult } from '../../types';
import './SearchPage.css';

const PAGE_SIZE = 12;

type SortOption = '' | 'name_desc' | 'newest' | 'oldest' | 'country';

export default function SearchPage() {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

  // Form state
  const [query, setQuery] = useState(searchParams.get('query') || '');
  const [countryId, setCountryId] = useState<number | undefined>(
    searchParams.get('countryId') ? Number(searchParams.get('countryId')) : undefined
  );
  const [cityId, setCityId] = useState<number | undefined>(
    searchParams.get('cityId') ? Number(searchParams.get('cityId')) : undefined
  );
  const [gender, setGender] = useState(searchParams.get('gender') || '');
  const [sortBy, setSortBy] = useState<SortOption>((searchParams.get('sortBy') as SortOption) || '');
  const [page, setPage] = useState(Number(searchParams.get('page') || 1));

  // Data state
  const [countries, setCountries] = useState<Country[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [results, setResults] = useState<PagedResult<PersonSummary> | null>(null);
  const [loading, setLoading] = useState(false);

  // Type-ahead state
  const [suggestions, setSuggestions] = useState<SearchResult[]>([]);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const inputWrapperRef = useRef<HTMLDivElement>(null);

  const debouncedQuery = useDebounce(query, 300);

  // Load countries once
  useEffect(() => {
    getCountries().then(setCountries);
  }, []);

  // Cascading city dropdown
  useEffect(() => {
    if (countryId) {
      getCitiesByCountry(countryId).then(setCities);
    } else {
      setCities([]);
      setCityId(undefined);
    }
  }, [countryId]);

  // Type-ahead suggestions (separate from grid)
  useEffect(() => {
    if (debouncedQuery.length >= 2) {
      searchPeople(debouncedQuery).then(setSuggestions);
    } else {
      setSuggestions([]);
    }
  }, [debouncedQuery]);

  // Hide suggestions on outside click
  useEffect(() => {
    const onClickOutside = (e: MouseEvent) => {
      if (inputWrapperRef.current && !inputWrapperRef.current.contains(e.target as Node)) {
        setShowSuggestions(false);
      }
    };
    document.addEventListener('mousedown', onClickOutside);
    return () => document.removeEventListener('mousedown', onClickOutside);
  }, []);

  // Fetch grid results whenever any filter, sort, query, or page changes.
  const fetchResults = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, string> = {};
      if (debouncedQuery) params.query = debouncedQuery;
      if (countryId) params.countryId = String(countryId);
      if (cityId) params.cityId = String(cityId);
      if (gender) params.gender = gender;
      if (sortBy) params.sortBy = sortBy;
      params.page = String(page);
      setSearchParams(params, { replace: true });

      const data = await getPeople({
        query: debouncedQuery || undefined,
        countryId,
        cityId,
        gender: gender || undefined,
        sortBy: sortBy || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      setResults(data);
    } finally {
      setLoading(false);
    }
  }, [debouncedQuery, countryId, cityId, gender, sortBy, page, setSearchParams]);

  useEffect(() => {
    fetchResults();
  }, [fetchResults]);

  // Reset to page 1 when filters or query change
  useEffect(() => {
    setPage(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedQuery, countryId, cityId, gender, sortBy]);

  const handleClearFilters = () => {
    setQuery('');
    setCountryId(undefined);
    setCityId(undefined);
    setGender('');
    setSortBy('');
    setPage(1);
  };

  const hasActiveFilters = Boolean(query || countryId || cityId || gender || sortBy);

  return (
    <div className="search-page">
      <header className="search-header">
        <h1>People Directory</h1>
        <p>Search by name and filter by country, city or gender</p>
      </header>

      <section className="search-form" aria-label="Search and filters">
        <div className="search-input-wrapper" ref={inputWrapperRef}>
          <input
            type="text"
            value={query}
            onChange={(e) => { setQuery(e.target.value); setShowSuggestions(true); }}
            onFocus={() => setShowSuggestions(true)}
            placeholder="Start typing a first name or surname..."
            className="search-input"
            aria-label="Search by first name or last name"
            autoComplete="off"
          />

          {showSuggestions && query.length >= 2 && suggestions.length > 0 && (
            <ul className="suggestions-dropdown" role="listbox">
              {suggestions.map((s) => (
                <li
                  key={s.id}
                  role="option"
                  onClick={() => { setShowSuggestions(false); navigate(`/people/${s.id}`); }}
                >
                  <strong>{s.fullName}</strong>
                  {s.cityName && <span className="suggestion-location"> — {s.cityName}, {s.countryName}</span>}
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="filters">
          <select
            value={countryId || ''}
            onChange={(e) => setCountryId(e.target.value ? Number(e.target.value) : undefined)}
            aria-label="Filter by country"
          >
            <option value="">All Countries</option>
            {countries.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>

          <select
            value={cityId || ''}
            onChange={(e) => setCityId(e.target.value ? Number(e.target.value) : undefined)}
            disabled={!countryId}
            aria-label="Filter by city"
          >
            <option value="">{countryId ? 'All Cities' : 'Select a country first'}</option>
            {cities.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>

          <select value={gender} onChange={(e) => setGender(e.target.value)} aria-label="Filter by gender">
            <option value="">All Genders</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
            <option value="Other">Other</option>
          </select>

          <select value={sortBy} onChange={(e) => setSortBy(e.target.value as SortOption)} aria-label="Sort by">
            <option value="">Sort: Name (A–Z)</option>
            <option value="name_desc">Name (Z–A)</option>
            <option value="country">Country (A–Z)</option>
            <option value="newest">Newest first</option>
            <option value="oldest">Oldest first</option>
          </select>

          {hasActiveFilters && (
            <button type="button" className="btn-clear" onClick={handleClearFilters}>
              Clear filters
            </button>
          )}
        </div>
      </section>

      {loading && <div className="loading">Loading...</div>}

      {results && !loading && (
        <section className="results-section" aria-live="polite">
          <p className="results-count">
            {results.totalCount === 0
              ? 'No people match your search.'
              : `Showing ${((results.page - 1) * results.pageSize) + 1}–${Math.min(results.page * results.pageSize, results.totalCount)} of ${results.totalCount} people`}
          </p>

          {results.items.length > 0 && (
            <div className="results-grid">
              {results.items.map((p) => (
                <button
                  key={p.id}
                  type="button"
                  className="person-card"
                  onClick={() => navigate(`/people/${p.id}`)}
                  aria-label={`View ${p.firstName} ${p.lastName}`}
                >
                  <div className="person-avatar">
                    {p.profilePictureUrl
                      ? <img src={p.profilePictureUrl} alt="" />
                      : <div className="avatar-placeholder">{p.firstName[0]}{p.lastName[0]}</div>}
                  </div>
                  <div className="person-info">
                    <h3>{p.firstName} {p.lastName}</h3>
                    <p className="person-email">{p.email}</p>
                    <p className="person-location">{p.cityName}, {p.countryName}</p>
                    {p.gender && <span className={`badge badge-${p.gender.toLowerCase()}`}>{p.gender}</span>}
                  </div>
                </button>
              ))}
            </div>
          )}

          {results.totalPages > 1 && (
            <nav className="pagination" aria-label="Pagination">
              <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
              <span>Page {page} of {results.totalPages}</span>
              <button disabled={page >= results.totalPages} onClick={() => setPage(page + 1)}>Next</button>
            </nav>
          )}
        </section>
      )}
    </div>
  );
}
