export interface PersonSummary {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  cityName?: string;
  countryName?: string;
  profilePictureUrl?: string;
  gender?: string;
}

export interface PersonDetail {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber?: string;
  gender?: string;
  profilePictureUrl?: string;
  dateOfBirth?: string;
  cityId: number;
  cityName?: string;
  countryId: number;
  countryName?: string;
  addressLine?: string;
  createdAt: string;
  updatedAt: string;
}

export interface SearchResult {
  id: number;
  fullName: string;
  cityName?: string;
  countryName?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Country {
  id: number;
  name: string;
  code: string;
}

export interface City {
  id: number;
  name: string;
  countryId: number;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
}

export interface PersonFormData {
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber?: string;
  gender?: string;
  dateOfBirth?: string;
  cityId: number;
  addressLine?: string;
}
