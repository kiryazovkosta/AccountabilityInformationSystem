export interface StorageFileResponse {
  id: string;
  originalFileName: string;
  contentType: string;
  sizeBytes: number;
  url?: string;
}

export interface WarrantyBrandResponse {
  id: string;
  name: string;
}

export interface WarrantyRecordResponse {
  id: string;
  warrantyBrand: WarrantyBrandResponse;
  model: string;
  purchaseDate: Date;
  duration: number;
  endDate: Date;
  receipt?: StorageFileResponse;
  frontImage?: StorageFileResponse;
  backImage?: StorageFileResponse;

}
