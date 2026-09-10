export interface WarrantyBrandResponse {
  id: string;
  name: string;
  logo?: string;
  warranties: [string];
}

export interface WarrantyBrandsCollectionResponse {
  items: WarrantyBrandResponse[];
}
