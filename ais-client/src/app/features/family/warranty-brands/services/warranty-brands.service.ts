import { HttpClient, httpResource } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WarrantyBrandsCollectionResponse, WarrantyBrandResponse } from '../models/warranty-brand.response';
import { CreateWarrantyBrandRequest } from '../models/create-warranty-brand.request';
import { AuthService } from '../../../../core/auth/auth.service';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';
import { inlineErrors } from '../../../../core/http/http-context';

@Injectable({ providedIn: 'root' })
export class WarrantyBrandsService {
  readonly #http = inject(HttpClient);
  readonly #authService = inject(AuthService);

  /**
   * Keyed on the signed-in user: the resource goes idle and drops its value on logout and
   * refetches for the next user, so one user's brands never show up for another.
   */
  readonly warrantyBrands = httpResource<WarrantyBrandsCollectionResponse>(() =>
    this.#authService.userId() ? { url: apiUrl(Endpoints.warrantyBrands) } : undefined
  );

  create(request: CreateWarrantyBrandRequest): Observable<WarrantyBrandResponse> {
    const formData = new FormData();
    formData.append('name', request.name);
    if (request.logo) {
      formData.append('logo', request.logo);
    }

    return this.#http.post<WarrantyBrandResponse>(apiUrl(Endpoints.warrantyBrands), formData, {
      context: inlineErrors(),
    });
  }

  delete(id: string): Observable<void> {
    return this.#http.delete<void>(apiUrl(Endpoints.warrantyBrands, id));
  }
}
