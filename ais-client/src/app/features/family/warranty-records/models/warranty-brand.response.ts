export interface WarrantyBrandResponse {
  id: string;
  name: string;
  logo?: string;
}

export interface WarrantyBrandsCollectionResponse {
  items: WarrantyBrandResponse[];
}
