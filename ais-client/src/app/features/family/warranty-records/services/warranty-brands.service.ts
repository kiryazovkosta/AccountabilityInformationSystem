import { httpResource } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { WarrantyBrandsCollectionResponse } from '../models/warranty-brand.response';

@Injectable()
export class WarrantyBrandsService {
  readonly warrantyBrands = httpResource<WarrantyBrandsCollectionResponse>(() => ({
    url: `${environment.apiBaseUrl}api/family/warranty-brands`,
    withCredentials: true,
  }));
}
