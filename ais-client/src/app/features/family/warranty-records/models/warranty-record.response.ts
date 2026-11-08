import { WarrantyBrandResponse } from '../../warranty-brands/models/warranty-brand.response';

export interface StorageFileResponse {
  id: string;
  originalFileName: string;
  contentType: string;
  sizeBytes: number;
  url?: string;
}

export interface WarrantyRecordResponse {
  id: string;
  warrantyBrand: WarrantyBrandResponse;
  model: string;
  /** ISO 8601 date string as sent by the API. */
  purchaseDate: string;
  duration: number;
  /** ISO 8601 date string as sent by the API. */
  endDate: string;
  receipt?: StorageFileResponse;
  frontImage?: StorageFileResponse;
  backImage?: StorageFileResponse;
}
