export interface CreateWarrantyRecordRequest {
  warrantyBrandId: string;
  model: string;
  purchaseDate: string;
  receipt?: File;
  frontImage?: File;
  backImage?: File;
  duration: number;
}
