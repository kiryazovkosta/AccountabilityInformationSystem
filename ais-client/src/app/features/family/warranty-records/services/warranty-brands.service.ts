import { HttpClient, httpResource } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { WarrantyBrandResponse, WarrantyBrandsCollectionResponse } from '../models/warranty-brand.response';
import { CreateWarrantyBrandRequest } from '../models/create-warranty-brand.request';

@Injectable({ providedIn: 'root' })
export class WarrantyBrandsService {
  readonly #http = inject(HttpClient);

  readonly warrantyBrandId = signal<string | undefined>(undefined);

  readonly warrantyBrands = httpResource<WarrantyBrandsCollectionResponse>(() => ({
    url: `${environment.apiBaseUrl}api/family/warranty-brands`,
    withCredentials: true,
  }));

  readonly warrantyBrand = httpResource<WarrantyBrandResponse>(() =>
    this.warrantyBrandId()
      ? {
          url: `${environment.apiBaseUrl}api/family/warranty-brands/${this.warrantyBrandId()}`,
          withCredentials: true,
        }
      : undefined
  );

  create(request: CreateWarrantyBrandRequest): Observable<WarrantyBrandResponse> {
    const formData = new FormData();
    formData.append('name', request.name);
    if (request.logo) {
      formData.append('logo', request.logo);
    }

    return this.#http.post<WarrantyBrandResponse>(
      `${environment.apiBaseUrl}api/family/warranty-brands`,
      formData,
      { withCredentials: true }
    );
  }

  delete(id: string): Observable<void> {
    return this.#http.delete<void>(
      `${environment.apiBaseUrl}api/family/warranty-brands/${id}`,
      { withCredentials: true }
    );
  }
}
