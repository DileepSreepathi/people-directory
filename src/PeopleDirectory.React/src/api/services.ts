import api from './client';
import type { AuthResponse, City, Country, PagedResult, PersonDetail, PersonSummary, SearchResult } from '../types';

// Public People API
export const searchPeople = (query: string) =>
  api.get<SearchResult[]>(`/people/search?query=${encodeURIComponent(query)}`).then(r => r.data);

export const getPeople = (params: {
  query?: string;
  countryId?: number;
  cityId?: number;
  gender?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}) =>
  api.get<PagedResult<PersonSummary>>('/people', { params }).then(r => r.data);

export const getPersonById = (id: number) =>
  api.get<PersonDetail>(`/people/${id}`).then(r => r.data);

// Locations API
export const getCountries = () =>
  api.get<Country[]>('/locations/countries').then(r => r.data);

export const getCitiesByCountry = (countryId: number) =>
  api.get<City[]>(`/locations/countries/${countryId}/cities`).then(r => r.data);

// Auth API
export const login = (email: string, password: string) =>
  api.post<AuthResponse>('/auth/login', { email, password }).then(r => r.data);

// Admin People API
export const getAdminPeople = (params: { query?: string; page?: number; pageSize?: number }) =>
  api.get<PagedResult<PersonSummary>>('/admin/people', { params }).then(r => r.data);

export const getAdminPersonById = (id: number) =>
  api.get<PersonDetail>(`/admin/people/${id}`).then(r => r.data);

export const createPerson = (formData: FormData) =>
  api.post<PersonDetail>('/admin/people', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  }).then(r => r.data);

export const updatePerson = (id: number, formData: FormData) =>
  api.put<PersonDetail>(`/admin/people/${id}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  }).then(r => r.data);

export const deletePerson = (id: number) =>
  api.delete(`/admin/people/${id}`);
