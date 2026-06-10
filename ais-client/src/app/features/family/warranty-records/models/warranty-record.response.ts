export interface WarrantyRecordResponse {
  id: string;
  warrantyBrandId: string;
  model: string;
  purchaseDate: string;
  receipt?: string;
  frontImage?: string;
  backImage?: string;
}
